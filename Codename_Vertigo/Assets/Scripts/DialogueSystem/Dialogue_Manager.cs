using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class Dialogue_Manager : MonoBehaviour
{
    public static Dialogue_Manager instance;

    [Header("Dialogue UI")]
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI speakerText;
    [SerializeField] Animator panelAnimator;
    [SerializeField] Animator portraitAnimator;
    [SerializeField] Animator layoutAnimator;
    [SerializeField] GameObject portraitPanel;
    [SerializeField] GameObject speakerPanel;

    [Header("Choices UI")]
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject[] choices;
    TextMeshProUGUI[] choicesTexts;

    [Header("Dialogue Audio")]
    [SerializeField] AudioSource dmAudioSource;
    [SerializeField] AudioClip[] dialogueSFX;

    Story currentStory;

    public bool dialogueIsPlaying { get; private set; }
    public float normalTypeSpeed;
    public float fastTypeSpeed;
    float _typeSpeed;

    public float _currTextCount { get; private set; }

    private const string speaker_TAG = "speaker";
    private const string potrait_TAG = "portrait";
    private const string layout_TAG = "layout";

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        choicesTexts = new TextMeshProUGUI[choices.Length];

        for(int i = 0; i < choices.Length; i++)
        {
            choicesTexts[i] = choices[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        dmAudioSource = GetComponent<AudioSource>();

    }

    public void SetDialogueSpeed()
    {
        _typeSpeed = fastTypeSpeed;
    }

    public void StartDialogue(TextAsset inkJSON, string inkFlow)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        
        panelAnimator.SetBool("isOpening", true);
        panelAnimator.SetBool("isClosing", false);

        if(inkFlow != "")
        {
            
            currentStory.ChoosePathString(inkFlow);
            
        }

        speakerPanel.gameObject.SetActive(false);
        //Set portrait animation to a default
        portraitPanel.SetActive(false);
        //Set layout animation to a default
        layoutAnimator.Play("right");

        ContinueDialogue();
    }

    public void ContinueDialogue()
    {
        if (currentStory.canContinue)
        {
            string textToAdd = currentStory.Continue();
            
            StartCoroutine(TypeDialogueCo(textToAdd));
            DisplayChoices();
            //Handle the dialogue's tags
            HandleTags(currentStory.currentTags);
        }
        else
        {
            StartCoroutine(EndDialogue());
        }
    }

    void HandleTags(List<string> currentTags)
    {
        foreach(string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                //If the length of the split tag is not 2, flag error
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case speaker_TAG:
                    if (tagValue == "None")
                    {
                        speakerPanel.SetActive(false);
                    }
                    else
                    {
                        speakerPanel.SetActive(true);
                        speakerText.text = tagValue;
                    }
                    break;
                case potrait_TAG:
                    if(tagValue == "None")
                    {
                        portraitPanel.SetActive(false);
                    }
                    else
                    {
                        portraitPanel.SetActive(true);
                    }
                    break;
                case layout_TAG:
                    layoutAnimator.Play(tagValue);
                    break;
                default:
                    break;
            }
        }
    }

    public IEnumerator EndDialogue()
    {
        yield return new WaitForSeconds(.2f);
        dialogueIsPlaying = false;
        dialogueText.text = "";
        _typeSpeed = normalTypeSpeed;


        //Set the panel to false after animation
        panelAnimator.SetBool("isOpening", false);
        panelAnimator.SetBool("isClosing", true);
    }

    IEnumerator TypeDialogueCo(string textToPrint)
    {
        _typeSpeed = normalTypeSpeed;
        dialogueText.text = "";
        char[] dialogueChars = textToPrint.ToCharArray();
        _currTextCount = dialogueChars.Length;

        for(int i = 0; i < dialogueChars.Length; i++)
        {
            dialogueText.text += dialogueChars[i];

            dmAudioSource.Play();


            _currTextCount--;
            Debug.Log(_currTextCount);
            yield return new WaitForSeconds(_typeSpeed);
        }
    }

    

    void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        continueButton.SetActive(false);

        if(currentChoices.Count == 0)
        {
            continueButton.SetActive(true);
            
        }

        if(currentChoices.Count > choices.Length)
        {
            //More choices than can be supported by the UI, flag an error
        }

        for(int i = 0; i < choices.Length; i++)
        {
            if(i <= currentChoices.Count - 1)
            {
                choices[i].SetActive(true);
                choicesTexts[i].text = currentChoices[i].text;
                //choicesTexts[i].text = "Choice" + (i + 1);
            }
            else
            {
                choices[i].SetActive(false);
            }
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
    }
}

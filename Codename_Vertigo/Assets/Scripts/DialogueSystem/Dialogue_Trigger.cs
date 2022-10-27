using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class Dialogue_Trigger : MonoBehaviour
{
    [Header("Boolean Toggles")]
    [SerializeField] bool triggerOnEnter;
    [SerializeField] bool triggerOnce;
    [SerializeField] bool triggered;

    public string inkFlow;

    [Header("Visual Cues")]
    [SerializeField] GameObject visualCue;

    [Header("Text Asset")]
    [SerializeField] TextAsset inkJSON;

    bool playerInRange;

    private void FixedUpdate()
    {
        if(visualCue != null)
        {
            if (playerInRange)
            {
                visualCue.SetActive(true);
                //Check for player input here, trigger dialogue if it isn't currently playing
                if (!Dialogue_Manager.instance.dialogueIsPlaying)
                {

                }
                
            }
            else
            {
                visualCue.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnEnter)
            {
                TriggerDialogue();
            }
            else
            {
                playerInRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void TriggerDialogue()
    {
        if(triggerOnce && !triggered)
        {
            if (inkJSON != null)
            {
                triggered = true;
                Dialogue_Manager.instance.StartDialogue(inkJSON, inkFlow);
            }
        }
        else
        {

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject optionsPanel;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!menuPanel.activeInHierarchy)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
    }

    #region Main Game Menu Methods

    void OpenMenu()
    {
        menuPanel.SetActive(true);
        animator.SetBool("isOpening", true);
        animator.SetBool("isClosing", false);
        //Set the canvas to active
        //Play the animation for opening the menu
    }

    public void CloseMenu()
    {
        animator.SetBool("isOpening", false);
        animator.SetBool("isClosing", true);
    }

    //Allow for manual saving via the menu
    public void SaveGame()
    {
        DataPersistenceManager.instance.SaveGame();
    }

    //Used to return to the level hub
    public void ReturnToHub()
    {
        StartCoroutine(LoadLevelHubCo());
    }

    //Return to the main menu
    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadMainMenuCo());
    }

    public IEnumerator SetMenuInactiveCo()
    {
        yield return new WaitForSeconds(.2f);
        menuPanel.SetActive(false);
    }

    IEnumerator LoadMainMenuCo()
    {
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1.1f);
        //Destroy(DataPersistenceManager.instance.gameObject);
        SceneManager.LoadSceneAsync("Main_Menu");
    }

    IEnumerator LoadLevelHubCo()
    {
        yield return new WaitForSeconds(1.1f);
        SceneManager.LoadSceneAsync("Level_Hub");
    }

    public void OptionsButton()
    {
        if (!optionsPanel.activeInHierarchy)
        {
            OpenOptionsMenu();
        }
    }

    #endregion

    #region Options Menu Methods

    public void OpenOptionsMenu()
    {
        optionsPanel.SetActive(true);
        optionsPanel.GetComponentInParent<Animator>().SetBool("isOpening", true);
        optionsPanel.GetComponentInParent<Animator>().SetBool("isClosing", false);
    }

    public void CloseOptionsMenu()
    {
        optionsPanel.GetComponentInParent<Animator>().SetBool("isOpening", false);
        optionsPanel.GetComponentInParent<Animator>().SetBool("isClosing", true);
        StartCoroutine(SetOptionsMenuInactiveCo());
    }

    public IEnumerator SetOptionsMenuInactiveCo()
    {
        yield return new WaitForSeconds(1.2f);
        optionsPanel.SetActive(false);
    }

    #endregion
}

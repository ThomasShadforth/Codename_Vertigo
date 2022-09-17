using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;

    private SaveSlot[] saveSlots;

    private bool isLoading = false;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    [Header("Confirmation Pop-up")]
    [SerializeField] private ConfirmationPopUp confirmationPopUpMenu;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }


    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        //Disable all buttons
        DisableMenuButtons();

        if (isLoading)
        {
            DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileId());
            SaveGameAndLoadScene(saveSlot.lastSavedScene);
        }

        //New game, but data exists
        else if (saveSlot.hasData)
        {
            confirmationPopUpMenu.ActivateMenu("Start a new game with overwrite the currently saved data. Are you sure?",
                () =>
                {
                    //If confirm button is clicked
                    DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileId());
                    DataPersistenceManager.instance.NewGame();
                    SaveGameAndLoadScene();
                },
                () =>
                {
                    //If cancel button is pressed
                    this.ActivateMenu(isLoading);
                }
             );
        }
        //New game, slot is empty
        else
        {
            DataPersistenceManager.instance.ChangeSelectedProfileID(saveSlot.GetProfileId());
            DataPersistenceManager.instance.NewGame();
            SaveGameAndLoadScene();
        }
         

    }

    void SaveGameAndLoadScene(string sceneToLoad = "")
    {
        DataPersistenceManager.instance.SaveGame();

        StartCoroutine(LoadSceneCo(sceneToLoad));
    }

    public void OnClearClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();
        confirmationPopUpMenu.ActivateMenu(
                "Are you sure you want to delete this save data?",
                //On confirm button
                () =>
                {
                    DataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
                    ActivateMenu(isLoading);
                },
                //On cancel button
                () =>
                {
                    ActivateMenu(isLoading);
                }
            );
        
    }
    
    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }


    public void ActivateMenu(bool isLoadingTheGame)
    {
        this.gameObject.SetActive(true);

        this.isLoading = isLoadingTheGame;
        //load all profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        //Ensure back button is enabled
        backButton.interactable = true;

        //Loop through each slot and set content appropriately
        foreach(SaveSlot slot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(slot.GetProfileId(), out profileData);

            slot.SetData(profileData);
            if(profileData == null && isLoadingTheGame)
            {
                slot.SetInteractable(false);
            }
            else
            {
                slot.SetInteractable(true);
            }
        }
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    public void DisableMenuButtons()
    {
        foreach(SaveSlot slot in saveSlots)
        {
            slot.SetInteractable(false);
        }
        backButton.interactable = false;
    }

    IEnumerator LoadSceneCo(string sceneToLoad = "")
    {
        UIFade.instance.FadeToBlack();
        yield return new WaitForSeconds(1f);
        if (sceneToLoad == "")
        {
            //Load the scene
            SceneManager.LoadSceneAsync("Tutorial Pt. 1");
        }
        else
        {
            GameManager.instance.loadingGame = true;
            SceneManager.LoadSceneAsync(sceneToLoad);
        }

        
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public void Continue()
    {
        if (Dialogue_Manager.instance._currTextCount > 0)
        {
            Dialogue_Manager.instance.SetDialogueSpeed();
        }
        else
        {
            Dialogue_Manager.instance.ContinueDialogue();
        }
    }

    public void SelectChoice(int choiceIndex)
    {
        Dialogue_Manager.instance.MakeChoice(choiceIndex);
        Dialogue_Manager.instance.ContinueDialogue();
    }
}

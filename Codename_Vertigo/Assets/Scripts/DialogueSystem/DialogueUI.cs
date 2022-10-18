using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public void Continue()
    {
        Dialogue_Manager.instance.ContinueDialogue();
    }

    public void SelectChoice(int choiceIndex)
    {
        Dialogue_Manager.instance.MakeChoice(choiceIndex);
        Dialogue_Manager.instance.ContinueDialogue();
    }
}

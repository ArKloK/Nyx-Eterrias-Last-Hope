using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBoxActivator : MonoBehaviour
{
    [SerializeField] GameObject dialogueBox;

    // Start is called before the first frame update
    void Start()
    {
        DialogueTrigger.OnTriggerDialogue += TriggerDialogue;
    }

    public void TriggerDialogue(Dialogue dialogue)
    {
        PauseMenuController.canPause = false;
        dialogueBox.SetActive(true);
        DialogueManager.Instance.StartDialogue(dialogue);
    }
}

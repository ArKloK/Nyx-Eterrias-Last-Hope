using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> lines;
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject dialogueBox;

    public void TriggerDialogue()
    {
        dialogueBox.SetActive(true);
        DialogueManager.instance.StartDialogue(dialogue);
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }
}

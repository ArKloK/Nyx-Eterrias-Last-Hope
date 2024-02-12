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
    public Dialogue(List<DialogueLine> lines)
    {
        this.lines = lines;
    }
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject dialogueBox;

    void OnEnable()
    {
        PlayerController.OnPlayerLevelUp += TriggerLevelUpDialogue;
    }

    void OnDisable()
    {
        PlayerController.OnPlayerLevelUp -= TriggerLevelUpDialogue;
    }

    public void TriggerDialogue()
    {
        dialogueBox.SetActive(true);
        DialogueManager.Instance.StartDialogue(dialogue);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void TriggerLevelUpDialogue()
    {
        dialogueBox.SetActive(true);
        DialogueManager.Instance.StartDialogue(new Dialogue(
            new List<DialogueLine>
            {
                new DialogueLine { line = "You leveled up!" }
            }
        ));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }
}

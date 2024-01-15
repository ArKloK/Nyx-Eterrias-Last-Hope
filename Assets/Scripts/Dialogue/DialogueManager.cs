using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public TextMeshProUGUI dialogueText;
    private Queue<DialogueLine> lines;
    public bool isDialogueActive = false;
    public float typingSpeed = 0.2f;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Return))
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        lines = new Queue<DialogueLine>();
        foreach (DialogueLine line in dialogue.lines)
        {
            lines.Enqueue(line);
        }
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }
        DialogueLine line = lines.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(DialogueLine dialogueLine)
    {
        dialogueText.text = "";
        foreach (char c in dialogueLine.line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void EndDialogue()
    {
        this.gameObject.SetActive(false);
        isDialogueActive = false;
        dialogueText.text = "";
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    [SerializeField] GameObject player;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    private Queue<DialogueLine> lines;
    private bool isDialogueActive;
    private bool isSelectingMove;
    [SerializeField] float typingSpeed = 0.2f;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
        if (isDialogueActive)
        {
            //This should avoid the script from modifying the player's movement while in TB
            if (player != null)
            {
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                player.GetComponent<PlayerMovementController.PlayerMovementController>().canMove = false;
            }
            if (isSelectingMove)
            {
                moveSelectionUI.HandleMoveSelection();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                DisplayNextLine();
            }
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

    public void EnqueueDialogue(Dialogue dialogue)
    {
        foreach (DialogueLine line in dialogue.lines)
        {
            lines.Enqueue(line);
        }
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

        //This conditional is to activate the move selection UI when the player levels up and tries to learn a new move
        if (dialogueLine.isMoveSelectionLine)
        {
            moveSelectionUI.gameObject.SetActive(true);
            isSelectingMove = true;
        }
        else
        {
            moveSelectionUI.gameObject.SetActive(false);
            isSelectingMove = false;
        }
    }

    public void EndDialogue()
    {
        //This should avoid the script from modifying the player's movement while in TB
        if (player != null)
        {
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            player.GetComponent<PlayerMovementController.PlayerMovementController>().canMove = true;
        }
        gameObject.SetActive(false);
        moveSelectionUI.gameObject.SetActive(false);
        isDialogueActive = false;
        dialogueText.text = "";
    }
}

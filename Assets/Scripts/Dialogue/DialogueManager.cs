using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static event Action OnStartTBCombat;
    public static DialogueManager Instance;
    [SerializeField] GameObject player;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] ElementSelectionUI elementSelectionUI;
    private Animator animator;
    float typingSpeed;
    private Queue<DialogueLine> lines;
    private bool isDialogueActive;
    private bool isSelectingMove;
    private bool isSelectingElement;
    private bool isTBBattleLine;
    private bool isDialogueLineFinished;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (Instance == null)
            Instance = this;
    }

    void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }
    void Update()
    {
        if (PlayerPrefs.HasKey("TextVelocity"))
        {
            int dbTextVelocity = PlayerPrefs.GetInt("TextVelocity");
            if (dbTextVelocity == 0)
            {
                typingSpeed = 0.1f;
            }
            else if (dbTextVelocity == 1)
            {
                typingSpeed = 0.05f;
            }
            else if (dbTextVelocity == 2)
            {
                typingSpeed = 0.01f;
            }
        }
        else
        {
            typingSpeed = 0.05f;
        }
        if (isDialogueActive)
        {
            //This should avoid the script from modifying the player's movement while in TB
            if (player != null)
            {
                //player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                player.GetComponent<PlayerMovementController.PlayerMovementController>().canMove = false;
                player.GetComponent<PlayerMovementController.PlayerMovementController>().isDialogueActive = true;
                foreach (EnemyMovementController enemy in FindObjectsByType<EnemyMovementController>(FindObjectsSortMode.None))
                {
                    enemy.CanMove = false;
                    enemy.IsDialogueActive = true;
                }

            }
            if (isSelectingMove)
            {
                moveSelectionUI.HandleMoveSelection();
            }
            if (isSelectingElement)
            {
                elementSelectionUI.HandleElementSelection();
            }
            if (Input.GetKeyDown(KeyCode.Return) && isDialogueLineFinished)
            {
                DisplayNextLine();
            }
            if (isTBBattleLine && Input.GetKeyDown(KeyCode.Return) && isDialogueLineFinished)
            {
                isTBBattleLine = false;
                OnStartTBCombat?.Invoke();
            }
        }
    }
    public IEnumerator StartDialogue(Dialogue dialogue)
    {
        dialogueText.text = "";
        isDialogueActive = true;
        lines = new Queue<DialogueLine>();

        foreach (DialogueLine line in dialogue.lines)
        {
            lines.Enqueue(line);
        }

        yield return new WaitForSeconds(GetAnimationDuration("Open"));

        DisplayNextLine();
    }
    public IEnumerator EnqueueDialogueFromInvoke(Dialogue dialogue)
    {
        foreach (DialogueLine line in dialogue.lines)
        {
            lines.Enqueue(line);
        }
        yield return null;
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
            StartCoroutine(EndDialogue());
            return;
        }
        DialogueLine line = lines.Dequeue();

        StopAllCoroutines();
        StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(DialogueLine dialogueLine)
    {
        dialogueText.text = "";
        isDialogueLineFinished = false;
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

        if (dialogueLine.isElementSelectionLine)
        {
            elementSelectionUI.gameObject.SetActive(true);
            isSelectingElement = true;
        }
        else
        {
            elementSelectionUI.gameObject.SetActive(false);
            isSelectingElement = false;
        }

        if (dialogueLine.isTBBattleLine)
        {
            isTBBattleLine = true;
        }
        isDialogueLineFinished = true;
    }
    public IEnumerator EndDialogue()
    {
        //This should avoid the script from modifying the player's movement while in TB
        if (player != null)
        {
            //player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            player.GetComponent<PlayerMovementController.PlayerMovementController>().canMove = true;
            player.GetComponent<PlayerMovementController.PlayerMovementController>().isDialogueActive = false;
            foreach (EnemyMovementController enemy in FindObjectsByType<EnemyMovementController>(FindObjectsSortMode.None))
            {
                enemy.CanMove = false;
            }
        }
        PauseMenuController.canPause = true;
        isDialogueActive = false;
        dialogueText.text = "";
        animator.Play("Close");
        yield return new WaitForSeconds(GetAnimationDuration("Close"));
        gameObject.SetActive(false);
        moveSelectionUI.gameObject.SetActive(false);
        
    }

    float GetAnimationDuration(string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == animationName)
            {
                return ac.animationClips[i].length;
            }
        }

        Debug.LogError("La animación con nombre '" + animationName + "' no fue encontrada.");
        return 0;
    }
}

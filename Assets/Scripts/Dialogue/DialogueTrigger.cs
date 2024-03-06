using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 10)]
    public string line;
    public bool isMoveSelectionLine = false;
    public bool isTBBattleLine = false;
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
    public static event Action<Dialogue> OnTriggerDialogue;
    [SerializeField] Dialogue dialogue;

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
        OnTriggerDialogue?.Invoke(dialogue);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    public void TriggerLevelUpDialogue()
    {
        Dialogue lvlUpDialogue;
        if (PlayerStats.Moves.Count < 4)
        {
            lvlUpDialogue = new Dialogue(
                        new List<DialogueLine>
                            {
                                new DialogueLine { line = $"You leveled up to level {PlayerStats.CurrentLevel}!" },
                                new DialogueLine { line = "Nyx learned a move: " + PlayerStats.GetLearnableMovesAtCurrentLevel().MoveData.MoveName + "!" }
                            }
                    );
        }
        else
        {
            lvlUpDialogue = new Dialogue(
                        new List<DialogueLine>
                            {
                                new DialogueLine { line = $"You leveled up to level {PlayerStats.CurrentLevel}!" },
                                new DialogueLine { line = "Nyx wants to learn " + PlayerStats.GetLearnableMovesAtCurrentLevel().MoveData.MoveName},
                                new DialogueLine { line = "But Nyx cannot learn more than 4 moves!"},
                                new DialogueLine{line = "Choose a move to forget", isMoveSelectionLine = true}
                            }
                    );
        }
        OnTriggerDialogue?.Invoke(lvlUpDialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }
}

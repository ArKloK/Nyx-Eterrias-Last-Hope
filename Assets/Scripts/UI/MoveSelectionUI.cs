using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] Color highlightedColor;
    int currentSelection = 0;
    void OnEnable()
    {
        SetMoves();
    }

    public void SetMoves()
    {
        for (int i = 0; i < PlayerStats.Moves.Count; i++)
        {
            moveTexts[i].text = PlayerStats.Moves[i].MoveData.MoveName;
        }
        moveTexts[PlayerStats.Moves.Count].text = PlayerStats.GetLearnableMovesAtCurrentLevel().Move.MoveName;
    }

    public void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++currentSelection;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --currentSelection;
        }

        currentSelection = Mathf.Clamp(currentSelection, 0, 4);
        UpdateMoveSelection(currentSelection);
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Dialogue learningMoveDialogue;
            //If the player wants to forget the new move
            if (currentSelection == 4)
            {
                learningMoveDialogue = new Dialogue(
                    new List<DialogueLine>
                    {
                        new DialogueLine { line = "Nyx is not learning " + moveTexts[currentSelection].text + "!" }
                    }
                );
            }
            //If the player wants to forget an old move
            else
            {
                learningMoveDialogue = new Dialogue(
                    new List<DialogueLine>
                    {
                        new DialogueLine { line = "Nyx forgot " + PlayerStats.Moves[currentSelection].MoveData.MoveName + "!" },
                        new DialogueLine { line = "And Nyx learned " + moveTexts[4].text + "!" }
                    }
                );
                PlayerStats.Moves[currentSelection] = new TBMove(PlayerStats.GetLearnableMovesAtCurrentLevel().Move);
            }
            DialogueManager.Instance.EnqueueDialogue(learningMoveDialogue);
        }
    }

    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == selection)
            {
                moveTexts[i].color = highlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
    }
}

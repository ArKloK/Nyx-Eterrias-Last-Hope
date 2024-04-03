using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TBDialogueBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] float typingSpeed;
    [SerializeField] Color highlightedColor;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<TextMeshProUGUI> actionTexts;
    [SerializeField] List<TextMeshProUGUI> moveTexts;

    [SerializeField] TextMeshProUGUI powerText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI accuracyText;
    private bool isDialogueLineFinished;

    public bool IsDialogueLineFinished { get => isDialogueLineFinished; set => isDialogueLineFinished = value; }

    public IEnumerator TypeDialogueTB(string dialogue)
    {
        isDialogueLineFinished = false;
        dialogueText.text = "";
        foreach (char c in dialogue.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(0.8f);
        isDialogueLineFinished = true;
    }

    public void EnableDialogueText(bool enabled)
    {
        dialogueText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, TBMove move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = highlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
        powerText.text = "Power: " + move.MoveData.Power;
        typeText.text = "Type: " + move.MoveData.Element;
        accuracyText.text = "Accuracy: " + move.MoveData.Accuracy;
    }

    public void SetMoveNames(List<TBMove> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].MoveData.MoveName;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }

    }
}

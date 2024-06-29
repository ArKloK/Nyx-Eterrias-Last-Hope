using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElementSelectionUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> elementTexts;
    [SerializeField] Color highlightedColor;
    private bool isVerticalInputPressed = false;
    int currentSelection = 0;
    void OnEnable()
    {
        SetElements();
    }

    public void SetElements()
    {
        elementTexts[0].text = Element.Fire.ToString().ToUpper();
        elementTexts[1].text = Element.Water.ToString().ToUpper();
        elementTexts[2].text = Element.Grass.ToString().ToUpper();
    }

    public void HandleElementSelection()
    {
        // if (Input.GetKeyDown(KeyCode.DownArrow))
        if (Input.GetAxis("Vertical") < 0 && !isVerticalInputPressed)
        {
            ++currentSelection;
            isVerticalInputPressed = true;
        }
        //else if (Input.GetKeyDown(KeyCode.UpArrow))
        else if (Input.GetAxis("Vertical") > 0 && !isVerticalInputPressed)
        {
            --currentSelection;
            isVerticalInputPressed = true;
        }
        else if (Input.GetAxis("Vertical") == 0)
        {
            isVerticalInputPressed = false;
        }

        // currentSelection = Mathf.Clamp(currentSelection, 0, 2);
        if (currentSelection < 0)
        {
            currentSelection = 2;
        }
        else if (currentSelection > 2)
        {
            currentSelection = 0;
        }
        UpdateElementSelection(currentSelection);
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Dialogue selectingElementDIalogue;
            selectingElementDIalogue = new Dialogue(
                new List<DialogueLine>
                {
                        new DialogueLine { line = "Nyx is now " + elementTexts[currentSelection].text + " type!" }
                }
            );
            //Assign the element to the player
            PlayerStats.Element = (Element)currentSelection + 1;
            UpdateLearnableMoves();
            DialogueManager.Instance.EnqueueDialogue(selectingElementDIalogue);
        }
    }

    public void UpdateElementSelection(int selection)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == selection)
            {
                elementTexts[i].color = highlightedColor;
            }
            else
            {
                elementTexts[i].color = Color.black;
            }
        }
    }

    void UpdateLearnableMoves()
    {
        List<LearnableMove> learnableMoves = new List<LearnableMove>();
        if (PlayerStats.Element == Element.Fire)
        {
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Heat Hit"));
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Intimidate"));
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Tailwind"));
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Burn"));
        }
        else if (PlayerStats.Element == Element.Water)
        {
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Aqua Shoot"));
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Robust Defense"));
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Slow Tempo"));
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Soak"));
        }else if (PlayerStats.Element == Element.Grass)
        {
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Roots Power"));
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Robust Defense"));
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Tailwind"));
            learnableMoves.Add(PlayerStats.LearnableMoves.Find(x => x.MoveData.MoveName == "Poison"));
        }
        PlayerStats.LearnableMoves = learnableMoves;
    }
}

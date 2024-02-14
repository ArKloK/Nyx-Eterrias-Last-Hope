using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> moveTexts;
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
}

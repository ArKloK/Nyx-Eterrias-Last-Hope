using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BattleState { START, PLAYERACTION, PLAYERMOVE, ENEMYMOVE, BUSY }

public class BattleSystem : MonoBehaviour
{
    public EnemyUnit enemyUnit;
    public PlayerUnit playerUnit;
    public BattleHud enemyHud;
    public BattleHud playerHud;
    public TBDialogueBox dialogueBox;
    BattleState state;
    private int currentAction;
    private int currentMove;

    void Start()
    {
        StartCoroutine(SetUpBattle());
    }

    void Update()
    {
        if (state == BattleState.PLAYERACTION)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PLAYERMOVE)
        {
            HandleMoveSelection();
        }
    }
    public IEnumerator SetUpBattle()
    {
        playerUnit.setData();
        playerHud.setData(playerUnit.player);
        enemyUnit.setData();
        enemyHud.setData(enemyUnit.enemy);

        dialogueBox.SetMoveNames(playerUnit.player.moves);

        yield return dialogueBox.TypeDialogueTB("A wild " + enemyUnit.enemyData.EnemyName + " appeared!");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PLAYERACTION;
        StartCoroutine(dialogueBox.TypeDialogueTB("Choose an action."));
        dialogueBox.EnableActionSelector(true);
    }
    void PlayerMove()
    {
        state = BattleState.PLAYERMOVE;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }
    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
            {
                currentAction++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                currentAction--;
            }
        }

        dialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentAction == 0)
            {
                //Fight action
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                //Inventory action
            }
        }
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.player.moves.Count - 1)
            {
                currentMove++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
            {
                currentMove--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.player.moves.Count - 2)
            {
                currentMove += 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
            {
                currentMove -= 2;
            }
        }

        dialogueBox.UpdateMoveSelection(currentMove, playerUnit.player.moves[currentMove]);

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BattleState { START, PLAYERACTION, PLAYERMOVE, ENEMYMOVE, BUSY, BATTLEOVER }

public class BattleSystem : MonoBehaviour
{
    public EnemyUnit enemyUnit;
    public PlayerUnit playerUnit;
    public BattleHud enemyHud;
    public BattleHud playerHud;
    public TBDialogueBox dialogueBox;
    BattleState state;
    //This boolean is going to be true if the player wins the battle, and false if the player loses the battle.
    public event Action<bool> OnBattleEnd;
    private int currentAction;
    private int currentMove;

    void OnEnable()
    {
        StartCoroutine(SetUpBattle());
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PLAYERACTION)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PLAYERMOVE)
        {
            if (playerUnit.player.moves.Count > 0)
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

        yield return dialogueBox.TypeDialogueTB("A wild " + enemyUnit.enemyData.Name + " appeared!");

        PlayerAction();
        //ChooseFirstTurn();
    }

    void ChooseFirstTurn()
    {
        PlayerAction();
        if (playerUnit.player.Speed >= enemyUnit.enemy.Speed)
        {

        }
        else
        {

            StartCoroutine(EnemyMoveTurn());
        }
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
    IEnumerator PlayerMoveTurn()
    {
        state = BattleState.BUSY;
        var move = playerUnit.player.moves[currentMove];
        yield return dialogueBox.TypeDialogueTB(playerUnit.player.playerData.Name + " used " + move.MoveData.MoveName + "!");

        if (move.MoveData.Category == MoveCategory.Status)
        {
            yield return RunPlayerMoveEffects(move, playerUnit.player, enemyUnit.enemy);
        }
        else
        {
            var damageDetails = enemyUnit.enemy.TakeDamage(move, playerUnit.player);
            yield return enemyHud.UpdateEnemyHp();
            yield return ShowDamageDetails(damageDetails);
        }

        if (enemyUnit.enemy.currentHp <= 0)
        {
            yield return dialogueBox.TypeDialogueTB(enemyUnit.enemyData.Name + " fainted!");
            yield return new WaitForSeconds(2f);
            EndBattle(true);
        }
        else
        {
            StartCoroutine(EnemyMoveTurn());
        }
    }



    IEnumerator EnemyMoveTurn()
    {
        state = BattleState.ENEMYMOVE;
        var move = enemyUnit.enemy.GetRandomMove();
        yield return dialogueBox.TypeDialogueTB(enemyUnit.enemy.enemyData.Name + " used " + move.MoveData.MoveName + "!");

        if (move.MoveData.Category == MoveCategory.Status)
        {
            yield return RunEnemyMoveEffects(move, enemyUnit.enemy, playerUnit.player);
        }
        else
        {
            var damageDetails = playerUnit.player.TakeDamage(move, enemyUnit.enemy);
            yield return playerHud.UPdatePlayerHp();
            yield return ShowDamageDetails(damageDetails);
        }

        if (playerUnit.player.currentHp <= 0)
        {
            yield return dialogueBox.TypeDialogueTB(playerUnit.playerData.Name + " fainted!");
            yield return new WaitForSeconds(2f);
            EndBattle(false);
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator RunPlayerMoveEffects(TBMove move, TBPlayer player, TBEnemy enemy)
    {
        var effects = move.MoveData.Effects;
        if (effects.statBoosts != null)
        {
            if (move.MoveData.Target == MoveTarget.Self)
            {
                player.ApplyBoosts(effects.statBoosts);
            }
            else
            {
                enemy.ApplyBoosts(effects.statBoosts);
            }
        }

        yield return ShowPlayerStatusChanges(player);
        yield return ShowEnemyStatusChanges(enemy);
    }

    IEnumerator RunEnemyMoveEffects(TBMove move, TBEnemy enemy, TBPlayer player)
    {
        var effects = move.MoveData.Effects;
        if (effects.statBoosts != null)
        {
            if (move.MoveData.Target == MoveTarget.Self)
            {
                enemy.ApplyBoosts(effects.statBoosts);
            }
            else
            {
                player.ApplyBoosts(effects.statBoosts);
            }
        }

        yield return ShowEnemyStatusChanges(enemy);
        yield return ShowPlayerStatusChanges(player);
    }

    IEnumerator ShowPlayerStatusChanges(TBPlayer player)
    {
        while (player.statusChanges.Count > 0)
        {
            var message = player.statusChanges.Dequeue();
            yield return dialogueBox.TypeDialogueTB(message);
        }
    }

    IEnumerator ShowEnemyStatusChanges(TBEnemy enemy)
    {
        while (enemy.statusChanges.Count > 0)
        {
            var message = enemy.statusChanges.Dequeue();
            yield return dialogueBox.TypeDialogueTB(message);
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogueBox.TypeDialogueTB("A critical hit!");
        }
        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogueBox.TypeDialogueTB("It's super effective!");
        }
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogueBox.TypeDialogueTB("It's not very effective!");
        }
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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(PlayerMoveTurn());
        }
    }

    public void EndBattle(bool won)
    {
        state = BattleState.BATTLEOVER;
        playerUnit.player.ResetStatBoosts();
        enemyUnit.enemy.ResetStatBoosts();
        OnBattleEnd?.Invoke(won);
    }
}

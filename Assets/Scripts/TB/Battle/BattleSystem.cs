using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { PLAYERACTION, PLAYERMOVE, RUNNINGTURN, BATTLEOVER, INVENTORY }
public enum BattleAction { FIGHT, INVENTORY }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] TBCharacterUnit playerUnit;
    [SerializeField] TBCharacterUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] TBDialogueBox dialogueBox;
    [SerializeField] GameObject inventoryUI;
    [SerializeField] Inventory inventory;
    BattleState state;
    public event Action<bool> OnBattleEnd;
    int currentAction;
    int currentMove;

    void OnEnable()
    {
        StartCoroutine(SetupBattle());
        PlayerController.OnPlayerLevelUp += HandleLevelUp;
        InventoryManager.OnTBItemUsedUpdateHP += UpdatePlayerHP;
    }

    void OnDisable()
    {
        PlayerController.OnPlayerLevelUp -= HandleLevelUp;
        InventoryManager.OnTBItemUsedUpdateHP -= UpdatePlayerHP;
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PLAYERACTION)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PLAYERMOVE)
        {
            if (playerUnit.Character.Moves.Count > 0)
                HandleMoveSelection();
        }
        else if (state == BattleState.INVENTORY)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                inventoryUI.SetActive(false);
                PlayerAction();
            }
        }
    }

    void UpdatePlayerHP(ItemData item)
    {
        Debug.Log("Updating player HP");
        StartCoroutine(OnInventoryItemUsed(item));
    }

    IEnumerator OnInventoryItemUsed(ItemData item)
    {
        inventoryUI.SetActive(false);
        dialogueBox.EnableActionSelector(false);
        yield return dialogueBox.TypeDialogueTB("Nyx used " + item.itemName + "!");
        yield return playerHud.UpdateHp();
        yield return new WaitForSeconds(2f);
        yield return RunTurns(BattleAction.INVENTORY);
    }

    IEnumerator SetupBattle()
    {
        playerUnit.setData();
        playerHud.SetData(playerUnit.Character);
        enemyUnit.setData();
        enemyHud.SetData(enemyUnit.Character);

        dialogueBox.SetMoveNames(playerUnit.Character.Moves);

        yield return dialogueBox.TypeDialogueTB($"{enemyUnit.Character.CharacterData.Name} wants to battle!");

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

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RUNNINGTURN;
        if (playerAction == BattleAction.FIGHT)
        {
            //Check who goes first
            bool playerGoesFirst = playerUnit.Character.Speed >= enemyUnit.Character.Speed;
            var firstUnitToMove = playerGoesFirst ? playerUnit : enemyUnit;
            var secondUnitToMove = playerGoesFirst ? enemyUnit : playerUnit;

            yield return CharacterMoveTurn(firstUnitToMove, secondUnitToMove);
            //This will check if the battle is over before the second character moves
            if (state == BattleState.BATTLEOVER)
            {
                yield return RunAfterTurn(secondUnitToMove);
                yield break;
            }

            yield return CharacterMoveTurn(secondUnitToMove, firstUnitToMove);

            yield return RunAfterTurn(enemyUnit);
            yield return RunAfterTurn(playerUnit);
        }
        else if (playerAction == BattleAction.INVENTORY)
        {
            //Inventory action
            yield return CharacterMoveTurn(enemyUnit, playerUnit);
            if (state == BattleState.BATTLEOVER)
            {
                yield return RunAfterTurn(playerUnit);
                yield break;
            }
            yield return RunAfterTurn(enemyUnit);
            yield return RunAfterTurn(playerUnit);
        }

        if (state != BattleState.BATTLEOVER)
        {
            PlayerAction();
        }
    }

    IEnumerator CharacterMoveTurn(TBCharacterUnit source, TBCharacterUnit target)
    {
        source.Character.OnBeforeMove();
        state = BattleState.RUNNINGTURN;
        TBMove move;
        if (source.CharacterData.IsEnemy)
        {
            move = source.Character.GetRandomMove();
        }
        else
        {
            move = source.Character.Moves[currentMove];
        }
        yield return dialogueBox.TypeDialogueTB(source.Character.CharacterData.Name + " used " + move.MoveData.MoveName + "!");

        if (CheckIfMoveHits(move))
        {
            if (move.MoveData.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move, source.Character, target.Character);
            }
            else
            {
                var damageDetails = target.Character.TakeDamage(move, source.Character);
                if (target.CharacterData.IsEnemy)
                {
                    yield return enemyHud.UpdateHp();
                }
                else
                {
                    yield return playerHud.UpdateHp();
                }
                yield return ShowDamageDetails(damageDetails);
            }

            if (target.Character.CurrentHP <= 0)
            {
                yield return dialogueBox.TypeDialogueTB(target.CharacterData.Name + " fainted!");
                yield return new WaitForSeconds(2f);
                if (target.CharacterData.IsEnemy)
                {
                    EndBattle(true);
                }
                else
                {
                    EndBattle(false);
                }
            }
        }
        else
        {
            Debug.Log("Player's attack missed!");
            yield return dialogueBox.TypeDialogueTB(source.Character.CharacterData.Name + "'s attack missed!");
        }
    }

    IEnumerator RunAfterTurn(TBCharacterUnit unit)
    {
        //This will check if the battle is over before running the after turn effects
        if (state == BattleState.BATTLEOVER)
        {
            if (unit.Character.CurrentHP <= 0)
            {
                yield return dialogueBox.TypeDialogueTB(unit.CharacterData.Name + " fainted!");
                yield return new WaitForSeconds(2f);
                EndBattle(unit.CharacterData.IsEnemy);
                //This will break the coroutine if the battle is over
                yield break;
            }
        }

        //Statuses like poison will affect the unit after the turn
        unit.Character.OnAfterTurn();
        yield return ShowStatusChanges(unit.Character);
        yield return (unit.CharacterData.IsEnemy ? enemyHud : playerHud).UpdateHp();
        if (unit.Character.CurrentHP <= 0)
        {
            yield return dialogueBox.TypeDialogueTB(unit.CharacterData.Name + " fainted!");
            yield return new WaitForSeconds(2f);
            EndBattle(unit.CharacterData.IsEnemy);
        }
    }

    // IEnumerator RunAfterTurn()
    // {
    //     //This will check if the battle is over before running the after turn effects
    //     if (state == BattleState.BATTLEOVER)
    //     {
    //         if (playerUnit.Character.CurrentHP <= 0)
    //         {
    //             yield return dialogueBox.TypeDialogueTB(playerUnit.CharacterData.Name + " fainted!");
    //             yield return new WaitForSeconds(2f);
    //             EndBattle(false);
    //         }
    //         else if (enemyUnit.Character.CurrentHP <= 0)
    //         {
    //             yield return dialogueBox.TypeDialogueTB(enemyUnit.CharacterData.Name + " fainted!");
    //             yield return new WaitForSeconds(2f);
    //             EndBattle(true);
    //         }
    //         //This will break the coroutine if the battle is over
    //         yield break;
    //     }
    //     //Statuses like poison will affect the enemy after the turn
    //     enemyUnit.Character.OnAfterTurn();
    //     yield return ShowStatusChanges(enemyUnit.Character);
    //     yield return enemyHud.UpdateHp();
    //     if (enemyUnit.Character.CurrentHP <= 0)
    //     {
    //         yield return dialogueBox.TypeDialogueTB(enemyUnit.CharacterData.Name + " fainted!");
    //         yield return new WaitForSeconds(2f);
    //         EndBattle(true);
    //     }
    //     else
    //     {
    //         playerUnit.Character.OnAfterTurn();
    //         yield return ShowStatusChanges(playerUnit.Character);
    //         yield return playerHud.UpdateHp();
    //         if (playerUnit.Character.CurrentHP <= 0)
    //         {
    //             yield return dialogueBox.TypeDialogueTB(playerUnit.CharacterData.Name + " fainted!");
    //             yield return new WaitForSeconds(2f);
    //             EndBattle(false);
    //         }
    //     }
    // }
    IEnumerator RunMoveEffects(TBMove move, TBCharacter source, TBCharacter target)
    {
        var effects = move.MoveData.Effects;
        if (effects.statBoosts != null)
        {
            if (move.MoveData.Target == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.statBoosts);
            }
            else
            {
                target.ApplyBoosts(effects.statBoosts);
            }
        }

        //Status Condition
        if (effects.status != ConditionID.None)
        {
            target.AddStatus(effects.status);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    IEnumerator ShowStatusChanges(TBCharacter character)
    {
        while (character.StatusChanges.Count > 0)
        {
            var message = character.StatusChanges.Dequeue();
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
                OpenInventory();
            }
        }
    }
    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Character.Moves.Count - 1)
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
            if (currentMove < playerUnit.Character.Moves.Count - 2)
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

        dialogueBox.UpdateMoveSelection(currentMove, playerUnit.Character.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(RunTurns(BattleAction.FIGHT));
        }
    }

    //The parameter won is going to be true if the player wins the battle, and false if the player loses the battle.
    public void EndBattle(bool won)
    {
        PlayerStats.CurrentHealthPoints = playerUnit.Character.CurrentHP;
        //This will add experience to the player if he wins the battle
        if (won)
        {
            ExperienceManager.Instance.AddExperience(enemyUnit.CharacterData.ExperienceAmount);
        }
        state = BattleState.BATTLEOVER;
        playerUnit.Character.ResetStatBoosts();
        enemyUnit.Character.ResetStatBoosts();
        OnBattleEnd?.Invoke(won);
    }

    public void HandleLevelUp()
    {
        playerUnit.Character.LearnMove(PlayerStats.GetLearnableMovesAtCurrentLevel());
    }

    bool CheckIfMoveHits(TBMove move)
    {
        if (move.MoveData.Accuracy == 100)
        {
            return true;
        }
        return UnityEngine.Random.Range(1, 101) <= move.Accuracy;
    }

    void OpenInventory()
    {
        state = BattleState.INVENTORY;
        inventoryUI.SetActive(true);
        inventory.LaunchInventoryChange();
    }
}

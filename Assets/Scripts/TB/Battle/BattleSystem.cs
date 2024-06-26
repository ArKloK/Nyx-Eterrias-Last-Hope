using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

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
    [SerializeField] SelectMovementAgent selectMovementAgent;
    [SerializeField] GameObject playerActionFirst;
    [SerializeField] GameObject playerMoveFirst;
    [SerializeField] List<Collectible> collectibles;
    [SerializeField] bool TBDemo;
    [SerializeField] bool playerControlledByAI;
    BattleState state;
    HumanModelAI humanModelAI;
    Animator animator;
    public static event Action<bool> OnBattleEnd;
    int currentAction;
    int currentMove;
    int currentEnemyMove;
    string humanModelAction = "";
    private string humanModelActionFirstPart;
    private string humanModelActionSecondPart;
    private bool humanModelAIActionSelectioned;
    private bool enemyActionSelected;

    public static event Action<InventoryItem> OnHumanModelHeals;

    void Awake()
    {
        if (!TBDemo)
            animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        PlayerController.OnPlayerLevelUp += HandleLevelUp;
        InventoryManager.OnTBItemUsedUpdateHP += UpdatePlayerHP;
        SelectMovementAgent.OnEpisodeBeginAction += RestartBattle;
    }

    void OnDisable()
    {
        PlayerController.OnPlayerLevelUp -= HandleLevelUp;
        InventoryManager.OnTBItemUsedUpdateHP -= UpdatePlayerHP;
        SelectMovementAgent.OnEpisodeBeginAction -= RestartBattle;
    }
    void Start()
    {
        dialogueBox.PlayerControlledByAI1 = playerControlledByAI;
        if (playerControlledByAI)
        {
            humanModelAI = GetComponent<HumanModelAI>();
        }
        // else
        //     StartCoroutine(SetupBattle());
    }
    private void RestartBattle()
    {
        StartCoroutine(SetupBattle());
    }
    public void HandleUpdate()
    {
        PlayerStats.MaxHealthPoints = playerUnit.Character.MaxHp;
        Debug.Log("Player max health points: " + PlayerStats.MaxHealthPoints);
        if (state == BattleState.PLAYERACTION)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PLAYERMOVE)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !playerControlledByAI)
            {
                dialogueBox.EnableMoveSelector(false);
                PlayerAction();
            }
            if (playerUnit.Character.Moves.Count > 0)
                HandleMoveSelection();
        }
        else if (state == BattleState.INVENTORY)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !playerControlledByAI)
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
        yield return dialogueBox.TypeDialogueTB("Nyx used " + item.ItemName + "!");
        if (item.ConditionID != ConditionID.None)
        {
            playerUnit.Character.RemoveStatus(item.ConditionID);
            yield return dialogueBox.TypeDialogueTB("Nyx has been cured from " + item.ConditionID + "!");
        }
        else
        {
            yield return playerHud.UpdateHp();
        }
        yield return RunTurns(BattleAction.INVENTORY);
    }
    public IEnumerator SetupBattle()
    {
        if (TBDemo)
        {
            PlayerStats.CurrentHealthPoints = PlayerStats.MaxHealthPoints;
            List<ItemData> items = new List<ItemData>();
            foreach (Collectible collectible in collectibles)
            {
                items.Add(collectible.ItemData);
            }
            inventory.SetInventory(items);
        }

        playerUnit.setData(TBDemo);
        playerHud.SetData(playerUnit.Character);
        enemyUnit.setData(TBDemo);
        enemyHud.SetData(enemyUnit.Character);

        if (playerControlledByAI) humanModelAI.ResetBattle();

        dialogueBox.SetMoveNames(playerUnit.Character.Moves);

        if (!TBDemo)
            yield return new WaitForSeconds(GetAnimationDuration("StartCombat"));

        yield return dialogueBox.TypeDialogueTB($"{enemyUnit.Character.CharacterData.Name} wants to battle!");

        PlayerAction();
    }
    void PlayerAction()
    {
        state = BattleState.PLAYERACTION;
        StartCoroutine(dialogueBox.TypeDialogueTB("Choose an action."));
        dialogueBox.EnableMoveSelector(false);
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
        enemyActionSelected = false;
        if (playerAction == BattleAction.FIGHT)
        {
            //Check who goes first
            bool playerGoesFirst = playerUnit.Character.Speed >= enemyUnit.Character.Speed;
            var firstUnitToMove = playerGoesFirst ? playerUnit : enemyUnit;
            var secondUnitToMove = playerGoesFirst ? enemyUnit : playerUnit;

            if (!playerGoesFirst)
            {
                if (!enemyActionSelected)
                {
                    currentEnemyMove = selectMovementAgent.GetMoveIndex();
                    enemyActionSelected = true;
                    Debug.Log("Current enemy move: " + currentEnemyMove);
                }
            }

            yield return CharacterMoveTurn(firstUnitToMove, secondUnitToMove);
            //This will check if the battle is over before the second character moves
            if (state == BattleState.BATTLEOVER)
            {
                yield return RunAfterTurn(secondUnitToMove);
                yield break;
            }
            if (playerGoesFirst)
            {
                if (!enemyActionSelected)
                {
                    currentEnemyMove = selectMovementAgent.GetMoveIndex();
                    enemyActionSelected = true;
                    Debug.Log("Current enemy move: " + currentEnemyMove);
                }
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
            move = source.Character.Moves[currentEnemyMove];
        }
        else
        {
            move = source.Character.Moves[currentMove];
        }
        yield return dialogueBox.TypeDialogueTB(source.Character.CharacterData.Name + " used " + move.MoveData.MoveName + "!");

        if (CheckIfMoveHits(move))
        {
            if (move.MoveData.Category == MoveCategory.Stats)
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
                    StartCoroutine(EndBattle(true));
                }
                else
                {
                    StartCoroutine(EndBattle(false));
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
                StartCoroutine(EndBattle(unit.CharacterData.IsEnemy));
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
            StartCoroutine(EndBattle(unit.CharacterData.IsEnemy));
        }
        humanModelAIActionSelectioned = false;
    }
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
            if (source.CharacterData.IsEnemy)
            {
                Debug.Log("Enemy used " + move.MoveData.MoveName + " and the value of the enemy stat is " + source.Defense);
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
        if (!playerControlledByAI)
        {
            if (Input.GetAxis("Vertical") < 0)
            //if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentAction < 1)
                {
                    currentAction++;
                }
            }
            else if (Input.GetAxis("Vertical") > 0)
            //else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentAction > 0)
                {
                    currentAction--;
                }
            }

            dialogueBox.UpdateActionSelection(currentAction);

            if (Input.GetKeyDown(KeyCode.Return) && dialogueBox.IsDialogueLineFinished)
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
        else
        {
            if (!humanModelAIActionSelectioned) HandleActionSelectionAI();

            if (dialogueBox.IsDialogueLineFinished)
            {
                if (humanModelActionFirstPart.Equals("Attack"))
                {
                    PlayerMove();
                }
                else if (humanModelActionFirstPart.Equals("Healh"))
                {
                    OpenInventory();
                    InventoryItem item = inventory.inventoryItems.Find(item => item.ItemData.ItemName == humanModelActionSecondPart);
                    OnHumanModelHeals?.Invoke(item);
                    CloseInventory();
                }
            }

        }

    }
    public void HandleActionSelectionAI()
    {
        do
        {
            humanModelAction = humanModelAI.GetAction();
            if (humanModelAction == "")
            {
                Debug.Log("Human model action is empty");
            }
        } while (humanModelAction == "");

        humanModelAI.CanCheckIsConditioned = true;
        humanModelAIActionSelectioned = true;
        string[] actionParts = humanModelAction.Split(' ');
        humanModelActionFirstPart = actionParts[0];
        humanModelActionSecondPart = string.Join(" ", actionParts.Skip(1));
    }
    private void HandleMoveSelection()
    {
        if (!playerControlledByAI)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (currentMove < playerUnit.Character.Moves.Count - 1)
                {
                    currentMove++;
                }
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                if (currentMove > 0)
                {
                    currentMove--;
                }
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                if (currentMove < playerUnit.Character.Moves.Count - 2)
                {
                    currentMove += 2;
                }
            }
            else if (Input.GetAxis("Vertical") > 0)
            {
                if (currentMove > 1)
                {
                    currentMove -= 2;
                }
            }

            dialogueBox.UpdateMoveSelection(currentMove, playerUnit.Character.Moves[currentMove]);

            if (Input.GetKeyDown(KeyCode.Return) && dialogueBox.IsDialogueLineFinished)
            {
                dialogueBox.EnableMoveSelector(false);
                dialogueBox.EnableDialogueText(true);
                StartCoroutine(RunTurns(BattleAction.FIGHT));
            }
        }
        else
        {
            currentMove = playerUnit.Character.Moves.FindIndex(move => move.MoveData.MoveName == humanModelActionSecondPart);
            dialogueBox.UpdateMoveSelection(currentMove, playerUnit.Character.Moves[currentMove]);

            if (dialogueBox.IsDialogueLineFinished)
            {
                dialogueBox.EnableMoveSelector(false);
                dialogueBox.EnableDialogueText(true);
                StartCoroutine(RunTurns(BattleAction.FIGHT));
            }
        }
    }

    public void ButtonPressed()
    {
        Debug.Log("Button pressed");
    }

    //The parameter won is going to be true if the player wins the battle, and false if the player loses the battle.
    public IEnumerator EndBattle(bool playerWon)
    {
        PlayerStats.CurrentHealthPoints = playerUnit.Character.CurrentHP;
        dialogueBox.ClearDialogueText();
        //This will add experience to the player if he wins the battle

        if (playerWon && !TBDemo)
        {
            animator.Play("EnemyFaint");
            yield return new WaitForSeconds(GetAnimationDuration("EnemyFaint"));
            ExperienceManager.Instance.AddExperience(enemyUnit.CharacterData.ExperienceAmount);
        }
        else if (!playerWon && !TBDemo)
        {
            animator.Play("PlayerFaint");
            yield return new WaitForSeconds(GetAnimationDuration("PlayerFaint"));
            RespawnController.Instance.SetPlayerStats();
        }

        StopAllCoroutines();
        state = BattleState.BATTLEOVER;

        playerUnit.Character.ResetStatBoosts();
        enemyUnit.Character.ResetStatBoosts();
        OnBattleEnd?.Invoke(playerWon);
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
    void CloseInventory()
    {
        inventoryUI.SetActive(false);
    }
    public void ClosePlayerMove(InputAction.CallbackContext context)
    {
        if (context.performed && state == BattleState.PLAYERMOVE)
        {
            Debug.Log("Close player move");
            PlayerAction();
        }
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

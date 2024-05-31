using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HumanModelAI : MonoBehaviour
{
    [SerializeField] TBCharacterUnit playerUnit;
    [SerializeField] TBCharacterUnit enemyUnit;
    [SerializeField] Inventory inventory;

    private List<TBMove> playerMoves;
    private List<TBMove> enemyMoves;
    private float playerCritical;
    private TBMove playerMostDamagingMove;
    private bool canCheckIsConditioned = true;

    public float PlayerCritical { get => playerCritical; set => playerCritical = value; }
    public TBMove PlayerMostDamagingMove { get => playerMostDamagingMove; set => playerMostDamagingMove = value; }
    public bool CanCheckIsConditioned { get => canCheckIsConditioned; set => canCheckIsConditioned = value; }

    public void ResetBattle()
    {
        playerMoves = playerUnit.Character.Moves;
        enemyMoves = enemyUnit.Character.Moves;
    }

    public string GetAction()
    {
        bool playerCanFinishEnemy = PlayerCanFinishEnemy();
        //If the enemy can finish the player without critical, then think
        if (EnemyCanFinishPlayerWithoutCritical())
        {
            Debug.Log("Inside EnemyCanFinishPlayerWithoutCritical");
            if (playerCanFinishEnemy)
            {
                //If the player can finish the enemy, and is faster than the enemy, then attack 
                if (playerUnit.Character.Speed >= enemyUnit.Character.Speed)
                {
                    return "Attack " + playerMostDamagingMove.MoveData.MoveName;
                }
                // If the player can finish the enemy, and is slower than the enemy, then use a health potion if it exists. Otherwise, attack
                else
                {
                    if (HasHealPotion())
                    {
                        return "Healh Potion";
                    }
                    else
                    {
                        return "Attack " + playerMostDamagingMove.MoveData.MoveName;
                    }
                }
            }
            else
            {
                if (HasHealPotion())
                {
                    return "Healh Potion";
                }
                else
                {
                    return "Attack " + playerMostDamagingMove.MoveData.MoveName;
                }
            }
        }
        //If the enemy can finish the player with critical, tends to ignore the menace but it has a low probability (15%) to be "scared" of losing the battle
        //and tries to use a health potion
        if (EnemyCanFinishPlayerWithCritical())
        {
            Debug.Log("Inside EnemyCanFinishPlayerWithCritical");
            Random.InitState(System.DateTime.Now.Millisecond);
            if (Random.Range(1, 21) > 17)// 18-20
            {
                if (HasHealPotion())
                {
                    return "Healh Potion";
                }
                else
                {
                    return "Attack " + playerMostDamagingMove.MoveData.MoveName;
                }
            }
        }
        //If the player can finish the enemy, then attack
        if (playerCanFinishEnemy)
        {
            Debug.Log("Inside PlayerCanFinishEnemy");
            return "Attack " + playerMostDamagingMove.MoveData.MoveName;
        }
        // If the player is conditioned, then use the appropriate potion if it exists
        if (PlayerIsConditioned() && canCheckIsConditioned)
        {
            Debug.Log("Inside PlayerIsConditioned");
            if (PlayerCondition() == ConditionID.Burning)
            {
                Debug.Log("Inside player burning");
                if (HasHealBurnPotion())
                {
                    Debug.Log("Inside player burning has heal burn potion");
                    return "Healh Burn Potion";
                }
                else
                {
                    canCheckIsConditioned = false;
                    return "";
                }
            }
            if (PlayerCondition() == ConditionID.Poisoned)
            {
                Debug.Log("Inside player poisoned");
                if (HasHealPoisonPotion())
                {
                    Debug.Log("Inside player poisoned has heal poison potion");
                    return "Healh Poison Potion";
                }
                else
                {
                    canCheckIsConditioned = false;
                    return "";
                }
            }
            if (PlayerCondition() == ConditionID.Soaked)
            {
                if (HasHealSoakPotion())
                {
                    return "Healh Soak Potion";
                }
                else
                {
                    canCheckIsConditioned = false;
                    return "";
                }
            }
        }
        // If the player's health is less than 25% of the max health, then use a health potion if it exists
        if (playerUnit.Character.CurrentHP <= playerUnit.CharacterData.MaxHealthPoints * 0.25f)
        {
            Debug.Log("Inside player <= 25% health");
            if (HasHealPotion())
            {
                return "Healh Potion";
            }
            else
            {
                return "Attack " + playerMostDamagingMove.MoveData.MoveName;
            }
        }
        //If the player's health is greater than 75% of the max health, it means that he is in a save condition, 
        //so he can decide between attacking, using a status condition move, or using a stats move
        if (playerUnit.Character.CurrentHP >= playerUnit.CharacterData.MaxHealthPoints * 0.75f)
        {
            Debug.Log("Inside player >= 75% health");
            //If the player's health is greater than the enemy's health, then he tends to use status condition moves or stats moves
            if (playerUnit.Character.CurrentHP >= enemyUnit.Character.CurrentHP)
            {
                return DontPrioritizeAttack();

            }
            //If the player's health is lower than the enemy's health, then he tends to attack
            else
            {
                return PrioritizeAttack();
            }
        }
        if (playerUnit.Character.CurrentHP > playerUnit.CharacterData.MaxHealthPoints * 0.25f && playerUnit.Character.CurrentHP < playerUnit.CharacterData.MaxHealthPoints * 0.75f)
        {
            if (playerUnit.Character.CurrentHP > playerUnit.CharacterData.MaxHealthPoints * 0.5f)
            {
                return PrioritizeAttack();
            }
            else
            {
                return "Attack " + playerMostDamagingMove.MoveData.MoveName;
            }
        }
        //Finally, if any of the previous conditions are not met, then attack
        Debug.Log("Inside default case");
        return "Attack " + playerMostDamagingMove.MoveData.MoveName;
    }

    private string PrioritizeAttack()
    {
        bool repeatSwitch;
        int maxAttempts = 100; // Limitar el número de intentos
        int attempts = 0;
        TBMove selectedMove = null;
        do
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            repeatSwitch = false;
            attempts++;
            if (attempts > maxAttempts)
            {
                foreach (var move in playerMoves)
                {
                    Debug.Log("Player move before ERROR " + move.MoveData.MoveName);
                }
                Debug.LogWarning("Exceeded maximum attempts in PrioritizeAttack. Attacking");
                return "Attack " + playerMostDamagingMove.MoveData.MoveName;
            }
            int randomAction = Random.Range(1, 7); // 1-6
            Debug.Log("Random action inside DontPrioritizeAttack" + randomAction);
            switch (randomAction)
            {
                case 1:
                case 2:
                case 3:
                case 4: // Attacks
                    return "Attack " + playerMostDamagingMove.MoveData.MoveName;
                case 5: // Status condition move if the enemy is not conditioned
                    if (enemyUnit.Character.Statuses.Count == 0)
                    {
                        var move = playerMoves.Find(m => m.MoveData.Effects.status != ConditionID.None);
                        if (move != null)
                        {
                            return "Attack " + move.MoveData.MoveName;
                        }
                    }
                    repeatSwitch = true;
                    break;
                case 6: // Stats move
                    int innerAttempts = 0;
                    do
                    {
                        innerAttempts++;
                        if (innerAttempts > maxAttempts)
                        {
                            foreach (var move in playerMoves)
                            {
                                Debug.Log("Player move before ERROR " + move.MoveData.MoveName);
                            }
                            Debug.LogError("Exceeded maximum inner attempts getting a stat move in PrioritizeAttack");
                            break;
                        }
                        if (enemyUnit.Character.Speed > playerUnit.Character.Speed)
                        {
                            if (GetDropOrBoostSpeedMove() != null)
                            {
                                selectedMove = GetDropOrBoostSpeedMove();
                            }
                            else
                            {
                                selectedMove = GetRandomStatsMove();
                            }
                        }
                        else
                        {
                            selectedMove = GetRandomStatsMove();
                        }
                    } while (selectedMove == null || IsSelectedStatMaxDroppedOrBoosted(selectedMove));
                    if (selectedMove != null)
                    {
                        return "Attack " + selectedMove.MoveData.MoveName;
                    }
                    repeatSwitch = true;
                    break;
            }
        } while (repeatSwitch);
        return "";
    }

    private string DontPrioritizeAttack()
    {
        bool repeatSwitch;
        int maxAttempts = 100; // Limitar el número de intentos
        int attempts = 0;
        TBMove selectedMove = null;
        do
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            repeatSwitch = false;
            attempts++;
            if (attempts > maxAttempts)
            {
                foreach (var move in playerMoves)
                {
                    Debug.Log("Player move before ERROR " + move.MoveData.MoveName);
                }
                Debug.LogWarning("Exceeded maximum attempts in DontPrioritizeAttack. Attacking");
                return "Attack " + playerMostDamagingMove.MoveData.MoveName;
            }

            int randomAction = Random.Range(1, 10); // 1-9
            Debug.Log("Random action inside DontPrioritizeAttack" + randomAction);
            switch (randomAction)
            {
                case 1:
                case 2:
                case 3:
                case 4: // Status condition move if the enemy is not conditioned
                    if (enemyUnit.Character.Statuses.Count == 0)
                    {
                        var move = playerMoves.Find(m => m.MoveData.Effects.status != ConditionID.None);
                        if (move != null)
                        {
                            return "Attack " + move.MoveData.MoveName;
                        }
                    }
                    repeatSwitch = true;
                    break;
                case 5:
                case 6:
                case 7: // Stats move
                    int innerAttempts = 0;
                    do
                    {
                        innerAttempts++;
                        if (innerAttempts > maxAttempts)
                        {
                            foreach (var move in playerMoves)
                            {
                                Debug.Log("Player move before ERROR " + move.MoveData.MoveName);
                            }
                            Debug.LogError("Exceeded maximum inner attempts getting a stat move in DontPrioritizeAttack");
                            break;
                        }
                        if (enemyUnit.Character.Speed > playerUnit.Character.Speed)
                        {
                            if (GetDropOrBoostSpeedMove() != null)
                            {
                                selectedMove = GetDropOrBoostSpeedMove();
                            }
                            else
                            {
                                selectedMove = GetRandomStatsMove();
                            }
                        }
                        else
                        {
                            selectedMove = GetRandomStatsMove();
                        }
                    } while (selectedMove == null || IsSelectedStatMaxDroppedOrBoosted(selectedMove));
                    if (selectedMove != null)
                    {
                        return "Attack " + selectedMove.MoveData.MoveName;
                    }
                    repeatSwitch = true;
                    break;
                case 8:
                case 9: // Attacks
                    return "Attack " + playerMostDamagingMove.MoveData.MoveName;
            }
        } while (repeatSwitch);
        return "";
    }


    bool HasHealPotion()
    {
        foreach (InventoryItem item in inventory.inventoryItems)
        {
            if (item.ItemData.ItemName == "Potion")
            {
                return true;
            }
        }
        return false;
    }
    bool HasHealPoisonPotion()
    {
        Debug.Log("Inventory items count inside HasHealPoisonPotion" + inventory.inventoryItems.Count);
        foreach (InventoryItem item in inventory.inventoryItems)
        {
            if (item.ItemData.ItemName == "Poison Potion")
            {
                return true;
            }
        }
        return false;
    }
    bool HasHealBurnPotion()
    {
        Debug.Log("Inventory items count " + inventory.inventoryItems.Count);
        foreach (InventoryItem item in inventory.inventoryItems)
        {
            if (item.ItemData.ItemName == "Burn Potion")
            {
                return true;
            }
        }
        return false;
    }
    bool HasHealSoakPotion()
    {
        foreach (InventoryItem item in inventory.inventoryItems)
        {
            if (item.ItemData.ItemName == "Soak Potion")
            {
                return true;
            }
        }
        return false;
    }
    public bool EnemyCanFinishPlayerWithCritical()
    {
        bool canFinish = false;
        foreach (var move in enemyMoves)
        {
            float critical = 2f; //Considers always the worst case scenario

            float type = TypeChart.GetEffectiveness(move.MoveData.Element, playerUnit.Character.CharacterData.Element);

            int damage = Mathf.FloorToInt(move.MoveData.Power * (enemyUnit.Character.Attack / playerUnit.Character.Defense) * critical * type * 0.1f);

            if (damage >= playerUnit.Character.CurrentHP)
            {
                canFinish = true;
                break;
            }
        }
        return canFinish;
    }
    public bool EnemyCanFinishPlayerWithoutCritical()
    {
        bool canFinish = false;
        foreach (var move in enemyMoves)
        {
            float critical = 1f; //Considers the best case scenario

            float type = TypeChart.GetEffectiveness(move.MoveData.Element, playerUnit.Character.CharacterData.Element);

            int damage = Mathf.FloorToInt(move.MoveData.Power * (enemyUnit.Character.Attack / playerUnit.Character.Defense) * critical * type * 0.1f);

            if (damage >= playerUnit.Character.CurrentHP)
            {
                canFinish = true;
                break;
            }
        }
        return canFinish;
    }
    public bool PlayerCanFinishEnemy()
    {
        bool canFinish = false;

        if (GetMostPowerfulAttack() >= enemyUnit.Character.CurrentHP)
        {
            canFinish = true;
        }

        return canFinish;
    }
    public int GetMostPowerfulAttack()
    {
        int maxDamage = 0;

        foreach (TBMove move in playerMoves)
        {
            playerCritical = 1f;
            if (Random.value * 100f <= move.MoveData.CriticalChance)
            {
                playerCritical = 2f;
            }

            float type = TypeChart.GetEffectiveness(move.MoveData.Element, enemyUnit.Character.CharacterData.Element);

            int damage = Mathf.FloorToInt(move.MoveData.Power * (playerUnit.Character.Attack / enemyUnit.Character.Defense) * playerCritical * type * 0.1f);

            if (damage > maxDamage)
            {
                maxDamage = damage;
                playerMostDamagingMove = move;
            }
        }
        return maxDamage;
    }
    bool PlayerIsConditioned()
    {
        return playerUnit.Character.Statuses.Count > 0;
    }
    ConditionID PlayerCondition()
    {
        Condition status = playerUnit.Character.Statuses.Find(status => status.ID == ConditionID.Burning || status.ID == ConditionID.Poisoned || status.ID == ConditionID.Soaked);
        Debug.Log("Player condition " + status.ID);
        return status.ID;
    }
    TBMove GetDropOrBoostSpeedMove()
    {
        var move = playerMoves.Find(move => move.MoveData.Effects.statBoosts.Exists(statBoost => statBoost.stat == Stat.Speed && (statBoost.boost < 0 || statBoost.boost > 0)));
        if (move == null)
        {
            Debug.LogWarning("No move found in GetDropOrBoostSpeedMove");
        }
        return move;
    }
    TBMove GetRandomStatsMove()
    {
        var move = playerMoves.Find(move => move.MoveData.Category == MoveCategory.Stats);
        if (move == null)
        {
            Debug.LogWarning("No move found in GetRandomStatsMove");
        }
        return move;
    }
    bool IsSelectedStatMaxDroppedOrBoosted(TBMove move)
    {
        Debug.Log("Move to check stat max dropped or boosted " + move.MoveData.MoveName);
        if (move.MoveData.Effects.statBoosts == null || move.MoveData.Effects.statBoosts.Count == 0)
        {
            Debug.LogWarning("No stat boosts found for move " + move.MoveData.MoveName);
            return false;
        }
        var stat = move.MoveData.Effects.statBoosts[0].stat;
        return playerUnit.Character.Stats.ContainsKey(stat) && (playerUnit.Character.Stats[stat] == 5 || playerUnit.Character.Stats[stat] == 0);
    }

}

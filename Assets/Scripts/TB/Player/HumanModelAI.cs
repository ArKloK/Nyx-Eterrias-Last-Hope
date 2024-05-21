using System.Collections.Generic;
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

    public float PlayerCritical { get => playerCritical; set => playerCritical = value; }
    public TBMove PlayerMostDamagingMove { get => playerMostDamagingMove; set => playerMostDamagingMove = value; }

    public void ResetBattle()
    {
        playerMoves = playerUnit.Character.Moves;
        enemyMoves = enemyUnit.Character.Moves;
    }

    public string GetAction()
    {
        string action = "";
        //If the enemy can finish the player without critical, then think
        if (EnemyCanFinishPlayerWithoutCritical())
        {
            Debug.Log("Inside EnemyCanFinishPlayerWithoutCritical");
            if (PlayerCanFinishEnemy())
            {
                //If the player can finish the enemy, and is faster than the enemy, then attack 
                if (playerUnit.Character.Speed >= enemyUnit.Character.Speed)
                {
                    action = "Attack " + playerMostDamagingMove.MoveData.MoveName;
                }
                // If the player can finish the enemy, and is slower than the enemy, then use a health potion if it exists. Otherwise, attack
                else
                {
                    if (HasHealPotion())
                    {
                        action = "Healh Potion";
                    }
                    else
                    {
                        action = "Attack " + playerMostDamagingMove.MoveData.MoveName;
                    }
                }
            }
        }
        //If the enemy can finish the player with critical, tends to ignore the menace but it has a low probability (15%) to be "scared" of losing the battle
        //and tries to use a health potion
        else if (EnemyCanFinishPlayerWithCritical())
        {
            Debug.Log("Inside EnemyCanFinishPlayerWithCritical");
            int randomProbability = Random.Range(1, 21); // 1-20
            if (Random.Range(1, 21) > 1)// 
            {
                if (HasHealPotion())
                {
                    action = "Healh Potion";
                }
                else
                {
                    GetMostPowerfulAttack();
                    action = "Attack " + playerMostDamagingMove.MoveData.MoveName;
                }
            }
        }
        //If the player can finish the enemy, then attack
        else if (PlayerCanFinishEnemy())
        {
            Debug.Log("Inside PlayerCanFinishEnemy");
            action = "Attack " + playerMostDamagingMove.MoveData.MoveName;
        }
        // If the player is conditioned, then use the appropriate potion if it exists
        else if (PlayerIsConditioned())
        {
            Debug.Log("Inside PlayerIsConditioned");
            if (PlayerCondition() == ConditionID.Burning)
            {
                Debug.Log("Inside player burning");
                if (HasHealBurnPotion())
                {
                    Debug.Log("Inside player burning has heal burn potion");
                    action = "Healh Burn Potion";
                }
            }
            else if (PlayerCondition() == ConditionID.Poisoned)
            {
                Debug.Log("Inside player poisoned");
                if (HasHealPoisonPotion())
                {
                    Debug.Log("Inside player poisoned has heal poison potion");
                    action = "Healh Poison Potion";
                }
            }
            else if (PlayerCondition() == ConditionID.Soaked)
            {
                if (HasHealSoakPotion())
                {
                    action = "Healh Soak Potion";
                }
            }
        }
        // If the player's health is less than 25% of the max health, then use a health potion if it exists
        else if (playerUnit.Character.CurrentHP <= playerUnit.CharacterData.MaxHealthPoints * 0.25f)
        {
            Debug.Log("Inside player <= 25% health");
            if (HasHealPotion())
            {
                action = "Healh Potion";
            }
        }
        else if (playerUnit.Character.CurrentHP >= enemyUnit.Character.CurrentHP)
        {
            Debug.Log("Inside player >= enemy health");
            //If the player's health is greater than 75% of the max health, it means that he is in a save condition, so he prioritizes status condition or stats moves
            if (playerUnit.Character.CurrentHP >= playerUnit.CharacterData.MaxHealthPoints * 0.75f)
            {
                int randomAction = Random.Range(1, 10); // 1-9
                switch (randomAction)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4: // 1-4 Status condition move if the enemy is not conditioned
                        {
                            if (enemyUnit.Character.Statuses.Count == 0)
                            {
                                //Perform status condition move
                                action = "Attack " + playerMoves.Find(move => move.MoveData.Effects.status != ConditionID.None).MoveData.MoveName;
                            }
                            break;
                        }
                    case 5:
                    case 6:
                    case 7:
                    case 8: // 5-8 Stats move. If the speed of the enemy is greater than the player, then drop/boost the speed of the enemy/player respectively, else perform a random stats move
                        {
                            TBMove selectedMove;
                            do
                            {
                                Debug.Log("Enemy speed " + enemyUnit.Character.Speed + " Player speed " + playerUnit.Character.Speed);
                                if (enemyUnit.Character.Speed > playerUnit.Character.Speed)
                                {
                                    selectedMove = GetDropOrBoostSpeedMove();
                                }
                                else
                                {
                                    selectedMove = GetRandomStatsMove();
                                }
                            } while (IsSelectedStatMaxDroppedOrBoosted(selectedMove));

                            action = "Attack " + selectedMove.MoveData.MoveName;
                            break;
                        }
                    case 9: // Attacks
                        {
                            GetMostPowerfulAttack();
                            action = "Attack " + playerMostDamagingMove.MoveData.MoveName;
                            break;
                        }
                }
            }
            //If the player's health is less than 75% of the max health, then he prioritizes attacks
            else
            {
                int randomAction = Random.Range(1, 7); // 1-6
                switch (randomAction)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4: // 1-4 Attacks
                        {
                            GetMostPowerfulAttack();
                            action = "Attack " + playerMostDamagingMove.MoveData.MoveName;
                            break;

                        }
                    case 5: // 5 Status condition move if the enemy is not conditioned
                        {
                            if (enemyUnit.Character.Statuses.Count == 0)
                            {
                                //Perform status condition move
                                action = "Attack " + playerMoves.Find(move => move.MoveData.Effects.status != ConditionID.None).MoveData.MoveName;
                            }
                            break;
                        }
                    case 6: // 6 Stats move. If the speed of the enemy is greater than the player, then drop/boost the speed of the enemy/player respectively, else perform a random stats move
                        {
                            TBMove selectedMove;
                            do
                            {
                                if (enemyUnit.Character.Speed > playerUnit.Character.Speed)
                                {
                                    selectedMove = GetDropOrBoostSpeedMove();
                                }
                                else
                                {
                                    selectedMove = GetRandomStatsMove();
                                }
                            } while (IsSelectedStatMaxDroppedOrBoosted(selectedMove));

                            action = "Attack " + selectedMove.MoveData.MoveName;
                            break;
                        }
                }
            }
        }
        //Finally, if the player's health is less than the enemy's health, then attack
        else if (playerUnit.Character.CurrentHP < enemyUnit.Character.CurrentHP)
        {
            Debug.Log("Inside player < enemy health");
            GetMostPowerfulAttack();
            action = "Attack " + playerMostDamagingMove.MoveData.MoveName;
        }
        Debug.Log("Action: " + action);
        return action;
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
            if (UnityEngine.Random.value * 100f <= move.MoveData.CriticalChance)
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
        //Returns the first move that drops/boosts the speed of the enemy/player respectively
        return playerMoves.Find(move => move.MoveData.Effects.statBoosts.Exists(statBoost => statBoost.stat == Stat.Speed && statBoost.boost < 0) || move.MoveData.Effects.statBoosts.Exists(statBoost => statBoost.stat == Stat.Speed && statBoost.boost > 0));
    }
    TBMove GetRandomStatsMove()
    {
        //Returns the first move that drops/boosts the stats of the enemy/player respectively
        return playerMoves.Find(move => move.MoveData.Category == MoveCategory.Stats);
    }
    bool IsSelectedStatMaxDroppedOrBoosted(TBMove move)
    {
        Debug.Log("Move to check stat max dropped or boosted " + move.MoveData.MoveName);
        //Returns true if the selected stat is max dropped or boosted
        return playerUnit.Character.Stats[move.MoveData.Effects.statBoosts[0].stat] == 6 || playerUnit.Character.Stats[move.MoveData.Effects.statBoosts[0].stat] == 0;
    }

}

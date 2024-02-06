using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.Poisoned,
            new Condition
            {
                ID = ConditionID.Poisoned,
                Name = "Poisoned",
                Description = "This unit is poisoned and will take damage at the start of its turn.",
                StartMessage = "is poisoned!",
                HudMessage = "PSN",
                RepeatedMovementMessage = "is already poisoned and can't be poisoned again!",
                OnEffectAppliedToEnemy = (TBEnemy enemy) =>
                {
                    enemy.statusChanges.Enqueue($"{enemy.enemyData.Name} is hurt by poison!");
                    enemy.UpdateHp(enemy.MaxHp / 8);
                },
                OnEffectAppliedToPlayer = (TBPlayer player) =>
                {
                    player.statusChanges.Enqueue($"{player.playerData.Name} is hurt by poison!");
                    player.UpdateHp(player.MaxHp / 8);
                }
            }
        },
        {
            ConditionID.Burning,
            new Condition
            {
                ID = ConditionID.Burning,
                Name = "Burning",
                Description = "This unit is burning and will take damage at the start of its turn.",
                HudMessage = "BRN",
                StartMessage = "is burning!",
                RepeatedMovementMessage = "is already burning and can't be burned again!",
                OnEffectAppliedToEnemy = (TBEnemy enemy) =>
                {
                    enemy.statusChanges.Enqueue($"{enemy.enemyData.Name} is hurt by burn!");
                    enemy.UpdateHp(enemy.MaxHp / 16);
                },
                OnEffectAppliedToPlayer = (TBPlayer player) =>
                {
                    player.statusChanges.Enqueue($"{player.playerData.Name} is hurt by burn!");
                    player.UpdateHp(player.MaxHp / 16);
                }
            }
        },
        {
            ConditionID.Soaked,
            new Condition
            {
                ID = ConditionID.Soaked,
                Name = "Soaked",
                Description = "This unit is soaked and will have its accuracy reduced by 50%.",
                HudMessage = "SOAK",
                StartMessage = "is soaked and will have its accuracy and speed reduced by 50%!",
                RepeatedMovementMessage = "is already soaked and can't be soaked again!",
                OnBeforePlayerMove = (TBPlayer player) =>
                {
                    player.statusTime--;
                    if (player.statusTime <= 0)
                    {
                        player.statusTime = 0;
                        if (player.statuses.Contains(ConditionsDB.Conditions[ConditionID.Soaked]))
                        {
                            player.moves.ForEach(move =>
                            {
                                player.Speed = PlayerStats.TBAttackSpeed;
                                move.Accuracy = move.MoveData.Accuracy;
                            });
                            player.statusChanges.Enqueue($"{player.playerData.Name} is no longer soaked!");
                            return ConditionsDB.Conditions[ConditionID.Soaked];
                        }
                    }
                    return null;
                },
                OnBeforeEnemyMove = (TBEnemy enemy) =>
                {
                    enemy.statusTime--;
                    if (enemy.statusTime <= 0)
                    {
                        enemy.statusTime = 0;
                        if (enemy.statuses.Contains(ConditionsDB.Conditions[ConditionID.Soaked]))
                        {
                            enemy.moves.ForEach(move =>
                            {
                                enemy.Speed = enemy.enemyData.AttackSpeed;
                                move.Accuracy = move.MoveData.Accuracy;
                            });
                            enemy.statusChanges.Enqueue($"{enemy.enemyData.Name} is no longer soaked!");
                            return ConditionsDB.Conditions[ConditionID.Soaked];
                        }
                    }
                    return null;
                },
                OnEffectAppliedToPlayer = (TBPlayer player) =>
                {
                    //Soaked for 2-4 turns
                    player.statusTime = UnityEngine.Random.Range(2, 5);
                    Debug.Log("Player soaked for " + player.statusTime + " turns");
                    //Reduce player's speed and moves accuracy
                    player.moves.ForEach(move =>
                    {
                        player.Speed = PlayerStats.TBAttackSpeed / 2;
                        move.Accuracy = move.MoveData.Accuracy / 2;
                        Debug.Log("Move" + move.MoveData.MoveName + " accuracy: " + move.Accuracy);
                        Debug.Log("Player speed: " + player.Speed);
                    });
                },
                OnEffectAppliedToEnemy = (TBEnemy enemy) =>
                {
                    //Soaked for 2-4 turns
                    enemy.statusTime = UnityEngine.Random.Range(2, 5);
                    Debug.Log("Enemy soaked for " + enemy.statusTime + " turns");
                    //Reduce enemy speed and moves accuracy
                    enemy.moves.ForEach(move =>
                    {
                        enemy.Speed = enemy.enemyData.AttackSpeed / 2;
                        move.Accuracy = move.MoveData.Accuracy / 2;
                        Debug.Log("Move" + move.MoveData.MoveName + " accuracy: " + move.Accuracy);
                        Debug.Log("Enemy speed: " + enemy.Speed);
                    });
                }
            }
        },
    };
}

public enum ConditionID
{
    None,
    Poisoned,
    Burning,
    Soaked,
}
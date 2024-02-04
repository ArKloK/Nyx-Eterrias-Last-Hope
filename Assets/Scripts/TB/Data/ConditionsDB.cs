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
                Name = "Poisoned",
                Description = "This unit is poisoned and will take damage at the start of its turn.",
                StartMessage = "is poisoned!",
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
                Name = "Burning",
                Description = "This unit is burning and will take damage at the start of its turn.",
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
                Name = "Soaked",
                Description = "This unit is soaked and will have its accuracy reduced by 50%.",
                StartMessage = "is soaked and will have its accuracy reduced by 50%!",
                OnEffectAppliedToPlayer = (TBPlayer player) =>
                {
                    player.moves.ForEach(move =>
                    {
                        move.MoveData.Accuracy /= 2;
                    });
                },
                OnEffectAppliedToEnemy = (TBEnemy enemy) =>
                {
                    enemy.moves.ForEach(move =>
                    {
                        move.MoveData.Accuracy /= 2;
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
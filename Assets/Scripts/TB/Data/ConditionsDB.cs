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
            ConditionID.Slowed,
            new Condition
            {
                Name = "Slowed",
                Description = "This unit is slowed and will have its movement reduced by 2.",
                StartMessage = "is slowed!",
            }
        },
        {
            ConditionID.Blinded,
            new Condition
            {
                Name = "Blinded",
                Description = "This unit is blinded and will have its accuracy reduced by 50%.",
                StartMessage = "is blinded!",
            }
        },
    };
}

public enum ConditionID
{
    None,
    Poisoned,
    Burning,
    Slowed,
    Blinded,
}
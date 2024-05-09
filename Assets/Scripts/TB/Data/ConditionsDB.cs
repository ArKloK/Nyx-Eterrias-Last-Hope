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
                OnEffectAppliedToCharacter = (TBCharacter character) =>
                {
                    character.StatusChanges.Enqueue($"{character.CharacterData.Name} is hurt by poison!");
                    character.UpdateHp(character.MaxHp / 8);
                },
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
                OnEffectAppliedToCharacter = (TBCharacter character) =>
                {
                    character.StatusChanges.Enqueue($"{character.CharacterData.Name} is hurt by burn!");
                    character.UpdateHp(character.MaxHp / 16);
                },
            }
        },
        {
            ConditionID.Soaked,
            new Condition
            {
                ID = ConditionID.Soaked,
                Name = "Soaked",
                Description = "This unit is soaked and will have its accuracy reduced by 25%.",
                HudMessage = "SOAK",
                StartMessage = "is soaked and will have its accuracy and speed reduced by 25%!",
                RepeatedMovementMessage = "is already soaked and can't be soaked again!",
                OnBeforeCharacterMove = (TBCharacter character) =>
                {
                    character.StatusTime--;
                    if (character.StatusTime <= 0)
                    {
                        character.StatusTime = 0;
                        if (character.Statuses.Contains(ConditionsDB.Conditions[ConditionID.Soaked]))
                        {
                            character.Moves.ForEach(move =>
                            {
                                character.Speed = character.CharacterData.AttackSpeed;
                                move.Accuracy = move.MoveData.Accuracy;
                            });
                            character.StatusChanges.Enqueue($"{character.CharacterData.Name} is no longer soaked!");
                            return ConditionsDB.Conditions[ConditionID.Soaked];
                        }
                    }
                    return null;
                },
                OnEffectAppliedToCharacter = (TBCharacter character) =>
                {
                    //Soaked for 2-4 turns
                    character.StatusTime = UnityEngine.Random.Range(2, 5);
                    Debug.Log($"{character.CharacterData.Name} soaked for " + character.StatusTime + " turns");
                    
                    //Reduce character's speed and moves accuracy
                
                    Debug.Log($"{character.CharacterData.Name} speed before soak: " + character.Speed);
                    character.Speed = (int)(character.CharacterData.AttackSpeed * 0.75f);
                    Debug.Log($"{character.CharacterData.Name} speed: " + character.Speed);

                    character.Moves.ForEach(move =>
                    {
                        move.Accuracy = (int)(move.MoveData.Accuracy * 0.75f);
                        Debug.Log("Move" + move.MoveData.MoveName + " accuracy: " + move.Accuracy);
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
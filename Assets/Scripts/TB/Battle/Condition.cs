using System;

public class Condition
{
    public ConditionID ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string HudMessage { get; set; }
    public string StartMessage { get; set; }
    public string RepeatedMovementMessage { get; set; }
    public string EndMessage { get; set; }

    //New events
    public Action<TBCharacter> OnEffectAppliedToCharacter { get; set; }
    public Func<TBCharacter, Condition> OnBeforeCharacterMove { get; set; }

}

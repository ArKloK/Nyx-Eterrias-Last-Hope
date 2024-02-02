using System;

public class Condition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    public Action<TBEnemy> OnEffectAppliedToEnemy { get; set; }
    public Action<TBPlayer> OnEffectAppliedToPlayer { get; set; }

}

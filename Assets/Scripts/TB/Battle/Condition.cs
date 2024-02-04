using System;

public class Condition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public string RepeatedMovementMessage { get; set; }
    public string EndMessage { get; set; }

    public Action<TBEnemy> OnEffectAppliedToEnemy { get; set; }
    public Action<TBPlayer> OnEffectAppliedToPlayer { get; set; }

    public Func<TBEnemy, bool> OnBeforeEnemyMove { get; set; }
    public Func<TBPlayer, bool> OnBeforePlayerMove { get; set; }

}

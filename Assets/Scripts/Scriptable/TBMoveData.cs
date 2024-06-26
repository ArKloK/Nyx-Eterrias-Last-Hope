using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TBMoveData : ScriptableObject
{
    public string MoveName;
    public string Description;
    public Element Element;
    public MoveCategory Category;
    public MoveEffects Effects;
    public MoveTarget Target;
    public int Power;
    public int Accuracy;
    public int CriticalChance;
}

public enum MoveCategory
{
    Physical,
    Stats
}

[System.Serializable]
public class MoveEffects
{
    public List<StatBoost> statBoosts;
    public ConditionID status;
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveTarget
{
    Foe,
    Self
}
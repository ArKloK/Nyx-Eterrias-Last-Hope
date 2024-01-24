using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TBMoveData : ScriptableObject
{
    public string MoveName;
    public string Description;
    public Element Element;
    public int Power;
    public int Accuracy;
    public int CriticalChance;
}   

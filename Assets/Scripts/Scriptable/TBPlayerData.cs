using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class TBPlayerData : ScriptableObject
{
    public string Name;
    public Element element;
    public Sprite Sprite;
    //Base stats
    public int MaxHealthPoints;
    public int AttackPower;
    public int DefensePower;
    public int AttackSpeed;
    public List<LearnableMove> LearnableMoves;
}

[System.Serializable]
public class LearnableMove
{
    public TBMoveData Move;
    public int Level;
}
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class TBEnemyData : ScriptableObject
{
    public string EnemyName;
    public Element element;
    public Sprite EnemySprite;
    //Base stats
    public int MaxHealthPoints;
    public int AttackPower;
    public int DefensePower;
    public int AttackSpeed;
    public List<TBMoveData> Moves = new List<TBMoveData>(4);
}
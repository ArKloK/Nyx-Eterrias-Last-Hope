using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class TBPlayerData : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    public List<LearnableMove> LearnableMoves;
}

[System.Serializable]
public class LearnableMove
{
    public TBMoveData Move;
    public int Level;
}
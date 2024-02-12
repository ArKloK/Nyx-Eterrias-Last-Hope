using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Diagnostics;

[CreateAssetMenu]
public class TBCharacterData : ScriptableObject
{
    [SerializeField] string _name;
    [SerializeField] Sprite _sprite;
    [SerializeField] List<LearnableMove> _learnableMoves;
    [SerializeField] bool _isEnemy;
    [SerializeField][ShowIf("_isEnemy")] int _maxHealthPoints;
    [SerializeField][ShowIf("_isEnemy")] int _attackPower;
    [SerializeField][ShowIf("_isEnemy")] int _defensePower;
    [SerializeField][ShowIf("_isEnemy")] int _attackSpeed;
    [SerializeField][ShowIf("_isEnemy")] int _experienceAmount;
    [SerializeField][ShowIf("_isEnemy")] Element _element;

    public string Name { get => _name; set => _name = value; }
    public Sprite Sprite { get => _sprite; set => _sprite = value; }
    public bool IsEnemy { get => _isEnemy; set => _isEnemy = value; }
    public int MaxHealthPoints { get => _maxHealthPoints; set => _maxHealthPoints = value; }
    public int AttackPower { get => _attackPower; set => _attackPower = value; }
    public int DefensePower { get => _defensePower; set => _defensePower = value; }
    public int AttackSpeed { get => _attackSpeed; set => _attackSpeed = value; }
    public int ExperienceAmount { get => _experienceAmount; set => _experienceAmount = value; }
    public Element Element { get => _element; set => _element = value; }
    public List<LearnableMove> LearnableMoves { get => _learnableMoves; set => _learnableMoves = value; }
}

[System.Serializable]
public class LearnableMove
{
    public TBMoveData Move;
    public int Level;
}
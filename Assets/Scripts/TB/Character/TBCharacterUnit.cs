using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class TBCharacterUnit : MonoBehaviour
{
    [SerializeField] TBCharacterData _characterData;
    [SerializeField] int _level;
    [SerializeField] TBCharacter _character;
    public TBCharacterData CharacterData { get => _characterData; set => _characterData = value; }
    public TBCharacter Character { get => _character; set => _character = value; }

    public void setData()
    {
        if (_characterData.IsEnemy)
        {
            _character = new TBCharacter(_characterData, _level);
        }
        else
        {
            _character = new TBCharacter(_characterData, PlayerStats.CurrentLevel);
        }
    }
}

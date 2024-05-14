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

    public void setData(bool TBDemo)
    {
        if (_characterData.IsEnemy)
        {
            _character = new TBCharacter(_characterData, _level, TBDemo);
        }
        else
        {
            if (!TBDemo)
                _character = new TBCharacter(_characterData, PlayerStats.CurrentLevel, TBDemo);
            else
            {
                int randomLevel = Random.Range(5, 7);
                PlayerStats.CurrentLevel = randomLevel;
                PlayerStats.MaxHealthPoints = PlayerStats.MaxHealthPoints * PlayerStats.CurrentLevel;
                PlayerStats.CurrentHealthPoints = PlayerStats.MaxHealthPoints;
                _character = new TBCharacter(_characterData, randomLevel, TBDemo);
            }
        }
    }
}

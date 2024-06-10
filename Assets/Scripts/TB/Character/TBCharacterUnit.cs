using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class TBCharacterUnit : MonoBehaviour
{
    [SerializeField] TBCharacterData _characterData;
    [SerializeField] int _level;
    [SerializeField] TBCharacter _character;
    int baseMaxHealthPoints = 0;
    private int currentSet;

    public TBCharacterData CharacterData { get => _characterData; set => _characterData = value; }
    public TBCharacter Character { get => _character; set => _character = value; }

    public void setData(bool TBDemo)
    {
        if (_characterData.IsEnemy)
        {
            _character = new TBCharacter(_characterData, _level, TBDemo, currentSet);
        }
        else
        {
            if (!TBDemo)
                _character = new TBCharacter(_characterData, PlayerStats.CurrentLevel, TBDemo, currentSet);
            else
            {
                if (baseMaxHealthPoints == 0) baseMaxHealthPoints = PlayerStats.MaxHealthPoints;

                if (currentSet > 2)
                    currentSet = 0;

                Debug.Log("Current set: " + currentSet);
                int tBDemoPlayerLevel = 5;
                PlayerStats.CurrentLevel = tBDemoPlayerLevel;
                PlayerStats.MaxHealthPoints = baseMaxHealthPoints * PlayerStats.CurrentLevel;
                PlayerStats.CurrentHealthPoints = PlayerStats.MaxHealthPoints;
                _character = new TBCharacter(_characterData, tBDemoPlayerLevel, TBDemo, currentSet);
                currentSet++;
            }
        }
    }
}

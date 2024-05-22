using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TBCharacter
{
    #region Private Fields
    private TBCharacterData _characterData;
    private int _level;
    private int _currentHP;
    private bool _hpChanged;
    private int _statusTime;
    private List<TBMove> _moves;
    private TBMove _currentMove;
    private Dictionary<Stat, int> _stats;
    private Dictionary<Stat, int> _statBoosts;
    private Queue<string> _statusChanges = new Queue<string>();
    private List<Condition> _statuses = new List<Condition>();
    #endregion

    #region Public Properties
    public TBCharacterData CharacterData { get => _characterData; set => _characterData = value; }
    public int Level { get => _level; set => _level = value; }
    public int CurrentHP { get => _currentHP; set => _currentHP = value; }
    public bool HpChanged { get => _hpChanged; set => _hpChanged = value; }
    public int StatusTime { get => _statusTime; set => _statusTime = value; }
    public List<TBMove> Moves { get => _moves; set => _moves = value; }
    public TBMove CurrentMove { get => _currentMove; set => _currentMove = value; }
    public Dictionary<Stat, int> Stats { get => _stats; set => _stats = value; }
    public Dictionary<Stat, int> StatBoosts { get => _statBoosts; set => _statBoosts = value; }
    public List<Condition> Statuses { get => _statuses; set => _statuses = value; }
    public Queue<string> StatusChanges { get => _statusChanges; set => _statusChanges = value; }
    public event Action OnStatusChanged;
    public float Attack
    {
        get
        {
            return GetStat(Stat.Attack);
        }
    }

    public float Defense
    {
        get
        {
            return GetStat(Stat.Defense);
        }
    }

    public int Speed
    {
        get
        {
            return GetStat(Stat.Speed);
        }
        set
        {
            _stats[Stat.Speed] = value;
        }
    }

    public int MaxHp
    {
        get; private set;
    }
    #endregion

    public TBCharacter(TBCharacterData characterData, int level, bool TBDemo)
    {
        _characterData = characterData;
        _level = level;
        CalculateStats();

        _moves = new List<TBMove>();

        //If the character is the player, the moves will be taken from the player's stats unless the moves list in Player stats is empty
        if (!_characterData.IsEnemy)
        {
            if (!TBDemo)
                _moves = PlayerStats.Moves;
            else
            {
                string[] randomAttacks = { "Aqua Shoot", "Heat Hit", "Roots Power" };
                string randomAttack = randomAttacks[UnityEngine.Random.Range(0, randomAttacks.Length)];

                string[] randomStatsMoves = { "Intimidate", "Robust Defense", "Slow Tempo", "Tailwind" };
                string randomStatMove1 = randomStatsMoves[UnityEngine.Random.Range(0, randomStatsMoves.Length)];
                string randomStatMove2;
                do
                {
                    randomStatMove2 = randomStatsMoves[UnityEngine.Random.Range(0, randomStatsMoves.Length)];
                } while (randomStatMove1 == randomStatMove2);

                string[] randomStatusConditionMoves = { "Soak", "Burn", "Poison" };
                string randomStatusConditionMove = "";
                switch (randomAttack)
                {
                    case "Aqua Shoot":
                        randomStatusConditionMove = "Soak";
                        break;
                    case "Heat Hit":
                        randomStatusConditionMove = "Burn";
                        break;
                    case "Roots Power":
                        randomStatusConditionMove = "Poison";
                        break;
                }

                LearnableMove desiredMove, attackMove;
                attackMove = PlayerStats.LearnableMoves.Find(m => m.MoveData.MoveName == randomAttack);
                Debug.Log("Random attack: " + randomAttack);
                _moves.Add(new TBMove(attackMove.MoveData));
                desiredMove = PlayerStats.LearnableMoves.Find(m => m.MoveData.MoveName == randomStatMove1);
                Debug.Log("Random stat move 1: " + randomStatMove1);
                _moves.Add(new TBMove(desiredMove.MoveData));
                desiredMove = PlayerStats.LearnableMoves.Find(m => m.MoveData.MoveName == randomStatMove2);
                Debug.Log("Random stat move 2: " + randomStatMove2);
                _moves.Add(new TBMove(desiredMove.MoveData));
                desiredMove = PlayerStats.LearnableMoves.Find(m => m.MoveData.MoveName == randomStatusConditionMove);
                Debug.Log("Random status condition move: " + randomStatusConditionMove);
                _moves.Add(new TBMove(desiredMove.MoveData));
            }
        }
        //If the character is an enemy, it will have a set of moves        
        else
        {
            foreach (LearnableMove move in _characterData.LearnableMoves)
            {
                if (move.Level <= level)
                {
                    _moves.Add(new TBMove(move.MoveData));
                }

                if (_moves.Count >= 4)
                {
                    break;
                }
            }
        }

        ResetStatBoosts();
    }

    public void ResetStatBoosts()
    {
        _statBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.Speed, 0 },
        };
    }

    void CalculateStats()
    {
        if (_characterData.IsEnemy)
        {
            MaxHp = _characterData.MaxHealthPoints * _level;
            _currentHP = MaxHp;
            _stats = new Dictionary<Stat, int>
            {
                { Stat.Attack, Mathf.FloorToInt(_characterData.AttackPower * _level) },
                { Stat.Defense, Mathf.FloorToInt(_characterData.DefensePower * _level) },
                { Stat.Speed, Mathf.FloorToInt(_characterData.AttackSpeed * _level) }
            };

        }
        else
        {
            _currentHP = PlayerStats.CurrentHealthPoints;
            _characterData.Element = PlayerStats.Element;
            _stats = new Dictionary<Stat, int>
            {
                { Stat.Attack, Mathf.FloorToInt(PlayerStats.TBAttackPower * _level) },
                { Stat.Defense, Mathf.FloorToInt(PlayerStats.TBDefensePower * _level) },
                { Stat.Speed, Mathf.FloorToInt(PlayerStats.TBAttackSpeed * _level) }
            };
            MaxHp = PlayerStats.MaxHealthPoints;
        }
    }

    public void AddStatus(ConditionID conditionId)
    {
        var condition = ConditionsDB.Conditions[conditionId];
        if (_statuses.Contains(condition))
        {
            _statusChanges.Enqueue($"{_characterData.Name} {condition.RepeatedMovementMessage}");
            return;
        }
        _statuses.Add(condition);

        //Statuses like soaked will be applied immediately and only once
        if (conditionId.Equals(ConditionID.Soaked))
        {
            condition.OnEffectAppliedToCharacter?.Invoke(this);
        }
        _statusChanges.Enqueue($"{_characterData.Name} {condition.StartMessage}");
        OnStatusChanged?.Invoke();
    }
    public void RemoveStatus(ConditionID conditionId)
    {
        var condition = _statuses.Find(status => status == ConditionsDB.Conditions[conditionId]);
        if (condition != null)
        {
            _statuses.Remove(condition);
        }
        OnStatusChanged?.Invoke();
    }
    public void RemoveAllStatuses()
    {
        _statuses.Clear();
        //OnStatusChanged?.Invoke();
    }
    public int GetStat(Stat stat)
    {
        int statValue = _stats[stat];

        int boost = _statBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f };

        if (boost >= 0)
        {
            statValue = Mathf.FloorToInt(statValue * boostValues[boost]);
        }
        else
        {
            statValue = Mathf.FloorToInt(statValue / boostValues[-boost]);
        }

        return statValue;
    }
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;
            bool canBoostOrDrop = true;

            //First we check if the stat can be boosted or dropped
            if (_statBoosts[stat] == 5 || _statBoosts[stat] == -5)
            {
                _statusChanges.Enqueue($"{_characterData.Name}'s {stat} can't go any higher or lower!");
                canBoostOrDrop = false;
            }
            else
            {
                this._statBoosts[stat] = Mathf.Clamp(this._statBoosts[stat] + boost, -5, 5);
            }

            //If the stat was boosted or dropped, we add the message to the status changes queue
            if (canBoostOrDrop)
            {
                if (boost > 0)
                {
                    _statusChanges.Enqueue($"{_characterData.Name}'s {stat} increased!");
                }
                else
                {
                    _statusChanges.Enqueue($"{_characterData.Name}'s {stat} decreased!");
                } 
                
                Debug.Log(CharacterData.Name + "'s " + stat + " has been boosted to " + this._statBoosts[stat]);
            }
        }
    }
    public DamageDetails TakeDamage(TBMove move, TBCharacter attacker)
    {
        float critical = 1f;
        if (UnityEngine.Random.value * 100f <= move.MoveData.CriticalChance)
        {
            Debug.Log($"{attacker._characterData.Name} did Critical!");
            critical = 2f;
        }
        float type = TypeChart.GetEffectiveness(move.MoveData.Element, _characterData.Element);
        var damageDetails = new DamageDetails()
        {
            Fainted = false,
            Critical = critical,
            TypeEffectiveness = type
        };

        int damage = Mathf.FloorToInt(move.MoveData.Power * (attacker.Attack / Defense) * critical * type * 0.1f);
        Debug.Log($"{attacker._characterData.Name} did {damage} damage to {_characterData.Name}");

        UpdateHp(damage);

        return damageDetails;
    }
    public bool CanAttackerFinishSelf(TBCharacter attacker)
    {
        bool canFinish = false;
        foreach (var move in _moves)
        {
            float critical = 1f;
            if (UnityEngine.Random.value * 100f <= move.MoveData.CriticalChance)
            {
                critical = 2f;
            }

            float type = TypeChart.GetEffectiveness(move.MoveData.Element, _characterData.Element);

            int damage = Mathf.FloorToInt(move.MoveData.Power * (attacker.Attack / Defense) * critical * type * 0.1f);

            if (damage >= _currentHP)
            {
                canFinish = true;
                break;
            }
        }
        return canFinish;
    }
    public void LearnMove(LearnableMove moveToLearn)
    {
        if (_moves.Count >= 4)
            return;
        PlayerStats.Moves.Add(new TBMove(moveToLearn.MoveData));
        Debug.Log($"{_characterData.Name} has learned {moveToLearn.MoveData.MoveName}");
    }
    public TBMove GetRandomMove()
    {
        return _moves[UnityEngine.Random.Range(0, _moves.Count)];
    }
    public void UpdateHp(int damage)
    {
        if (damage == 0)
            damage = 1;

        _currentHP = Mathf.Clamp(_currentHP - damage, 0, MaxHp);

        if (!_characterData.IsEnemy) PlayerStats.CurrentHealthPoints = _currentHP;

        _hpChanged = true;
    }
    public void setStats()
    {
        if (!_characterData.IsEnemy)
        {
            MaxHp = PlayerStats.MaxHealthPoints;
            _currentHP = Mathf.Clamp(PlayerStats.CurrentHealthPoints, 0, MaxHp);
            _stats[Stat.Attack] = Mathf.FloorToInt(PlayerStats.TBAttackPower * _level);
            _stats[Stat.Defense] = Mathf.FloorToInt(PlayerStats.TBDefensePower * _level);
            _stats[Stat.Speed] = Mathf.FloorToInt(PlayerStats.TBAttackSpeed * _level);
            _hpChanged = true;
        }
    }
    public void OnAfterTurn()
    {
        foreach (var status in _statuses)
        {
            //This will avoid the soaked condition to be applied to the character more than once
            if (status != ConditionsDB.Conditions[ConditionID.Soaked])
                status?.OnEffectAppliedToCharacter?.Invoke(this);
        }
    }
    public void OnBeforeMove()
    {
        List<Condition> conditions = new List<Condition>();
        foreach (var status in _statuses)
        {
            //This action returns a condition that will be removed from the player's statuses
            conditions.Add(status?.OnBeforeCharacterMove?.Invoke(this));
        }

        if (conditions.Count > 0)
        {
            foreach (var condition in conditions)
            {
                if (condition != null)
                {
                    RemoveStatus(condition.ID);
                }
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

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
    private List<Condition> _statuses = new List<Condition>();
    private Queue<string> _statusChanges = new Queue<string>();
    public event System.Action OnStatusChanged;
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

    public TBCharacter(TBCharacterData characterData, int level)
    {
        _characterData = characterData;
        _level = level;
        CalculateStats();

        _moves = new List<TBMove>();
        foreach (LearnableMove move in _characterData.LearnableMoves)
        {
            if (move.Level <= level)
            {
                _moves.Add(new TBMove(move.Move));
            }

            if (_moves.Count >= 4)
            {
                break;
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
            _currentHP = _characterData.MaxHealthPoints;
            _stats = new Dictionary<Stat, int>
            {
                { Stat.Attack, Mathf.FloorToInt(_characterData.AttackPower * _level) },
                { Stat.Defense, Mathf.FloorToInt(_characterData.DefensePower * _level) },
                { Stat.Speed, Mathf.FloorToInt(_characterData.AttackSpeed * _level) }
            };
            MaxHp = _characterData.MaxHealthPoints;
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

            this._statBoosts[stat] = Mathf.Clamp(this._statBoosts[stat] + boost, -6, 6);

            if (boost > 0)
            {
                _statusChanges.Enqueue($"{_characterData.Name}'s {stat} increased!");
            }
            else
            {
                _statusChanges.Enqueue($"{_characterData.Name}'s {stat} decreased!");
            }

            Debug.Log(stat + " has been boosted to " + this._statBoosts[stat]);
        }
    }
    public DamageDetails TakeDamage(TBMove move, TBCharacter attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= move.MoveData.CriticalChance)
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
        //IMPROVE THIS DAMAGE FORMULA LATER
        //int damage = Mathf.FloorToInt(move.MoveData.Power * (attacker.Attack / Defense));
        int damage = Mathf.FloorToInt(move.MoveData.Power * (attacker.Attack / Defense) * critical * type);

        UpdateHp(damage);

        return damageDetails;
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
        _hpChanged = true;
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

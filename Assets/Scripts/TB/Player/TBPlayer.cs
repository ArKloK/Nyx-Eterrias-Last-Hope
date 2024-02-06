using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBPlayer
{
    public TBPlayerData playerData;
    public int level;
    public int currentHp;
    public bool hpChanged;
    public int statusTime;
    public List<TBMove> moves;
    public TBMove CurrentMove { get; set; }
    private Dictionary<Stat, int> stats;
    private Dictionary<Stat, int> statBoosts;
    public List<Condition> statuses = new List<Condition>();
    public Queue<string> statusChanges = new Queue<string>();
    public event Action OnStatusChanged;

    public TBPlayer(TBPlayerData playerData, int level)
    {
        this.playerData = playerData;
        this.level = level;
        CalculateStats();
        currentHp = PlayerStats.CurrentHealthPoints;

        moves = new List<TBMove>();
        foreach (LearnableMove move in playerData.LearnableMoves)
        {
            if (move.Level <= level)
            {
                moves.Add(new TBMove(move.Move));
            }

            if (moves.Count >= 4)
            {
                break;
            }
        }

        ResetStatBoosts();
    }

    public void ResetStatBoosts()
    {
        statBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.Speed, 0 },
        };
    }

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
            stats[Stat.Speed] = value;
        }
    }

    public int MaxHp
    {
        get; private set;
    }

    public int GetStat(Stat stat)
    {
        int statValue = stats[stat];

        int boost = statBoosts[stat];
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

            this.statBoosts[stat] = Mathf.Clamp(this.statBoosts[stat] + boost, -6, 6);

            if (boost > 0)
            {
                statusChanges.Enqueue($"{playerData.Name}'s {stat} increased!");
            }
            else
            {
                statusChanges.Enqueue($"{playerData.Name}'s {stat} decreased!");
            }

            Debug.Log(stat + " has been boosted to " + this.statBoosts[stat]);
        }
    }

    public DamageDetails TakeDamage(TBMove move, TBEnemy attacker)
    {
        float critical = 1f;
        if (UnityEngine.Random.value * 100f <= move.MoveData.CriticalChance)
        {
            Debug.Log("Enemy did Critical");
            critical = 2f;
        }
        float type = TypeChart.GetEffectiveness(move.MoveData.Element, PlayerStats.Element);
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

    void CalculateStats()
    {
        stats = new Dictionary<Stat, int>
        {
            { Stat.Attack, Mathf.FloorToInt(PlayerStats.TBAttackPower * level) },
            { Stat.Defense, Mathf.FloorToInt(PlayerStats.TBDefensePower * level) },
            { Stat.Speed, Mathf.FloorToInt(PlayerStats.TBAttackSpeed * level) }
        };

        MaxHp = PlayerStats.MaxHealthPoints;
    }

    public void AddStatus(ConditionID conditionId)
    {
        var condition = ConditionsDB.Conditions[conditionId];
        if (statuses.Contains(condition))
        {
            statusChanges.Enqueue($"{playerData.Name} {condition.RepeatedMovementMessage}");
            return;
        }
        statuses.Add(condition);

        //Statuses like soaked will be applied immediately and only once
        if (conditionId.Equals(ConditionID.Soaked))
        {
            condition.OnEffectAppliedToPlayer?.Invoke(this);
        }
        statusChanges.Enqueue($"{playerData.Name} {condition.StartMessage}");
        OnStatusChanged?.Invoke();
    }
    public void RemoveStatus(ConditionID conditionId)
    {
        var condition = statuses.Find(status => status == ConditionsDB.Conditions[conditionId]);
        if (condition != null)
        {
            statuses.Remove(condition);
        }
        OnStatusChanged?.Invoke();
    }

    public void UpdateHp(int damage)
    {
        if (damage == 0)
            damage = 1;

        currentHp = Mathf.Clamp(currentHp - damage, 0, MaxHp);
        PlayerStats.CurrentHealthPoints = currentHp;
        hpChanged = true;
    }

    public void OnAfterTurn()
    {
        foreach (var status in statuses)
        {
            //This will avoid the soaked condition to be applied to the player more than once
            if (status != ConditionsDB.Conditions[ConditionID.Soaked])
                status?.OnEffectAppliedToPlayer?.Invoke(this);
        }
    }

    //This method will remove all the statuses that have expired
    public void OnBeforeMove()
    {
        List<Condition> conditions = new List<Condition>();
        foreach (var status in statuses)
        {
            //This action returns a condition that will be removed from the player's statuses
            conditions.Add(status?.OnBeforePlayerMove?.Invoke(this));
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

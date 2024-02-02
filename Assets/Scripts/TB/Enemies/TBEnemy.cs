using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class TBEnemy
{
    public TBEnemyData enemyData;
    public int currentHp;
    public int level;
    public bool hpChanged;
    public List<TBMove> moves;
    private Dictionary<Stat, int> stats;
    private Dictionary<Stat, int> statBoosts;
    public Condition status;
    public Queue<string> statusChanges = new Queue<string>();


    public TBEnemy(TBEnemyData enemyData, int level)
    {
        this.enemyData = enemyData;
        this.level = level;
        CalculateStats();
        currentHp = MaxHp;

        moves = new List<TBMove>();
        foreach (TBMoveData move in enemyData.Moves)
        {
            moves.Add(new TBMove(move));
        }

        ResetStatBoosts();
    }

    public void ResetStatBoosts()
    {
        statBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.Speed, 0 }
        };
    }

    public int Attack
    {
        get
        {
            return GetStat(Stat.Attack);
        }
    }

    public int Defense
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
    }

    public int MaxHp
    {
        get; private set;
    }

    public int GetStat(Stat stat)
    {
        int statValue = stats[stat];

        //TODO: Apply stat modifiers
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
                statusChanges.Enqueue($"{enemyData.Name}'s {stat} increased!");
            }
            else
            {
                statusChanges.Enqueue($"{enemyData.Name}'s {stat} decreased!");
            }

            Debug.Log(stat + " has been boosted to " + this.statBoosts[stat]);
        }
    }

    public DamageDetails TakeDamage(TBMove move, TBPlayer attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= move.MoveData.CriticalChance)
        {
            Debug.Log("Player did Critical");
            critical = 2f;
        }
        float type = TypeChart.GetEffectiveness(move.MoveData.Element, enemyData.element);
        var damageDetails = new DamageDetails()
        {
            Fainted = false,
            Critical = critical,
            TypeEffectiveness = type
        };
        //IMPROVE THIS DAMAGE FORMULA LATER
        //int damage = Mathf.FloorToInt(move.MoveData.Power * (attacker.Attack / Defense));
        int damage = Mathf.FloorToInt(move.MoveData.Power * (attacker.Attack / Defense) * type * critical);
        currentHp -= damage;

        UpdateHp(damage);

        return damageDetails;
    }

    //This method will be replaced with a more intelligent AI later
    public TBMove GetRandomMove()
    {
        return moves[Random.Range(0, enemyData.Moves.Count)];
    }

    void CalculateStats()
    {
        stats = new Dictionary<Stat, int>
        {
            { Stat.Attack, Mathf.FloorToInt(enemyData.AttackPower * level) },
            { Stat.Defense, Mathf.FloorToInt(enemyData.DefensePower * level) },
            { Stat.Speed, Mathf.FloorToInt(enemyData.AttackSpeed * level) }
        };

        MaxHp = enemyData.MaxHealthPoints;
    }

    public void SetStatus(ConditionID conditionId)
    {
        status = ConditionsDB.Conditions[conditionId];
        statusChanges.Enqueue($"{enemyData.Name} {status.StartMessage}");
    }

    public void UpdateHp(int damage)
    {
        if (damage == 0)
            damage = 1;

        currentHp = Mathf.Clamp(currentHp - damage, 0, MaxHp);
        hpChanged = true;
    }

    public void OnAfterTurn()
    {
        status?.OnEffectAppliedToEnemy?.Invoke(this);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBPlayer
{
    public TBPlayerData playerData;
    public int level;
    public int currentHp;
    public List<TBMove> moves;
    private Dictionary<Stat, int> stats;
    private Dictionary<Stat, int> statBoosts;

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

        statBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.Speed, 0 }
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

    public float Speed
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

            Debug.Log(stat + " has been boosted to " + this.statBoosts[stat]);
        }
    }

    public DamageDetails TakeDamage(TBMove move, TBEnemy attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= move.MoveData.CriticalChance)
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
        currentHp -= damage;
        PlayerStats.CurrentHealthPoints = currentHp;
        if (currentHp <= 0)
        {
            currentHp = 0;
            damageDetails.Fainted = true;
        }
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
}

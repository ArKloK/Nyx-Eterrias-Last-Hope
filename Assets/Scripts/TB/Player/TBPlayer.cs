using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBPlayer
{
    public TBPlayerData playerData;
    public int level;
    public int currentHp;
    public List<TBMove> moves;

    public TBPlayer(TBPlayerData playerData, int level)
    {
        this.playerData = playerData;
        this.level = level;
        this.currentHp = PlayerStats.CurrentHealthPoints;

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
    }

    public float Attack
    {
        get
        {
            return PlayerStats.TBAttackPower * level;
        }
    }

    public float Defense
    {
        get
        {
            return PlayerStats.TBDefensePower * level;
        }
    }

    public float Speed
    {
        get
        {
            return PlayerStats.TBAttackSpeed * level;
        }
    }

    public int MaxHp
    {
        get
        {
            return PlayerStats.MaxHealthPoints;
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
        //Improve the damage formula later
        //int damage = Mathf.FloorToInt(move.MoveData.Power * (attacker.Attack / Defense));
        int damage = Mathf.FloorToInt(move.MoveData.Power * critical * type);
        currentHp -= damage;
        PlayerStats.CurrentHealthPoints = currentHp;
        if (currentHp <= 0)
        {
            currentHp = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

}

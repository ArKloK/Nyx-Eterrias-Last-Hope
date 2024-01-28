using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class TBEnemy
{
    public TBEnemyData enemyData;
    public int currentHp;
    public int level;
    public List<TBMove> moves;
    public TBEnemy(TBEnemyData enemyData, int level)
    {
        this.enemyData = enemyData;
        this.level = level;
        currentHp = enemyData.MaxHealthPoints;
        moves = new List<TBMove>();
        foreach (TBMoveData move in enemyData.Moves)
        {
            moves.Add(new TBMove(move));
        }
    }

    public int Attack
    {
        get
        {
            return enemyData.AttackPower * level;
        }
    }

    public int Defense
    {
        get
        {
            return enemyData.DefensePower * level;
        }
    }

    public int Speed
    {
        get
        {
            return enemyData.AttackSpeed * level;
        }
    }

    public int MaxHp
    {
        get
        {
            return enemyData.MaxHealthPoints;
        }
    }

    public bool TakeDamage(TBMove move, TBPlayer attacker)
    {
        //Improve the damage formula later
        //int damage = Mathf.FloorToInt(move.MoveData.Power * (attacker.Attack / Defense));
        int damage = Mathf.FloorToInt(move.MoveData.Power);
        currentHp -= damage;
        if (currentHp <= 0)
        {
            currentHp = 0;
            return true;
        }
        return false;
    }

    //This method will be replaced with a more intelligent AI later
    public TBMove GetRandomMove()
    {
        return moves[Random.Range(0, enemyData.Moves.Count)];
    }

}

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
        currentHp = GameObject.Find("PLAYER").GetComponent<PlayerController>().CurrentHealthPoints;

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

    public int Attack
    {
        get
        {
            return playerData.AttackPower * level;
        }
    }

    public int Defense
    {
        get
        {
            return playerData.DefensePower * level;
        }
    }

    public int Speed
    {
        get
        {
            return playerData.AttackSpeed * level;
        }
    }

    public int MaxHp
    {
        get
        {
            return playerData.MaxHealthPoints * level;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats
{
    public static Element Element;
    public static int MaxHealthPoints;
    public static int CurrentHealthPoints;
    public static int MaxSpiritualEnergyPoints;
    public static int CurrentSpiritualEnergyPoints;
    public static int AttackPower;
    public static int TBAttackSpeed;
    public static int TBDefensePower;
    public static int TBAttackPower;
    public static int CurrentExperiencePoints;
    public static int MaxExperiencePoints;
    public static int CurrentLevel;
    public static List<LearnableMove> LearnableMoves;
    public static List<TBMove> Moves;
    public static LearnableMove GetLearnableMovesAtCurrentLevel()
    {
        return LearnableMoves.Find(x => x.Level == CurrentLevel);
    }
    public static void SetMoves()
    {
        Moves = new List<TBMove>();
        foreach (LearnableMove move in LearnableMoves)
        {
            if (move.Level <= CurrentLevel)
            {
                Moves.Add(new TBMove(move.Move));
            }
            if (Moves.Count >= 4)
            {
                break;
            }
        }
    }
}

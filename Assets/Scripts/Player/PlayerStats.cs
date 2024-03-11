using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats
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
                Moves.Add(new TBMove(move.MoveData));
            }
            if (Moves.Count >= 4)
            {
                break;
            }
        }
    }

    #region Save/Load Methods

    //This method returns the moves that the player can learn at any level
    public static List<TBMove> GetMovesLearnedAtLevel(int level)
    {
        if(LearnableMoves == null)
        {
            LearnableMoves = new List<LearnableMove>();
        }
        List<TBMove> moves = LearnableMoves.Where(x => x.Level == level).Select(move => new TBMove(move.MoveData)).ToList();
        return moves;
    }

    //This method returns the indexes of the moves that the player can learn at a specific level
    public static int[] GetMovesIndexesLearnedAtLevel(int level)
    {
        List<TBMove> moves = GetMovesLearnedAtLevel(level);
        int[] indexes = new int[4];
        if(moves.Count == 0)
        {
            return indexes;
        }
        for (int i = 0; i < 4; i++)
        {
            if (i < moves.Count)
            {
                indexes[i] = LearnableMoves.IndexOf(LearnableMoves.Find(x => x.MoveData == moves[i].MoveData));
            }
            else
            {
                indexes[i] = -1;
            }
        }
        return indexes;
    }

    //This method is used to get the indexes of the current moves inside the LearnableMoves list to save them
    public static int[] GetMovesIndexesInLearnableMoves()
    {
        int[] indexes = new int[4];
        int counter = 0;
        foreach (LearnableMove learnableMove in LearnableMoves)
        {
            if (Moves.Find(x => x.MoveData == learnableMove.MoveData) != null)
            {
                int i = LearnableMoves.IndexOf(learnableMove);
                indexes[counter++] = i;
            }
        }
        return indexes;
    }

    //This method is used to load the moves from the saved indexes
    public static void SetMovesFromLearnableMovesIndex(int[] indexes)
    {
        Moves = new List<TBMove>();
        for (int i = 0; i < 4; i++)
        {
            if (indexes[i] != -1)
            {
                Moves.Add(new TBMove(LearnableMoves[indexes[i]].MoveData));
            }
        }
    }

    public static void StaticLoadData(GameData gameData)
    {
        Element = (Element)gameData.element;
        MaxHealthPoints = gameData.maxHealthPoints;
        CurrentHealthPoints = gameData.currentHealthPoints;
        MaxSpiritualEnergyPoints = gameData.maxSpiritualEnergyPoints;
        CurrentSpiritualEnergyPoints = gameData.currentSpiritualEnergyPoints;
        AttackPower = gameData.attackPower;
        TBAttackSpeed = gameData.tBAttackSpeed;
        TBDefensePower = gameData.tBDefensePower;
        TBAttackPower = gameData.tBAttackPower;
        CurrentExperiencePoints = gameData.currentExperiencePoints;
        MaxExperiencePoints = gameData.maxExperiencePoints;
        CurrentLevel = gameData.currentLevel;
        SetMovesFromLearnableMovesIndex(gameData.moves);
    }

    public static void StaticSaveData(GameData gameData)
    {
        gameData.element = (int)Element;
        gameData.maxHealthPoints = MaxHealthPoints;
        gameData.currentHealthPoints = CurrentHealthPoints;
        gameData.maxSpiritualEnergyPoints = MaxSpiritualEnergyPoints;
        gameData.currentSpiritualEnergyPoints = CurrentSpiritualEnergyPoints;
        gameData.attackPower = AttackPower;
        gameData.tBAttackSpeed = TBAttackSpeed;
        gameData.tBDefensePower = TBDefensePower;
        gameData.tBAttackPower = TBAttackPower;
        gameData.currentExperiencePoints = CurrentExperiencePoints;
        gameData.maxExperiencePoints = MaxExperiencePoints;
        gameData.currentLevel = CurrentLevel;
        gameData.moves = GetMovesIndexesInLearnableMoves();
    }
    #endregion

}

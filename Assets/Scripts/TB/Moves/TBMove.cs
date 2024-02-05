using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBMove
{
    public TBMoveData MoveData;
    private int accuracy;
    public int Accuracy { get => accuracy; set => accuracy = value; }

    public TBMove(TBMoveData moveData)
    {
        MoveData = moveData;
        accuracy = moveData.Accuracy;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    public TBPlayerData playerData;
    public TBPlayer player;

    public void setData()
    {
        player = new TBPlayer(playerData, PlayerStats.CurrentLevel);
    }
}

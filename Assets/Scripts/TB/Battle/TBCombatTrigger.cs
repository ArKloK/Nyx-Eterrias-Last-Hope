using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBCombatTrigger : MonoBehaviour
{
    public GameController gameController;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            this.gameObject.SetActive(false);
            gameController.StartBattle();
        }
    }
}

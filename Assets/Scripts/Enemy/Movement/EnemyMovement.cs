using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 1f;
    public Rigidbody2D rb;
    public Vector2 movement;

    // Update is called once per frame
    void Update()
    {
        //Move the enemy
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}

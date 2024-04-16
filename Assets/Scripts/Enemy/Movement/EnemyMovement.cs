using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] EnemyMovementData Data;
    private Rigidbody2D rb;
    private BoxCollider2D _col;
    private bool grounded;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        grounded = Physics2D.BoxCast(_col.bounds.center, _col.size, 0, Vector2.down, 0.1f, Data.EnemyLayer);
        Debug.Log(grounded);
    }
}

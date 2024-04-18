using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] EnemyMovementData Data;
    private Rigidbody2D rb;
    private BoxCollider2D _col;
    private bool _grounded;
    private Transform currentTarget;

    [SerializeField] Transform Playertarget;
    [SerializeField] Transform firstPatrolPoint;
    [SerializeField] Transform secondPatrolPoint;

    [SerializeField] float speed;
    [SerializeField] float nextWaypointDistance;
    [SerializeField] float distanceToFollowPlayer;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
        currentTarget = firstPatrolPoint;

        InvokeRepeating("UpdatePath", 0f, 0.2f);
    }

    void Update()
    {
        if (Vector2.Distance(Playertarget.position, transform.position) < distanceToFollowPlayer)
        {
            currentTarget = Playertarget;
        }
        else
        {
            Debug.Log("Patrolling");
            if (currentTarget == null || currentTarget == Playertarget)
            {
                currentTarget = firstPatrolPoint;
            }

            if (Vector2.Distance(currentTarget.position, transform.position) <= 1f)
            {
                if (currentTarget == firstPatrolPoint)
                {
                    currentTarget = secondPatrolPoint;
                }
                else
                {
                    currentTarget = firstPatrolPoint;
                }
            }
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();
        Debug.Log(_grounded);
        if (!_grounded)
        {
            Fall();
        }
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    public void KnockBack(Vector2 direction)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * Data.KnockBackForce, ForceMode2D.Impulse);
    }

    public void Fall()
    {
        if (!_grounded)
        {
            rb.AddForce(Vector2.down * Data.FallAcceleration, ForceMode2D.Force);
            Vector2 aux = new Vector2();
            aux.y = Mathf.MoveTowards(rb.velocity.y, -Data.MaxFallSpeed, Data.FallAcceleration * Time.fixedDeltaTime);
            rb.velocity = aux;
            Debug.Log("Falling");
        }
    }

    void CheckGrounded()
    {
        _grounded = Physics2D.BoxCast(_col.bounds.center, _col.bounds.size, 0f, Vector2.down, 0.1f, Data.GroundLayer);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, currentTarget.position, OnPathComplete);
        }
    }
}

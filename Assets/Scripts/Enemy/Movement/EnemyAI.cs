using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] EnemyMovementData Data;
    private Rigidbody2D rb;
    private BoxCollider2D _col;
    private bool _grounded;
    private Transform currentTarget;
    private bool canMove = true;
    private bool runAway = false;
    private float speed;

    [SerializeField] Transform Playertarget;
    [SerializeField] Transform firstPatrolPoint;
    [SerializeField] Transform secondPatrolPoint;

    [SerializeField] float standardSpeed;
    [SerializeField] float nextWaypointDistance;
    [SerializeField] float distanceToFollowPlayer;

    Path path;
    int currentWaypoint = 0;
    //bool reachedEndOfPath = false;

    Seeker seeker;

    public float Speed { get => standardSpeed; set => standardSpeed = value; }
    public bool CanMove { get => canMove; set => canMove = value; }

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
        currentTarget = firstPatrolPoint;

        InvokeRepeating("UpdatePath", 0f, 0.2f); //Invoke the UpdatePath method every 0.2 seconds
    }
    void OnEnable()
    {
        PauseMenuController.OnPause += HandlePause;
        PauseMenuController.OnResume += HandleResume;
    }

    void Update()
    {
        if (canMove) CheckTarget();
        if(GetComponent<EnemyController>().CurrentHealthPoints <= GetComponent<EnemyController>().Data.MaxHealthPoints / 2)
        {
            runAway = true;
        }
        else
        {
            runAway = false;
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();
        if (!_grounded)
        {
            Fall();
        }
        Debug.Log("CanMove: " + canMove);
        ApplyMovement();
    }

    public void KnockBack(GameObject sender)
    {
        canMove = false;
        Vector2 direction = new Vector2((transform.position - sender.transform.position).normalized.x, 1f);
        rb.AddForce(direction * Data.KnockBackForce, ForceMode2D.Impulse);
        StartCoroutine(LoseControl());
    }

    IEnumerator LoseControl()
    {
        yield return new WaitForSeconds(Data.KnockBackTime);
        rb.velocity = Vector2.zero;
        canMove = true;
    }

    public void Fall()
    {
        if (!_grounded && canMove)
        {
            rb.AddForce(Vector2.down * Data.FallAcceleration, ForceMode2D.Force);
            Vector2 aux = new Vector2();
            aux.y = Mathf.MoveTowards(rb.velocity.y, -Data.MaxFallSpeed, Data.FallAcceleration * Time.fixedDeltaTime);
            rb.velocity = aux;
        }
    }

    void CheckGrounded()
    {
        _grounded = Physics2D.BoxCast(_col.bounds.center, _col.bounds.size, 0f, Vector2.down, 0.1f, Data.GroundLayer);
    }

    //Switches the target between the player and the patrol points
    void CheckTarget()
    {
        if (Vector2.Distance(Playertarget.position, transform.position) < distanceToFollowPlayer)
        {
            currentTarget = Playertarget;
        }
        else
        {
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
        if (seeker.IsDone() && canMove)
        {
            seeker.StartPath(rb.position, currentTarget.position, OnPathComplete);
        }
    }

    //Makes the enemy move towards the next waypoint
    void ApplyMovement()
    {
        if (path == null || !canMove) 
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            //reachedEndOfPath = true;
            return;
        }
        else
        {
            //reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        speed = GetComponent<FuzzyLogic>().SetEnemySpeed();
        Debug.Log("Speed: " + speed);
        
        Vector2 force;
        if (currentTarget == Playertarget)
            force = direction * speed * Time.deltaTime;
        else
            force = direction * standardSpeed * Time.deltaTime;

        if (runAway && currentTarget == Playertarget) force *= -1;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
    public void HandlePause()
    {
        canMove = false;
    }
    public void HandleResume()
    {
        canMove = true;
    }
}

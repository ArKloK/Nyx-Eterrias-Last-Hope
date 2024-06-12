using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class EnemyMovementController : MonoBehaviour
{
    public EnemyMovementData Data;
    private Rigidbody2D rb;
    private BoxCollider2D _col;
    private Transform currentTarget;
    private bool canMove = true;
    private bool isDialogueActive;
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

    Seeker seeker;

    public float Speed { get => standardSpeed; set => standardSpeed = value; }
    public bool CanMove { get => canMove; set => canMove = value; }
    public bool IsDialogueActive { get => isDialogueActive; set => isDialogueActive = value; }

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
        if (GetComponent<EnemyController>().CurrentHealthPoints <= GetComponent<EnemyController>().Data.MaxHealthPoints / 2)
        {
            runAway = true;
            rb.gravityScale = 1f;
        }
        else
        {
            runAway = false;
            rb.gravityScale = 0f;
        }
    }

    void FixedUpdate()
    {
        if (isDialogueActive)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (canMove)
            ApplyMovement();
    }

    public void KnockBack(Vector2 direction)
    {
        float knockBackPower = 16f;
        rb.AddForce(direction * knockBackPower, ForceMode2D.Impulse);
    }

    public IEnumerator LoseControl()
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(Data.KnockBackTime);
        canMove = true;
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
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        speed = GetComponent<FuzzyLogic>().SetEnemySpeed();
        Debug.Log("Speed: " + speed);

        Vector2 force;
        if (currentTarget == Playertarget)
        {
            force = direction * speed * 2 * Time.deltaTime;
            if (runAway) force *= -1;
            rb.AddForce(force);
        }
        else
        {
            force = direction * standardSpeed * Time.deltaTime;
            rb.velocity = force;
        }        

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

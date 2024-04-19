using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyMovementData : ScriptableObject
{
    public float MaxFallSpeed;
    public float FallAcceleration;
    public LayerMask EnemyLayer;
    public LayerMask GroundLayer;
    public Vector2 KnockBackForce;
    public float KnockBackTime;
}

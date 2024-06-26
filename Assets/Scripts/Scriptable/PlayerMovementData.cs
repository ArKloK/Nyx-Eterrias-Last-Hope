using UnityEngine;

namespace PlayerMovementController
{
    [CreateAssetMenu]
    public class PlayerMovementData : ScriptableObject
    {
        [Header("LAYERS")]
        [Tooltip("Set this to the layer your player is on")]
        public LayerMask PlayerLayer;
        [Tooltip("Set this to the layer the wall is on")]
        public LayerMask WallLayer;

        [Header("INPUT")]
        [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
        public bool SnapInput = true;

        [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
        public float VerticalDeadZoneThreshold = 0.3f;

        [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
        public float HorizontalDeadZoneThreshold = 0.1f;

        [Header("MOVEMENT")]
        [Tooltip("The top horizontal movement speed")]
        public float MaxSpeed = 14;

        [Tooltip("The player's capacity to gain horizontal speed")]
        public float Acceleration = 120;

        [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
        public float GroundingForce = -1.5f;

        [Tooltip("The detection distance for grounding and ceiling detection")]
        public float GrounderDistance = 0.1f;

        [Header("JUMP")]
        [Tooltip("The immediate velocity applied when jumping")]
        public float JumpPower = 36;

        [Tooltip("The maximum vertical movement speed")]
        public float MaxFallSpeed = 40;

        [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
        public float FallAcceleration = 110;

        [Tooltip("The gravity multiplier added when jump is released early")]
        public float JumpEndEarlyGravityModifier = 3;

        [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
        public float CoyoteTime = .15f;

        [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
        public float JumpBuffer = .2f;

        [Header("DOUBLE JUMP")]
        [Tooltip("The speed at which the player double jumps")]
        public float DoubleJumpHeight = 4f;

        [Header("WALL JUMP")]
        [Tooltip("The speed at which the player slides down a wall")]
        public float WallSlidingSpeed = 3f;
        [Tooltip("The time the player can jump after leaving a wall")]
        public float WallJumpingTime = 0.2f;
        [Tooltip("The duration of the wall jump")]
        public float WallJumpingDuration = 0.4f;
        [Tooltip("The radius of the wall check")]
        public float WallCheckRadius = 0.2f;
        [Tooltip("The power of the wall jump")]
        public Vector2 WallJumpingPower = new Vector2(8f, 16f);

        [Header("DASH")]
        [Tooltip("The speed at which the player dashes")]
        public float DashingPower = 20f;
        [Tooltip("The duration of the dash")]
        public float DashingTime = 0.2f;
        [Tooltip("The cooldown of the dash")]
        public float DashingCooldown = 0.5f;

        [Header("ATTACK")]
        [Tooltip("The cooldown of the attack")]
        public float AttackCooldown = 0.5f;

        [Header("KNOCKBACK DATA")]
        public Vector2 KnockBackPower;
    }
}
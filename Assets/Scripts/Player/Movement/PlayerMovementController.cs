using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace PlayerMovementController
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerMovementController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private PlayerMovementData Data;
        private Rigidbody2D _rb;
        private BoxCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        #region Checkers

        private bool _isFacingRight;
        bool groundHit;
        bool ceilingHit;
        public bool canMove = true;

        #endregion

        #region Abilities unlocked
        public bool doubleJumpUnlocked;
        #endregion

        #region Interface and Events

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<BoxCollider2D>();
            _isFacingRight = true;

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        public void HandleUpdate()
        {
            _time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            //If you're dashing or is hitted by an enemy, you can't move
            if (_isDashing || !canMove) return;

            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))

            };
            #region Movement And Jump Input
            if (Data.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < Data.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < Data.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
            #endregion
            #region Double Jump Input
            if (_grounded) _doubleJumpUsed = false;

            if (_frameInput.JumpDown && doubleJumpUnlocked)
            {
                if (!_grounded && !_doubleJumpUsed)
                {
                    _doubleJumpToConsume = true;
                }
            }
            #endregion
            #region Wall Jump Input

            if (_frameInput.JumpDown && wallJumpingCounter > 0)
            {
                isWallJumping = true;
            }
            #endregion
            #region Dash Input
            if (Input.GetButtonDown("Dash") && _canDash)
            {
                StartCoroutine(ExecuteDash());
            }
            #endregion
        }

        public void HandleFixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDoubleJump();
            HandleWallSlide();
            HandleWallJump();
            HandleDirection();
            HandleGravity();

            ApplyMovement();
        }

        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling
            groundHit = Physics2D.BoxCast(_col.bounds.center, _col.size, 0, Vector2.down, Data.GrounderDistance, ~Data.PlayerLayer);
            ceilingHit = Physics2D.BoxCast(_col.bounds.center, _col.size, 0, Vector2.up, Data.GrounderDistance, ~Data.PlayerLayer);

            // Hit a Ceiling
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // Landed on the Ground
            if (!_grounded && groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            // Left the Ground
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion


        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + Data.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + Data.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote) ExecuteJump();

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = Data.JumpPower;
            Jumped?.Invoke();
        }

        #endregion

        #region Double Jump
        private bool _doubleJumpUsed;
        private bool _doubleJumpToConsume;
        private void HandleDoubleJump()
        {
            if (!_doubleJumpToConsume || IsWalled()) return;
            ExectuteDoubleJump();
        }

        private void ExectuteDoubleJump()
        {
            _frameVelocity.y = Data.DoubleJumpHeight;
            _doubleJumpToConsume = false;
            _doubleJumpUsed = true;
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (!isWallJumping && !_isDashing && canMove)
            {

                if (_frameInput.Move.x != 0)
                {
                    _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * Data.MaxSpeed, Data.Acceleration * Time.fixedDeltaTime);
                    //_frameVelocity = new Vector2(_frameInput.Move.x * Data.MaxSpeed, _frameVelocity.y);
                    Turn();
                }
                else
                {
                    _frameVelocity = new Vector2(0, _frameVelocity.y);
                }
            }

        }

        #endregion

        #region Wall Jump


        [Tooltip("The object used to check if the player is touching a wall")]
        public Transform WallCheck;
        private bool turnOnWallJump;
        private bool isWallSliding;
        private bool isWallJumping;
        private float wallJumpingDirection;
        private float wallJumpingCounter;

        private void HandleWallJump()
        {
            if (isWallSliding)
            {
                isWallJumping = false;
                wallJumpingDirection = -transform.localScale.x;
                wallJumpingCounter = Data.WallJumpingTime;

                CancelInvoke(nameof(StopWallJumping));
            }
            else
            {
                wallJumpingCounter -= Time.deltaTime;
            }

            if (isWallJumping)
            {
                ExecuteWallJump();
            }
        }
        private void ExecuteWallJump()
        {
            _frameVelocity = new Vector2(wallJumpingDirection * Data.WallJumpingPower.x, Data.WallJumpingPower.y);
            wallJumpingCounter = 0;

            if (transform.localScale.x != wallJumpingDirection)
            {
                turnOnWallJump = true;
                Turn();
                turnOnWallJump = false;
            }

            Invoke(nameof(StopWallJumping), Data.WallJumpingDuration);

        }

        private void StopWallJumping()
        {
            _doubleJumpUsed = false;
            isWallJumping = false;
        }
        private bool IsWalled()
        {
            return Physics2D.OverlapCircle(WallCheck.position, Data.WallCheckRadius, Data.WallLayer);
        }
        private void HandleWallSlide()
        {
            if (IsWalled() && !_grounded)
            {
                isWallSliding = true;
                ExecuteWallSlide();
            }
            else
            {
                isWallSliding = false;
            }
        }
        private void ExecuteWallSlide()
        {
            _frameVelocity = new Vector2(_frameVelocity.x, Mathf.Clamp(_frameVelocity.y, -Data.WallSlidingSpeed, float.MaxValue));
        }

        #endregion

        #region Dash
        private bool _canDash = true;
        private bool _isDashing;

        private IEnumerator ExecuteDash()
        {
            _canDash = false;
            _isDashing = true;
            float originalGravity = _rb.gravityScale;
            _rb.gravityScale = 0f;
            _frameVelocity = new Vector2(transform.localScale.x * Data.DashingPower, 0f);
            yield return new WaitForSeconds(Data.DashingTime);
            _rb.gravityScale = originalGravity;
            _isDashing = false;
            yield return new WaitForSeconds(Data.DashingCooldown);
            _canDash = true;
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = Data.GroundingForce;
            }
            else
            {
                var inAirGravity = Data.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= Data.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -Data.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Auxiliar Methods
        private void Turn()
        {
            if (canMove && (_isFacingRight && _frameInput.Move.x < 0f || !_isFacingRight && _frameInput.Move.x > 0 || turnOnWallJump))
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                _isFacingRight = !_isFacingRight;
            }
        }

        public void KnockBack(Vector2 direction)
        {
            _frameVelocity = new Vector2(-Data.KnockBackPower.x * direction.x, Data.KnockBackPower.y);
        }
        #endregion

        private void ApplyMovement() => _rb.velocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Data == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace PlayerMovementController
{
    //[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerMovementController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private PlayerMovementData Data;
        [SerializeField] private ParticleSystem dust;
        [SerializeField] private TrailRenderer trailRenderer;
        private PlayerController playerController;
        private Rigidbody2D _rb;
        private BoxCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private Camera mainCamera;
        private bool _cachedQueryStartInColliders;

        #region Checkers
        private bool _isFacingRight;
        bool groundHit;
        bool ceilingHit;
        public bool canMove = true;
        public bool isDialogueActive;
        private bool isAttacking;
        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        #endregion

        #region Animation
        Animator animator;
        private string currentState;
        private string newAnimationState;
        //Animation States
        const string PLAYER_IDLE = "Idle";
        const string PLAYER_RUN = "Run";
        const string PLAYER_JUMP = "Jump";
        const string PLAYER_FALL = "Fall";
        const string PLAYER_DASH = "Dash";
        const string PLAYER_ATTACK = "Attack";
        #endregion

        #region Abilities unlocked
        public bool doubleJumpUnlocked;
        #endregion

        #region Interface and Events
        PlayerInputActions playerInputActions;
        PlayerInput playerInput;
        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<BoxCollider2D>();
            playerInput = GetComponent<PlayerInput>();
            _isFacingRight = true;

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

            InputManager.EnablePlayerInput();
            playerInputActions = InputManager.PlayerInputActions;

            playerInputActions.Player.Dash.started += Dash;

            playerInputActions.Player.Move.performed += Move;
            playerInputActions.Player.Move.canceled += Move;

            playerInputActions.Player.Jump.started += Jump;
            playerInputActions.Player.Jump.performed += Jump;
            playerInputActions.Player.Jump.canceled += Jump;

            playerInputActions.Player.Attack.started += Attack;

        }

        private void Start()
        {
            mainCamera = Camera.main;
            animator = GetComponent<Animator>();
            playerController = GetComponent<PlayerController>();
        }
        void OnEnable()
        {
            PauseMenuController.OnPause += HandlePause;
            PauseMenuController.OnResume += HandleResume;
        }
        void OnDisable()
        {
            PauseMenuController.OnPause -= HandlePause;
            PauseMenuController.OnResume -= HandleResume;
        }

        public void HandleUpdate()
        {
            _time += Time.deltaTime;
            GatherInput();
            ChangeAnimationState(newAnimationState);
        }
        public void HandleFixedUpdate()
        {
            CheckCollisions();

            CheckAnimations();

            if (canMove && !isAttacking)
            {
                HandleJump();
                if (doubleJumpUnlocked)
                    HandleDoubleJump();
                HandleWallSlide();
                HandleWallJump();
                HandleDirection();
            }
            else if ((!canMove && isDialogueActive) || isAttacking) //If the player can´t move, just apply gravity and stop the player
            {
                _frameInput.Move = Vector2.zero;
                _frameVelocity = new Vector2(0, _frameVelocity.y);
            }

            HandleGravity();

            ApplyMovement();

            //HandleMovementInCameraBounds();
        }

        #region Input System
        public void Move(InputAction.CallbackContext context)
        {
            if (canMove)
            {
                if (context.performed) _frameInput.Move.x = context.ReadValue<Vector2>().x;
                else if (context.canceled) _frameInput.Move.x = 0;
            }

        }
        public void Jump(InputAction.CallbackContext context)
        {
            if (groundHit || IsWalled())
            {
                if (context.started)
                {
                    _frameInput.JumpDown = true;
                }
                else if (context.performed)
                {
                    _frameInput.JumpHeld = true;
                }
            }
            else
            {
                if (context.started && !_doubleJumpUsed)
                {
                    _doubleJumpToConsume = true;
                }
            }

            if (context.canceled)
            {
                _frameInput.JumpDown = false;
                _frameInput.JumpHeld = false;
            }
        }
        public void Dash(InputAction.CallbackContext context)
        {
            if (context.started && _canDash && canMove && InputManager.PlayerInputActions.Player.enabled)
            {
                Debug.Log("Dash");
                StartCoroutine(ExecuteDash());
            }
        }
        public void Attack(InputAction.CallbackContext context)
        {
            Debug.Log("Attack");
            if (context.started && canMove && !_isDashing && attackCooldownTimer <= 0)
            {
                StartCoroutine(ExecuteAttack());
                attackCooldownTimer = Data.AttackCooldown;
            }
        }
        #endregion

        private void GatherInput()
        {
            //If you're dashing or is hitted by an enemy, you can't move
            if (_isDashing || !canMove) return;

            #region Movement And Jump Input
            if (Data.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < Data.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < Data.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                Debug.Log("Jump");
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
            #endregion
            #region Double Jump Input
            if (_grounded) _doubleJumpUsed = false;

            if (_frameInput.JumpDown && doubleJumpUnlocked && !_jumpToConsume)
            {
                if (!_grounded && !_doubleJumpUsed)
                {
                    Debug.Log("Double Jump");
                    _doubleJumpToConsume = true;
                    _jumpToConsume = false;
                }
            }
            #endregion
            #region Wall Jump Input

            if (_frameInput.JumpDown && wallJumpingCounter > 0)
            {
                isWallJumping = true;
                Debug.Log("Wall Jump");
            }
            #endregion
            #region Attack Input

            if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;

            #endregion
        }
        private float attackCooldownTimer = 0;
        private IEnumerator ExecuteAttack()
        {
            isAttacking = true;
            Debug.Log("Attacking");
            playerController.Attack();
            yield return new WaitForSeconds(Data.AttackCooldown);
            isAttacking = false;
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
            dust.Play();
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
                    Flip();
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
                wallJumpingDirection = transform.localScale.x;
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
                Flip();
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
        // void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireSphere(WallCheck.position, Data.WallCheckRadius);
        // }
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
            trailRenderer.emitting = true;
            yield return new WaitForSeconds(Data.DashingTime);
            trailRenderer.emitting = false;
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
        private void Flip()
        {
            if (canMove && (_isFacingRight && _frameInput.Move.x < 0f || !_isFacingRight && _frameInput.Move.x > 0 || turnOnWallJump) && !isAttacking)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                _isFacingRight = !_isFacingRight;
                if (_grounded) dust.Play();
            }
        }

        public void KnockBack(Vector2 direction)
        {
            Debug.Log("KnockBack direction: " + direction);
            _frameVelocity = new Vector2(-Data.KnockBackPower.x * direction.x, Data.KnockBackPower.y);
        }

        void ChangeAnimationState(string newState)
        {
            if (currentState == newState) return;

            animator.Play(newState);
            currentState = newState;
            //Debug.Log("Current State: " + currentState);
        }
        void CheckAnimations()
        {
            if (isAttacking)
            {
                newAnimationState = PLAYER_ATTACK;
            }
            else if (_isDashing)
            {
                newAnimationState = PLAYER_DASH;
            }
            else
            {
                if (groundHit)
                {
                    if (_frameInput.Move.x != 0)
                    {
                        if (!isDialogueActive)//If the player is in a dialogue, always play the idle animation
                            newAnimationState = PLAYER_RUN;
                        else
                            newAnimationState = PLAYER_IDLE;
                    }
                    else
                    {
                        newAnimationState = PLAYER_IDLE;
                    }
                }
                else
                {
                    if (_rb.velocity.y < 0)
                    {
                        newAnimationState = PLAYER_FALL;
                    }
                    else if (_rb.velocity.y > 0)
                    {
                        newAnimationState = PLAYER_JUMP;
                    }
                }
            }
        }
        private void HandleMovementInCameraBounds()
        {
            Vector3 playerScreenPosition = mainCamera.WorldToScreenPoint(transform.position);

            float screenEdgeBuffer = 80f;

            // Obtiene los límites de la pantalla con un margen adicional
            float minX = 0 + screenEdgeBuffer;
            float maxX = Screen.width - screenEdgeBuffer;
            float minY = 0 + screenEdgeBuffer;
            float maxY = Screen.height - screenEdgeBuffer;

            // Limita la posición del jugador dentro de los límites de la pantalla con el margen adicional
            playerScreenPosition.x = Mathf.Clamp(playerScreenPosition.x, minX, maxX);
            playerScreenPosition.y = Mathf.Clamp(playerScreenPosition.y, minY, maxY);

            // Convierte la posición de la pantalla nuevamente a la posición en el mundo
            Vector3 clampedPlayerPosition = mainCamera.ScreenToWorldPoint(playerScreenPosition);

            // Mantén la misma altura del jugador
            clampedPlayerPosition.z = transform.position.z;

            // Aplica la nueva posición al jugador
            transform.position = clampedPlayerPosition;
        }
        private void HandlePause()
        {
            canMove = false;
            InputManager.EnableUIInput();
        }
        private void HandleResume()
        {
            StartCoroutine(EnablePlayerInputWithDelay());
            canMove = true;
        }

        private IEnumerator EnablePlayerInputWithDelay()
        {
            yield return new WaitForSeconds(0.1f); // Un breve retraso para liberar inputs residuales
            InputManager.EnablePlayerInput();
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
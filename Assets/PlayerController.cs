using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerData Data;

    [Header("Horizontal Movement")]
    public float velocity = 5f;

    private Vector2 _moveInput;

    public float LastOnGroundTime { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsFacingRight { get; private set; }
    public bool IsWallJumping { get; private set; }

    private bool _isJumpFalling;

    [Header("Jump System")]
    public float jumpForce = 10f;
    public float jumpTime;
    public float fallmult;
    public float jumpMultiplier;
    private Rigidbody2D RB;
    public LayerMask groundLayer;
    public Transform groundCheck;
    private Vector2 vecGravity;
    bool isJumping;
    float jumpCounter;


    // Start is called before the first frame update
    void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        RB = GetComponent<Rigidbody2D>();
        IsFacingRight = true;
    }

    void Update()
    {

        _moveInput.x = Input.GetAxisRaw("Horizontal");

        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);


        if (isGrounded() && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        //Apply gravity
        if (RB.velocity.y < 0)
        {
            RB.velocity -= vecGravity * fallmult * Time.deltaTime;
        }

        //Apply jump multiplier
        if (RB.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.deltaTime;
            if (jumpCounter > jumpTime)
            {
                isJumping = false;
            }

            float t = jumpCounter / jumpTime;
            float currentJumpM = jumpMultiplier;

            if (t > 0.5f)
            {
                currentJumpM = jumpMultiplier * (1 - t);
            }

            RB.velocity += vecGravity * currentJumpM * Time.deltaTime;
        }

        //Reset jump
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
            jumpCounter = 0;
            if (RB.velocity.y > 0)
            {
                RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * 0.6f);
            }
        }
    }

    void FixedUpdate()
    {
        Run(1);
    }
    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }
    void Jump()
    {
        //This is for the double jump
        if (RB.velocity.y < 0)
        {
            jumpForce -= RB.velocity.y;
        }

        RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
        isJumping = true;
        jumpCounter = 0;
    }

    void MovePlayer()
    {
        //Get the player's current position
        Vector3 position = transform.position;

        //Get the player's input
        float horizontalInput = Input.GetAxis("Horizontal");

        //Calculate the player's new position
        position.x += horizontalInput * velocity * Time.deltaTime;

        //Move the player to the new position
        transform.position = position;
    }
    bool isGrounded()
    {
        //Check if the player is touching the ground
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.02f, 0.07f), CapsuleDirection2D.Horizontal, 0f, groundLayer);
    }
    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }
}

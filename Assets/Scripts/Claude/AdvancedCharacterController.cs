using UnityEngine;

public class AdvancedCharacterController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public float wallSlideSpeed = 2f;
    public float wallJumpForce = 6f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;

    [Header("Movement States")]
    public bool isGrounded;
    public bool isWallSliding;
    public bool isSprinting;

    private Rigidbody2D rb;
    private CharacterStats characterStats;
    private bool facingRight = true;
    private bool canWallJump = true;

    // Thêm phương thức mới
    public void SetMovementInput(Vector2 input)
    {
        // Xử lý input di chuyển
        float moveHorizontal = input.x;
        rb.linearVelocity = new Vector2(
            moveHorizontal * moveSpeed * characterStats.movementSpeed,
            rb.linearVelocity.y
        );
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * characterStats.jumpHeight);
        }
    }

    public void EnableSprint()
    {
        isSprinting = true;
    }

    public void DisableSprint()
    {
        isSprinting = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        characterStats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJumping();
        HandleWallSlide();
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float actualMoveSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        rb.linearVelocity = new Vector2(
            moveHorizontal * actualMoveSpeed * characterStats.movementSpeed,
            rb.linearVelocity.y
        );

        // Flip character
        if (moveHorizontal > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveHorizontal < 0 && facingRight)
        {
            Flip();
        }
    }

    void HandleJumping()
    {
        // Normal Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * characterStats.jumpHeight);
        }

        // Wall Jump
        if (isWallSliding && Input.GetButtonDown("Jump") && canWallJump)
        {
            WallJump();
        }
    }

    void HandleWallSlide()
    {
        bool isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, wallLayer);

        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;
        }
    }

    void WallJump()
    {
        // Determine wall jump direction
        Vector2 wallJumpDirection = facingRight ? Vector2.left : Vector2.right;

        rb.linearVelocity = new Vector2(
            wallJumpDirection.x * wallJumpForce,
            wallJumpForce * characterStats.jumpHeight
        );

        Flip();
        canWallJump = false;
        Invoke("ResetWallJump", 0.5f);
    }

    void ResetWallJump()
    {
        canWallJump = true;
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    // Ladder Climbing
    public void ClimbLadder(float verticalInput)
    {
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * moveSpeed);
    }

    public void StopClimbingLadder()
    {
        rb.gravityScale = 1f;
    }
}
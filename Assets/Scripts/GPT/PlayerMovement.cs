using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float wallClingGravityScale = 0.5f;

    private Rigidbody2D rb;
    private PlayerStats playerStats;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallClinging;

    [Header("Layers")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadius = 0.2f;

    [Header("Wall Check")]
    public float wallCheckDistance = 0.5f;

    [Header("Animation")]
    public Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (playerStats.isStunned) return;

        HandleMovement();
        HandleJump();
        HandleWallCling();
        UpdateAnimator();
    }

    private void HandleMovement()
    {
        float moveH = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveH * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleWallCling()
    {
        isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, wallCheckDistance, groundLayer);

        if (!isGrounded && isTouchingWall)
        {
            isWallClinging = true;
            rb.gravityScale = wallClingGravityScale;
        }
        else
        {
            isWallClinging = false;
            rb.gravityScale = 1f;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetBool("isMoving", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isWallClinging", isWallClinging);
    }
}
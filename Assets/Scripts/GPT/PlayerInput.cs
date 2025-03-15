using UnityEngine;

[RequireComponent(typeof(ComboSystem))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerInput : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Movement Keys")]
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Attack Key")]
    public KeyCode attackKey = KeyCode.Mouse0; // 1 nút duy nhất => TryComboAttack()

    private ComboSystem comboSystem;
    private PlayerCombat playerCombat;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Awake()
    {
        comboSystem = GetComponent<ComboSystem>();
        playerCombat = GetComponent<PlayerCombat>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovementInput();
        HandleAttackInput();
    }

    private void HandleMovementInput()
    {
        float horizontal = 0f;

        if (Input.GetKey(moveLeftKey)) horizontal -= 1f;
        if (Input.GetKey(moveRightKey)) horizontal += 1f;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

            if (Input.GetKeyDown(jumpKey) && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    private void HandleAttackInput()
    {
        // Kiểm tra null
        if (comboSystem == null || playerCombat == null) return;

        // Chỉ 1 nút => TryComboAttack
        if (Input.GetKeyDown(attackKey))
        {
            if (comboSystem != null)
            {
                comboSystem.EnqueueAttackInput();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

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

    [Header("Attack Keys")]
    public KeyCode attackKey = KeyCode.Mouse0;   // Bắt đầu combo
    public KeyCode comboKey = KeyCode.Mouse1;    // Tiếp combo (nếu muốn tách riêng)

    private ComboSystem comboSystem;
    private PlayerCombat playerCombat;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Awake()
    {
        // Tự động lấy reference nếu chưa gán trong Inspector
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
        else
        {
            Debug.LogWarning("[PlayerInput] No Rigidbody2D found for movement!");
        }
    }

    private void HandleAttackInput()
    {
        if (comboSystem == null || playerCombat == null)
        {
            Debug.LogError("[PlayerInput] comboSystem or playerCombat is null!");
            return;
        }

        // Bắt đầu/tiếp tục combo bằng chuột trái
        if (Input.GetKeyDown(attackKey))
        {
            if (!comboSystem.isComboActive)
            {
                comboSystem.StartCombo();
            }
            else if (!playerCombat.isAttacking)
            {
                comboSystem.ContinueCombo();
            }
        }

        // Tách nút chuột phải => tiếp combo
        if (Input.GetKeyDown(comboKey))
        {
            if (comboSystem.isComboActive && !playerCombat.isAttacking)
            {
                comboSystem.ContinueCombo();
            }
        }
    }

    // Kiểm tra chạm ground
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

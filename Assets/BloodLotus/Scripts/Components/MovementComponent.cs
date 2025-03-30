using UnityEngine;

// Đảm bảo các component cần thiết tồn tại trên cùng GameObject
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(StatsComponent))]
public class MovementComponent : MonoBehaviour
{
    [Header("Component References")]
    private Rigidbody2D rb;
    private StatsComponent stats;

    [Header("Ground Check Settings")]
    // [SerializeField] private Transform groundCheckPoint; // Nên có Transform riêng để kiểm tra chính xác hơn
    [SerializeField] private float groundCheckDistance = 0.2f; // Khoảng cách raycast xuống
    [SerializeField] private LayerMask groundLayer;    // Layer của nền đất
    public bool IsGrounded { get; private set; }

    [Header("Movement State")]
    private Vector2 currentMoveInput; // Đổi tên để rõ ràng hơn
    private bool jumpInputTriggered = false;
    private bool isFacingRight = true;

    // !!! KHÔNG DÙNG CONSTRUCTOR CHO MONOBEHAVIOUR !!!

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<StatsComponent>();

        // if (groundCheckPoint == null) {
        //     Debug.LogError("Chưa gán Ground Check Point!", this);
        // }
        if (groundLayer == 0) // Kiểm tra nếu LayerMask chưa được set
        {
            Debug.LogWarning("Ground Layer chưa được chọn trong MovementComponent!", this);
        }
    }

    void Update()
    {
        // Kiểm tra chạm đất bằng Raycast (hoặc OverlapCircle nếu dùng groundCheckPoint)
        CheckGrounded();
    }

    void FixedUpdate()
    {
        // Xử lý vật lý trong FixedUpdate
        ApplyMovement();
        ApplyJump();

        // Reset input nhảy sau khi xử lý
        jumpInputTriggered = false;
    }

    /// <summary>
    /// Nhận tín hiệu hướng di chuyển từ bên ngoài (Input Receiver).
    /// </summary>
    public void SetMoveInput(Vector2 direction)
    {
        currentMoveInput = direction;
    }

    /// <summary>
    /// Nhận tín hiệu yêu cầu nhảy từ bên ngoài.
    /// </summary>
    public void SetJumpInput(bool jump)
    {
        // Chỉ ghi nhận yêu cầu nhảy nếu đang trên mặt đất
        if (jump && IsGrounded)
        {
            jumpInputTriggered = true;
        }
    }

    private void CheckGrounded()
    {
        // Sử dụng Raycast từ vị trí gốc của transform xuống dưới
        // Bạn có thể điều chỉnh điểm bắt đầu raycast nếu cần (vd: từ chân nhân vật)
        Vector2 rayStart = transform.position; // Hoặc vị trí của groundCheckPoint nếu dùng
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, groundCheckDistance, groundLayer);

        IsGrounded = hit.collider != null;

        // Debug vẽ Raycast trong Scene view
        Color rayColor = IsGrounded ? Color.green : Color.red;
        Debug.DrawRay(rayStart, Vector2.down * groundCheckDistance, rayColor);
    }

    private void ApplyMovement()
    {
        // Lấy tốc độ từ StatsComponent, nếu không có thì dùng giá trị mặc định
        float moveSpeed = stats != null ? stats.MovementSpeed : 5f;
        if (moveSpeed <= 0)
        {
            // Debug.LogWarning("Movement Speed <= 0!", this);
            moveSpeed = 5f; // Đảm bảo có tốc độ tối thiểu để tránh đứng yên
        }

        // Tính vận tốc ngang mục tiêu
        float targetVelocityX = currentMoveInput.x * moveSpeed;

        // Debug Log để kiểm tra giá trị
        // Debug.Log($"MoveInputX: {currentMoveInput.x}, MoveSpeed: {moveSpeed}, TargetVelX: {targetVelocityX}");

        // *** THAY ĐỔI CHÍNH: Sử dụng rb.velocity ***
        // Giữ nguyên vận tốc dọc hiện tại, chỉ thay đổi vận tốc ngang
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);

        // Xử lý Flip dựa trên hướng di chuyển input (currentMoveInput.x)
        if (currentMoveInput.x > 0.1f && !isFacingRight) // Thêm ngưỡng nhỏ để tránh flip khi input gần 0
        {
            Flip();
        }
        else if (currentMoveInput.x < -0.1f && isFacingRight)
        {
            Flip();
        }
    }

    private void ApplyJump()
    {
        if (jumpInputTriggered)
        {
            // Lấy chiều cao nhảy từ StatsComponent
            float jumpHeightValue = stats != null ? stats.JumpHeight : 5f;
            if (jumpHeightValue <= 0) jumpHeightValue = 5f; // Giá trị dự phòng

            // Tính trọng lực thực tế
            float gravity = Physics2D.gravity.y * rb.gravityScale;
            // Đảm bảo gravity âm và gravityScale dương
            if (gravity >= 0 || rb.gravityScale <= 0)
            {
                 Debug.LogError("Gravity Scale của Rigidbody2D phải > 0 và trọng lực tổng thể phải âm!", this);
                 gravity = -9.81f * (rb.gravityScale > 0 ? rb.gravityScale : 1f); // Giá trị dự phòng an toàn
            }


            // Tính lực nhảy cần thiết
            float jumpForce = Mathf.Sqrt(jumpHeightValue * -2f * gravity);

            // Đặt vận tốc y về 0 trước khi nhảy để đảm bảo nhảy đúng độ cao từ mặt đất
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            // Áp dụng lực nhảy
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Debug.Log($"Jump Applied! Force: {jumpForce}");
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        // Lật nhân vật bằng cách xoay 180 độ quanh trục Y
        transform.Rotate(0f, 180f, 0f);
        // Lưu ý: Nếu dùng cách đổi localScale.x, hãy đảm bảo pivot của sprite/model là ở giữa.
        // Vector3 localScale = transform.localScale;
        // localScale.x *= -1f;
        // transform.localScale = localScale;
    }
}
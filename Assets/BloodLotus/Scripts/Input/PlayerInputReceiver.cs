using UnityEngine;
using UnityEngine.InputSystem; // <<< Bắt buộc phải có

// Gắn script này vào Player GameObject
public class PlayerInputReceiver : MonoBehaviour
{
    // Tham chiếu đến các component khác của Player mà chúng ta cần điều khiển
    [Header("Component References")]
    [SerializeField] private MovementComponent movementComponent;
    [SerializeField] private ComboComponent comboComponent;
    [SerializeField] private PlayerAnimationComponent animationComponent; // Nếu bạn có

    // Biến lưu trữ giá trị input di chuyển hiện tại
    private Vector2 moveDirection;

    // --- CÁC PHƯƠNG THỨC PUBLIC ĐỂ UNITY EVENTS GỌI ---
    // Các phương thức này sẽ được kết nối với các sự kiện trong PlayerInput component thông qua Inspector

    /// <summary>
    /// Được gọi bởi sự kiện "performed" và "canceled" của Move Action.
    /// </summary>
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // Đọc giá trị Vector2 từ context
        moveDirection = context.ReadValue<Vector2>();
        // Debug.Log($"Move Input: {moveDirection}"); // Bỏ comment để debug

        // Luôn cập nhật input di chuyển cho MovementComponent
        if (movementComponent != null)
        {
            movementComponent.SetMoveInput(moveDirection);
        }
        else
        {
            Debug.LogWarning("MovementComponent chưa được gán trong PlayerInputReceiver!", this);
        }
    }

    /// <summary>
    /// Được gọi bởi sự kiện "performed" của Jump Action.
    /// </summary>
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // Chỉ xử lý khi nút được nhấn xuống (performed)
        if (context.performed)
        {
            // Debug.Log("Jump Input Performed"); // Bỏ comment để debug
            if (movementComponent != null)
            {
                movementComponent.SetJumpInput(true); // Gửi tín hiệu nhảy

                // Kích hoạt animation nhảy nếu cần
                if (animationComponent != null && movementComponent.IsGrounded) // Giả sử có IsGrounded
                {
                    // animationComponent.PlayJumpAnimation(); // Gọi hàm animation tương ứng
                }
            }
            else
            {
                Debug.LogWarning("MovementComponent chưa được gán trong PlayerInputReceiver!", this);
            }
        }
    }

    /// <summary>
    /// Được gọi bởi sự kiện "performed" của Attack Action.
    /// </summary>
    public void OnAttackInput(InputAction.CallbackContext context)
    {
        // Chỉ xử lý khi nút được nhấn xuống (performed)
        if (context.performed)
        {
            // Debug.Log("Attack Input Performed"); // Bỏ comment để debug
            if (comboComponent != null)
            {
                comboComponent.AttemptAttack(); // Yêu cầu thực hiện tấn công
            }
            else
            {
                Debug.LogWarning("ComboComponent chưa được gán trong PlayerInputReceiver!", this);
            }
        }
    }

    // Thêm các phương thức On...Input khác nếu bạn có thêm Actions (vd: OnDodgeInput, OnInteractInput)

    // --- Cập nhật liên tục (nếu cần) ---
    // Mặc dù chúng ta đã gửi tín hiệu nhảy và tấn công qua event,
    // nhưng input di chuyển cần được cập nhật liên tục trong Update (hoặc FixedUpdate)
    // Tuy nhiên, cách làm trên đã gọi SetMoveInput ngay khi có thay đổi nên có thể không cần Update ở đây
    // Trừ khi MovementComponent của bạn cần giá trị moveDirection mỗi frame.

    // void Update()
    // {
    //     // Nếu MovementComponent cần được cập nhật mỗi frame:
    //     if (movementComponent != null)
    //     {
    //          movementComponent.SetMoveInput(moveDirection);
    //     }
    // }

    // --- Kiểm tra tham chiếu (Tùy chọn nhưng nên có) ---
    void Start()
    {
        // Kiểm tra xem các component cần thiết đã được gán trong Inspector chưa
        if (movementComponent == null)
        {
            Debug.LogError("MovementComponent chưa được gán vào PlayerInputReceiver trong Inspector!", this);
        }
        if (comboComponent == null)
        {
            Debug.LogError("ComboComponent chưa được gán vào PlayerInputReceiver trong Inspector!", this);
        }
        // Kiểm tra animationComponent nếu có
    }
}
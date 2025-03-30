using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc phải có

// Gắn component này vào Player GameObject
// Đồng thời cũng phải gắn component PlayerInput vào Player GameObject này
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandlerDirect : MonoBehaviour
{
    // Tham chiếu đến các component khác của Player
    private MovementComponent movement;
    private ComboComponent combo;
    private PlayerAnimationComponent playerAnim; // Giả sử bạn có component này

    private PlayerInput playerInput; // Tham chiếu đến component PlayerInput
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    // Thêm các InputAction khác nếu cần (Dodge, Interact...)

    void Awake()
    {
        // Lấy component PlayerInput trên cùng GameObject
        playerInput = GetComponent<PlayerInput>();

        // Lấy tham chiếu đến các component khác
        movement = GetComponent<MovementComponent>();
        combo = GetComponent<ComboComponent>();
        playerAnim = GetComponent<PlayerAnimationComponent>(); // Lấy component animation nếu có

        // Tìm các Actions dựa trên tên định nghĩa trong file .inputactions
        // QUAN TRỌNG: "Gameplay" là tên Action Map, "Move", "Jump", "Attack" là tên Actions
        // Tên phải khớp chính xác 100% (kể cả chữ hoa/thường) với những gì trong file .inputactions của bạn
        moveAction = playerInput.actions.FindAction("Gameplay/Move"); // Tìm action "Move" trong map "Gameplay"
        jumpAction = playerInput.actions.FindAction("Gameplay/Jump");
        attackAction = playerInput.actions.FindAction("Gameplay/Attack");
        // Ví dụ: dodgeAction = playerInput.actions.FindAction("Gameplay/Dodge");

        // Kiểm tra xem có tìm thấy Action không để báo lỗi sớm
        if (moveAction == null || jumpAction == null || attackAction == null)
        {
            Debug.LogError("Không tìm thấy một hoặc nhiều Input Actions (Move, Jump, Attack) trong InputActionAsset được liên kết. Hãy kiểm tra tên Action Map và Action ('Gameplay/TenAction').", this);
        }
    }

    private void OnEnable()
    {
        // Đăng ký lắng nghe các sự kiện của InputAction khi component này được kích hoạt
        // Sử dụng toán tử += để đăng ký hàm callback
        if (moveAction != null)
        {
            moveAction.performed += HandleMove; // Khi action được thực hiện (có giá trị)
            moveAction.canceled += HandleMove;  // Khi action bị hủy (ví dụ: nhả phím di chuyển)
        }
        if (jumpAction != null)
        {
            jumpAction.performed += HandleJump; // Chỉ cần biết khi nhấn nút Jump
            // jumpAction.canceled += HandleJumpRelease; // Nếu bạn cần xử lý khi nhả nút Jump
        }
        if (attackAction != null)
        {
            attackAction.performed += HandleAttack; // Chỉ cần biết khi nhấn nút Attack
        }
        // Đăng ký các action khác...
        // if (dodgeAction != null) dodgeAction.performed += HandleDodge;
    }

    private void OnDisable()
    {
        // Hủy đăng ký các sự kiện khi component bị vô hiệu hóa hoặc GameObject bị hủy
        // QUAN TRỌNG: Phải làm điều này để tránh lỗi và rò rỉ bộ nhớ
        // Sử dụng toán tử -= để hủy đăng ký
        if (moveAction != null)
        {
            moveAction.performed -= HandleMove;
            moveAction.canceled -= HandleMove;
        }
        if (jumpAction != null)
        {
            jumpAction.performed -= HandleJump;
            // jumpAction.canceled -= HandleJumpRelease;
        }
        if (attackAction != null)
        {
            attackAction.performed -= HandleAttack;
        }
        // Hủy đăng ký các action khác...
        // if (dodgeAction != null) dodgeAction.performed -= HandleDodge;
    }

    // --- Các hàm xử lý (Callback Methods) ---

    // Hàm được gọi khi sự kiện 'performed' hoặc 'canceled' của Move Action xảy ra
    private void HandleMove(InputAction.CallbackContext context)
    {
        if (movement != null)
        {
            // Đọc giá trị Vector2 từ context
            Vector2 moveInput = context.ReadValue<Vector2>();
            // Gửi giá trị này đến MovementComponent
            movement.SetMoveInput(moveInput);
        }
        else
        {
            // Log cảnh báo nếu không tìm thấy MovementComponent để dễ debug
            Debug.LogWarning("MovementComponent không được tìm thấy!", this);
        }
    }

    // Hàm được gọi khi sự kiện 'performed' của Jump Action xảy ra
    private void HandleJump(InputAction.CallbackContext context)
    {
        // context.performed đảm bảo hàm chỉ được gọi khi nút được nhấn xuống (không phải giữ hay nhả)
        if (context.performed && movement != null)
        {
            // Yêu cầu MovementComponent thực hiện nhảy
            movement.SetJumpInput(true);

            // Nếu có PlayerAnimationComponent và nhân vật đang trên mặt đất (kiểm tra qua MovementComponent) thì chơi animation nhảy
            if (playerAnim != null && movement.IsGrounded) // Giả sử MovementComponent có thuộc tính IsGrounded
            {
                 playerAnim.PlayJumpAnimation(); // Giả sử PlayerAnimationComponent có hàm này
            }
        }
        else if (context.performed) // Chỉ log nếu context.performed là true nhưng component movement lại null
        {
             Debug.LogWarning("MovementComponent không được tìm thấy!", this);
        }
    }

    // Hàm được gọi khi sự kiện 'performed' của Attack Action xảy ra
    private void HandleAttack(InputAction.CallbackContext context)
    {
        if (context.performed && combo != null)
        {
            // Yêu cầu ComboComponent thực hiện tấn công/combo
            combo.AttemptAttack();
        }
         else if (context.performed)
        {
             Debug.LogWarning("ComboComponent không được tìm thấy!", this);
        }
    }

    // Thêm các hàm HandleDodge, HandleInteract nếu cần
    // private void HandleDodge(InputAction.CallbackContext context) { ... }
}
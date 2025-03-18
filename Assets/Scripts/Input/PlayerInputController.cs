using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [Header("Movement References")]
    public AdvancedCharacterController characterController;
    public CharacterCombat combatController;

    [Header("Input Actions")]
    private PlayerInputActions inputActions;
    private Vector2 movementInput;

    [Header("Input Configuration")]
    public bool isGamepad = false;

    private void Awake()
    {
        // Khởi tạo Input Actions
        inputActions = new PlayerInputActions();

        // Đăng ký sự kiện di chuyển
        inputActions.Gameplay.Move.performed += ctx =>
        {
            movementInput = ctx.ReadValue<Vector2>();
            HandleMovementInput(movementInput);
        };

        inputActions.Gameplay.Move.canceled += ctx =>
        {
            movementInput = Vector2.zero;
            HandleMovementInput(movementInput);
        };

        // Sự kiện nhảy
        inputActions.Gameplay.Jump.performed += ctx => PerformJump();

        // Sự kiện tấn công
        inputActions.Gameplay.Attack.performed += ctx => PerformAttack();

        // Sự kiện sprint
        inputActions.Gameplay.Sprint.performed += ctx => EnableSprint();
        inputActions.Gameplay.Sprint.canceled += ctx => DisableSprint();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Disable();
    }

    // Xử lý di chuyển
    private void HandleMovementInput(Vector2 input)
    {
        characterController.SetMovementInput(input);
    }

    // Thực hiện nhảy
    private void PerformJump()
    {
        characterController.Jump();
    }

    // Thực hiện tấn công
    private void PerformAttack()
    {
        combatController.Attack();
    }

    // Kích hoạt sprint
    private void EnableSprint()
    {
        characterController.EnableSprint();
    }

    // Hủy sprint
    private void DisableSprint()
    {
        characterController.DisableSprint();
    }

    // Kiểm tra và chuyển đổi giữa gamepad và keyboard
    public void OnControlsChanged(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Gamepad");
        UpdateInputDisplay();
    }

    // Cập nhật hiển thị input
    private void UpdateInputDisplay()
    {
        Debug.Log(isGamepad ? "Gamepad Connected" : "Keyboard Connected");
    }
}
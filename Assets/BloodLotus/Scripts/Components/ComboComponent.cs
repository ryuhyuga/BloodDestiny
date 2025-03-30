// File: Assets/BloodLotus/Scripts/Components/ComboComponent.cs

using UnityEngine;
using System.Collections.Generic;
using BloodLotus.Data;
using BloodLotus.Core;

[RequireComponent(typeof(EquipmentComponent))]
[RequireComponent(typeof(CombatComponent))]
public class ComboComponent : MonoBehaviour
{
    [Header("Components")]
    private EquipmentComponent equipment;
    private CombatComponent combat;
    private PlayerAnimationComponent playerAnim;

    [Header("Combo Settings")]
    [Tooltip("Thời gian tối đa (giây) sau bước combo cuối cùng hoặc khi không nhấn nút, combo sẽ tự động reset.")]
    public float comboResetTime = 0.5f; // Thời gian chờ để reset nếu không spam

    [Header("Combo State")]
    private List<ComboStepData> currentComboSequence;
    private int currentStepIndex = -1;
    private float timeSinceLastAttackInput = 0f; // Đổi tên biến để rõ nghĩa hơn

    public bool IsInCombo => currentStepIndex >= 0;

    void Awake()
    {
        equipment = GetComponent<EquipmentComponent>();
        combat = GetComponent<CombatComponent>();
        playerAnim = GetComponent<PlayerAnimationComponent>();
    }

    void Update()
    {
        // Tăng bộ đếm thời gian kể từ lần tấn công cuối
        timeSinceLastAttackInput += Time.deltaTime;

        // Nếu đang trong combo và đã quá thời gian chờ reset -> Reset combo
        if (IsInCombo && timeSinceLastAttackInput > comboResetTime)
        {
            ResetCombo();
        }
    }

    /// <summary>
    /// Được gọi bởi Input Receiver khi có tín hiệu tấn công.
    /// Ưu tiên chuyển sang bước tiếp theo nếu có thể.
    /// </summary>
    public void AttemptAttack()
    {
        // Điều kiện cơ bản để tấn công
        if (!equipment || equipment.CurrentWeapon == null) return;
        // Không cần kiểm tra combat.CanAttack() ở đây nữa nếu muốn cho phép ngắt animation

        // Reset bộ đếm thời gian chờ reset mỗi khi nhấn nút tấn công
        timeSinceLastAttackInput = 0f;

        // Nếu chưa trong combo -> Bắt đầu combo mới
        if (!IsInCombo)
        {
            StartNewCombo();
        }
        // Nếu đã trong combo -> Chuyển sang bước tiếp theo
        else
        {
            AdvanceComboStep();
        }
    }

    private void StartNewCombo()
    {
        currentComboSequence = equipment.CurrentWeapon.baseComboSequence;
        // TODO: Logic lấy combo mở rộng nếu cần

        if (currentComboSequence == null || currentComboSequence.Count == 0)
        {
            Debug.LogWarning($"Vũ khí {equipment.CurrentWeapon.weaponName} không có Base Combo Sequence!");
            return;
        }

        currentStepIndex = 0;
        ExecuteComboStep(currentStepIndex);
    }

    private void AdvanceComboStep()
    {
        // Nếu đang ở bước cuối cùng -> Quay lại bước đầu tiên (hoặc reset tùy thiết kế)
        if (currentStepIndex >= currentComboSequence.Count - 1)
        {
            // Lựa chọn 1: Reset về trạng thái không combo
            // ResetCombo();
            // return;

            // Lựa chọn 2: Quay vòng lại từ đầu chuỗi combo
            currentStepIndex = 0;

            // Lựa chọn 3: Reset về Idle (Nếu ResetCombo() chỉ reset index mà không thoát hẳn)
            // currentStepIndex = -1; // -> Sẽ khiến lần nhấn tiếp theo gọi StartNewCombo
            // ResetCombo(); return; // Đơn giản nhất là reset hoàn toàn
        }
        else
        {
            // Tăng chỉ số lên bước tiếp theo
            currentStepIndex++;
        }

        // Thực thi bước combo mới (hoặc bước đầu tiên nếu quay vòng)
        ExecuteComboStep(currentStepIndex);
    }

    private void ExecuteComboStep(int stepIndex)
    {
        // Kiểm tra lại index hợp lệ (đề phòng trường hợp logic Advance thay đổi)
         if (stepIndex < 0 || currentComboSequence == null || stepIndex >= currentComboSequence.Count) {
             ResetCombo(); // Reset nếu index không hợp lệ
             return;
         }

        ComboStepData step = currentComboSequence[stepIndex];

        // Debug.Log($"Executing Combo Step: {stepIndex + 1}/{currentComboSequence.Count} - Anim: {step.animationTrigger}");

        // Kích hoạt animation NGAY LẬP TỨC (Animator sẽ xử lý việc ngắt animation cũ)
        if (playerAnim != null && !string.IsNullOrEmpty(step.animationTrigger))
        {
            playerAnim.PlayAttackAnimation(step.animationTrigger);
        }
        else if (string.IsNullOrEmpty(step.animationTrigger))
        {
             Debug.LogWarning($"Bước combo {stepIndex} thiếu animationTrigger!");
        }

        // Thông báo cho CombatComponent để xử lý logic hitbox/damage cho bước này
        // CombatComponent cần được thiết kế để có thể xử lý việc nhận step mới ngay cả khi step cũ chưa xong animation
        combat.InitiateAttackStep(step);

        // Không cần quản lý inputWindow trong logic này nữa,
        // vì việc chuyển tiếp xảy ra ngay khi nhấn nút.
        // Việc reset sẽ do timeSinceLastAttackInput và comboResetTime xử lý.
    }

    /// <summary>
    /// Reset trạng thái combo về ban đầu (không còn trong chuỗi combo).
    /// </summary>
    public void ResetCombo()
    {
        if (!IsInCombo) return;

        // Debug.Log("Combo Reset.");
        currentStepIndex = -1;
        // timeSinceLastAttackInput vẫn tiếp tục đếm trong Update
        currentComboSequence = null;

        // Thông báo cho CombatComponent (tùy chọn, có thể không cần nếu không có trạng thái đặc biệt)
        combat.FinishAttackSequence(); // Có thể không cần hàm này nữa
    }
}
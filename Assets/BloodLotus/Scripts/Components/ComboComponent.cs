using UnityEngine;
using System.Collections.Generic;
using BloodLotus.Data; // Thư viện chứa các enum, struct, class dùng chung
public class ComboComponent : MonoBehaviour
{
    private EquipmentComponent equipment;
    private PlayerAnimationComponent playerAnim; // Component xử lý animation
    private CombatComponent combat; // Để biết trạng thái isAttacking

    private List<ComboStepData> currentComboSequence; // Combo đang thực thi (base hoặc extended)
    private int currentStepIndex = -1; // -1: Chưa bắt đầu combo
    private float timeSinceLastInput = 0f;
    private float currentInputWindow = 0f; // Thời gian chờ input của bước hiện tại

    public bool IsInCombo => currentStepIndex >= 0;

    private void Awake()
    {
        equipment = GetComponent<EquipmentComponent>();
        playerAnim = GetComponent<PlayerAnimationComponent>();
        combat = GetComponent<CombatComponent>();
    }

    private void Update()
    {
        if (IsInCombo)
        {
            timeSinceLastInput += Time.deltaTime;
            // Nếu hết thời gian chờ input -> Reset combo
            if (timeSinceLastInput > currentInputWindow)
            {
                ResetCombo();
            }
        }
    }

    public void AttemptAttack() // Gọi từ Input hoặc CombatSystem
    {
        if (!equipment || !equipment.CurrentWeapon) return; // Cần vũ khí

        // Nếu đang không trong combo -> Bắt đầu combo mới
        if (!IsInCombo)
        {
            StartCombo();
        }
        // Nếu đang trong combo và trong thời gian chờ input -> Tiến tới bước tiếp theo
        else if (timeSinceLastInput <= currentInputWindow)
        {
            AdvanceComboStep();
        }
        // Else: Input quá trễ -> Reset combo (có thể xử lý mềm dẻo hơn)
        // else {
        //     ResetCombo();
        // }
    }

    private void StartCombo()
    {
        if (!combat.CanAttack()) return; // Kiểm tra xem có thể tấn công không (vd: không bị stun)

        currentComboSequence = equipment.CurrentWeapon.baseComboSequence; // Lấy combo gốc
        // TODO: Kiểm tra Skill có extend combo này không và level đủ chưa
        // if (CheckSkillExtension(equipment.CurrentWeapon.baseComboSequence, out var extendedSequence)) {
        //     currentComboSequence = extendedSequence;
        // }

        if (currentComboSequence == null || currentComboSequence.Count == 0)
        {
            Debug.LogWarning("Weapon has no base combo sequence defined!");
            return;
        }

        currentStepIndex = 0;
        ExecuteComboStep(currentStepIndex);
    }

    private void AdvanceComboStep()
    {
        currentStepIndex++;

        // Kiểm tra xem còn bước tiếp theo trong sequence không
        if (currentStepIndex < currentComboSequence.Count)
        {
            ExecuteComboStep(currentStepIndex);
        }
        else
        {
            // Đã hoàn thành sequence -> Reset
            Debug.Log("Combo Finished!");
            ResetCombo();
        }
    }

    private void ExecuteComboStep(int stepIndex)
    {
         if (!combat.CanAttack()) { // Kiểm tra lại trước khi thực hiện step
             ResetCombo();
             return;
         }

        ComboStepData step = currentComboSequence[stepIndex];

        Debug.Log($"Executing Combo Step: {stepIndex + 1}/{currentComboSequence.Count} - Anim: {step.animationTrigger}");

        // Trigger Animation
        playerAnim?.PlayAttackAnimation(step.animationTrigger);

        // Bắt đầu tính thời gian chờ input cho bước *tiếp theo* (nếu có)
        timeSinceLastInput = 0f;
        currentInputWindow = step.inputWindow;

        // Thông báo cho CombatComponent/System biết thông tin bước này để xử lý hitbox, damage, hitstop
        combat?.ProcessAttackStep(step);

        // Nếu đây là bước cuối cùng, không cần chờ input nữa (hoặc đặt window = 0)
        if (stepIndex == currentComboSequence.Count - 1)
        {
            currentInputWindow = 0f; // Hoặc một giá trị rất nhỏ
            // Reset combo sau khi animation của bước cuối hoàn thành? (Cần cơ chế chờ)
             // Invoke(nameof(ResetComboAfterDelay), playerAnim.GetCurrentAnimationLength()); // Ví dụ
        }
    }

    public void ResetCombo()
    {
        if (IsInCombo) // Chỉ reset nếu đang thực sự trong combo
        {
             Debug.Log("Combo Reset.");
            currentStepIndex = -1;
            timeSinceLastInput = 0f;
            currentInputWindow = 0f;
            currentComboSequence = null;
            combat?.FinishAttack(); // Thông báo cho CombatComponent là đã kết thúc tấn công/combo
        }
    }

    // Hàm kiểm tra và lấy combo mở rộng từ Skill (Cần logic chi tiết hơn)
    // private bool CheckSkillExtension(List<ComboStepData> baseCombo, out List<ComboStepData> extendedSequence)
    // {
    //     extendedSequence = new List<ComboStepData>(baseCombo); // Bắt đầu bằng base
    //     foreach (var skill in equipment.EquippedSkills)
    //     {
    //         if (skill.compatibleWeaponType == equipment.CurrentWeapon.weaponType &&
    //             skill.requiredLevel <= GetSkillLevel(skill)) // Giả sử có hàm GetSkillLevel
    //         {
    //              // Logic ghép nối extension vào base combo
    //              // Ví dụ: Thêm các bước từ skill.comboExtensionSteps vào cuối
    //              extendedSequence.AddRange(skill.comboExtensionSteps);
    //              return true; // Chỉ áp dụng extension của 1 skill? Hay kết hợp?
    //         }
    //     }
    //     return false;
    // }
}
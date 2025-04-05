// File: Assets/BloodLotus/Scripts/Components/ComboComponent.cs

using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Cần cho .FirstOrDefault() và .Concat()
using BloodLotus.Data;
using BloodLotus.Core;
using BloodLotus.Systems; // Namespace của SkillProgressionSystem

[RequireComponent(typeof(EquipmentComponent))]
[RequireComponent(typeof(CombatComponent))]
public class ComboComponent : MonoBehaviour
{
    [Header("Components")]
    private EquipmentComponent equipment;
    private CombatComponent combat;
    private PlayerAnimationComponent playerAnim;

    [Header("Combo Settings")]
    public float comboResetTime = 0.5f;

    [Header("Combo State")]
    private List<ComboStepData> currentEffectiveComboSequence; // Đổi tên để rõ là chuỗi hiệu lực
    private int currentStepIndex = -1;
    private float timeSinceLastAttackInput = 0f;

    public bool IsInCombo => currentStepIndex >= 0;

    void Awake()
    {
        equipment = GetComponent<EquipmentComponent>();
        combat = GetComponent<CombatComponent>();
        playerAnim = GetComponent<PlayerAnimationComponent>();
    }

    void Update()
    {
        timeSinceLastAttackInput += Time.deltaTime;
        if (IsInCombo && timeSinceLastAttackInput > comboResetTime)
        {
            ResetCombo();
        }
    }

    public void AttemptAttack()
    {
        if (!equipment || equipment.CurrentWeapon == null) return;

        timeSinceLastAttackInput = 0f;

        if (!IsInCombo)
        {
            StartNewCombo();
        }
        else
        {
            AdvanceComboStep();
        }
    }

    private void StartNewCombo()
    {
        // *** THAY ĐỔI CHÍNH Ở ĐÂY ***
        // Lấy chuỗi combo hiệu lực, đã bao gồm cả phần mở rộng từ skill (nếu có)
        currentEffectiveComboSequence = GetCurrentEffectiveComboSequence();

        if (currentEffectiveComboSequence == null || currentEffectiveComboSequence.Count == 0)
        {
            // Warning đã được log bên trong GetCurrentEffectiveComboSequence nếu cần
            return;
        }

        currentStepIndex = 0;
        ExecuteComboStep(currentStepIndex);
    }

    /// <summary>
    /// Lấy chuỗi combo hiệu lực dựa trên vũ khí và các skill đang trang bị.
    /// </summary>
    private List<ComboStepData> GetCurrentEffectiveComboSequence()
    {
        if (equipment.CurrentWeapon == null || equipment.CurrentWeapon.baseComboSequence == null || equipment.CurrentWeapon.baseComboSequence.Count == 0)
        {
            Debug.LogWarning("Vũ khí hiện tại hoặc Base Combo Sequence không hợp lệ.");
            return null;
        }

        // Bắt đầu với chuỗi combo cơ bản của vũ khí
        List<ComboStepData> effectiveSequence = new List<ComboStepData>(equipment.CurrentWeapon.baseComboSequence);

        // Tìm skill tương thích và có cấp độ đủ để mở rộng combo
        SkillData extendingSkill = equipment.EquippedSkills
            .Where(skill => skill != null && // Kiểm tra skill không null
                           skill.compatibleWeaponType == equipment.CurrentWeapon.weaponType && // Kiểm tra tương thích vũ khí
                           skill.comboExtensionSteps != null && skill.comboExtensionSteps.Count > 0) // Có bước mở rộng không
            .OrderByDescending(skill => SkillProgressionSystem.Instance.GetSkillLevel(skill)) // Ưu tiên skill cấp cao hơn? (Tùy chọn)
            .FirstOrDefault(skill => SkillProgressionSystem.Instance.GetSkillLevel(skill) >= skill.levelToUnlockExtension); // Lấy skill đầu tiên đủ cấp độ

        // Nếu tìm thấy skill phù hợp
        if (extendingSkill != null)
        {
            Debug.Log($"Mở rộng combo bằng Skill: {extendingSkill.skillName} (Level: {SkillProgressionSystem.Instance.GetSkillLevel(extendingSkill)})");
            // Nối các bước mở rộng vào chuỗi combo
            effectiveSequence.AddRange(extendingSkill.comboExtensionSteps);
            // Lưu ý: Đoạn code này chỉ lấy extension từ MỘT skill (skill đầu tiên đủ điều kiện).
            // Nếu bạn muốn kết hợp từ nhiều skill, logic sẽ phức tạp hơn.
        }

        return effectiveSequence;
    }


    private void AdvanceComboStep()
    {
        // Nếu đang ở bước cuối cùng của chuỗi combo HIỆU LỰC
        if (currentStepIndex >= currentEffectiveComboSequence.Count - 1)
        {
            // Quay vòng lại từ đầu (hoặc reset)
            currentStepIndex = 0;
            // ResetCombo(); // Bỏ comment nếu muốn reset thay vì quay vòng
            // return;     // Nhớ return nếu reset
        }
        else
        {
            currentStepIndex++;
        }

        ExecuteComboStep(currentStepIndex);
    }

    private void ExecuteComboStep(int stepIndex)
    {
        if (stepIndex < 0 || currentEffectiveComboSequence == null || stepIndex >= currentEffectiveComboSequence.Count) {
             ResetCombo();
             return;
         }

        ComboStepData step = currentEffectiveComboSequence[stepIndex];

        // Debug.Log($"Executing Combo Step: {stepIndex + 1}/{currentEffectiveComboSequence.Count} - Anim: {step.animationTrigger}");

        if (playerAnim != null && !string.IsNullOrEmpty(step.animationTrigger))
        {
            playerAnim.PlayAttackAnimation(step.animationTrigger);
        }
        else if (string.IsNullOrEmpty(step.animationTrigger))
        {
             Debug.LogWarning($"Bước combo {stepIndex} thiếu animationTrigger!");
        }

        combat.InitiateAttackStep(step);
    }

    public void ResetCombo()
    {
        if (!IsInCombo) return;
        // Debug.Log("Combo Reset.");
        currentStepIndex = -1;
        currentEffectiveComboSequence = null;
        // combat.FinishAttackSequence(); // Có thể không cần nữa
    }
}


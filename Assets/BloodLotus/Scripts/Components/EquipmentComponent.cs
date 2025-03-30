using UnityEngine;
using System.Collections.Generic;
public class EquipmentComponent : MonoBehaviour
{
    public WeaponData CurrentWeapon { get; private set; }
    public List<SkillData> EquippedSkills { get; private set; } = new List<SkillData>();
    public InnerPowerData CurrentInnerPower { get; private set; }
    public List<ArtifactData> EquippedArtifacts { get; private set; } = new List<ArtifactData>();

    // Sự kiện khi trang bị thay đổi (để cập nhật Stats, UI, Animation Override)
    public System.Action OnEquipmentChanged;

    private Animator animator; // Cache animator để đổi override controller
    private PlayerAnimationComponent playerAnim; // Hoặc component animation tùy chỉnh

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>(); // Tìm Animator trên con nếu cần
        playerAnim = GetComponent<PlayerAnimationComponent>();
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        CurrentWeapon = newWeapon;
        ApplyAnimationOverrides();
        RecalculateStats(); // Gọi hàm tính lại chỉ số
        OnEquipmentChanged?.Invoke();
        Debug.Log($"Equipped Weapon: {newWeapon?.weaponName}");
    }

    public void EquipSkill(SkillData newSkill) { /*...*/ }
    public void UnequipSkill(SkillData skill) { /*...*/ }
    public void EquipInnerPower(InnerPowerData newPower) { /*...*/ }
    public void EquipArtifact(ArtifactData newArtifact) { /*...*/ }
    // ... các hàm Equip/Unequip khác ...

    private void ApplyAnimationOverrides()
    {
        if (!animator) return;

        AnimatorOverrideController overrideController = null;

        // Ưu tiên override của Skill nếu có skill đặc biệt đang dùng? Hoặc chỉ dùng của Weapon?
        // Logic này cần làm rõ hơn dựa trên thiết kế chi tiết
        if (CurrentWeapon != null && CurrentWeapon.animationOverrideController != null)
        {
            overrideController = CurrentWeapon.animationOverrideController;
        }
        // TODO: Có thể có logic phức tạp hơn để kết hợp override từ skill

        // Gán override controller mới
        // Quan trọng: Cần đảm bảo Animator Controller gốc (RuntimeAnimatorController) được set trước
        if (animator.runtimeAnimatorController != null)
        {
             // Tạo một instance mới của override controller để tránh sửa đổi asset gốc
            var newOverride = new AnimatorOverrideController(animator.runtimeAnimatorController);

            if (overrideController != null)
            {
                // Sao chép các override từ SO vào instance mới
                List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                overrideController.GetOverrides(overrides);
                newOverride.ApplyOverrides(overrides);
            }
            // Else: Nếu không có override cụ thể, sẽ dùng bản gốc từ runtimeAnimatorController

             animator.runtimeAnimatorController = newOverride; // Apply the override
             if (playerAnim) playerAnim.CacheAnimationHashes(); // Bảo component animation cập nhật lại hash nếu cần
        } else {
             Debug.LogWarning("Animator does not have a base RuntimeAnimatorController assigned.");
        }


    }

    private void RecalculateStats()
    {
        // Gọi StatsComponent để tính lại chỉ số
        GetComponent<StatsComponent>()?.CalculateFinalStats();
    }
}
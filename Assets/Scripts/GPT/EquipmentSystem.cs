using System.Collections;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    private PlayerStats playerStats;

    [Header("Equipped Items")]
    public WeaponBase currentWeapon;
    public SkillBase currentSkill;
    public InnerPowerBase currentInnerPower;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("[EquipmentSystem] PlayerStats is NULL! Ensure EquipmentSystem is on the correct GameObject.");
            return;
        }
        UpdateStats();
    }

    public void EquipWeapon(WeaponBase newWeapon)
    {
        currentWeapon = newWeapon;
        UpdateStats();

        if (newWeapon == null || newWeapon.overrideController == null)
        {
            Debug.LogWarning("[EquipmentSystem] Weapon or its overrideController is NULL!");
            return;
        }

        Animator anim = GetComponentInParent<Animator>();
        if (anim == null)
        {
            Debug.LogError("[EquipmentSystem] Animator is NULL! Ensure EquipmentSystem is on the correct GameObject.");
            return;
        }

        if (anim.runtimeAnimatorController != newWeapon.overrideController)
        {
            anim.runtimeAnimatorController = null; // Reset trước khi gán mới
            anim.runtimeAnimatorController = newWeapon.overrideController;
            Debug.Log($"[EquipmentSystem] Animator updated to {newWeapon.overrideController.name}");
        }
        else
        {
            Debug.Log($"[EquipmentSystem] Animator already set to {newWeapon.overrideController.name}, skipping.");
        }

        Debug.Log($"[EquipmentSystem] Equipped weapon: {newWeapon.weaponName}");
    }


    private IEnumerator UpdateAnimator(WeaponBase newWeapon)
    {
        yield return null; // Đợi 1 frame trước khi cập nhật

        var anim = GetComponent<Animator>();
        if (newWeapon != null && newWeapon.overrideController != null)
        {
            anim.runtimeAnimatorController = newWeapon.overrideController;
            Debug.Log($"[EquipmentSystem] Animator set to {newWeapon.overrideController.name}");
        }
    } // ⚠️ Đóng dấu } đúng vị trí để kết thúc UpdateAnimator()

    // ✅ Tách riêng phương thức EquipSkill()
    public void EquipSkill(SkillBase newSkill)
    {
        currentSkill = newSkill;
        Debug.Log($"[EquipmentSystem] Equipped skill: {newSkill.skillName}");

        // Cập nhật Animator nếu có overrideController
        if (newSkill != null && newSkill.overrideController != null)
        {
            var anim = GetComponentInParent<Animator>(); // Đảm bảo lấy đúng Animator của nhân vật

            if (anim == null)
            {
                Debug.LogError("[EquipmentSystem] Animator is NULL! Ensure EquipmentSystem is on the correct GameObject.");
                return;
            }

            Debug.Log($"[EquipmentSystem] Applying Animator Override Controller from Skill: {newSkill.overrideController}");

            // Gán overrideController của SkillBase vào Animator
            anim.runtimeAnimatorController = newSkill.overrideController as RuntimeAnimatorController;

            Debug.Log($"[EquipmentSystem] Animator set to {newSkill.overrideController.name}");
        }
        else
        {
            Debug.LogWarning("[EquipmentSystem] Skill overrideController is NULL, keeping the previous animator.");
        }
    }


    public void EquipInnerPower(InnerPowerBase newInnerPower)
    {
        // Gỡ bỏ nội công cũ
        if (currentInnerPower != null)
        {
            RemoveInnerPower(currentInnerPower);
        }

        currentInnerPower = newInnerPower;
        currentInnerPower.ApplyPassive(playerStats);
        Debug.Log($"[EquipmentSystem] Equipped inner power: {newInnerPower.powerName}");
    }

    public void RemoveInnerPower(InnerPowerBase oldPower)
    {
        // Giảm các chỉ số (nếu muốn)
        playerStats.baseDamage -= oldPower.damageBonus;
        playerStats.baseHeavyDamage -= oldPower.heavyDamageBonus;
        playerStats.baseAttackSpeed -= oldPower.attackSpeedBonus;
        playerStats.baseCritRate -= oldPower.critRateBonus;
        playerStats.baseCritDamage -= oldPower.critDamageBonus;
        playerStats.baseArmor -= oldPower.armorBonus;
        playerStats.baseMaxHealth -= oldPower.maxHealthBonus;

        playerStats.ComputeFinalStats();
    }

    public void UpdateStats()
    {
        // Tính toán lại stats khi thay đổi vũ khí
        if (currentWeapon != null)
        {
            playerStats.baseDamage = currentWeapon.baseDamage;
            playerStats.baseAttackSpeed = currentWeapon.attackSpeed;
            // v.v...

            // In log trước khi compute
            Debug.Log($"[PlayerStats] Before compute: baseDamage={playerStats.baseDamage}");

            // Gọi hàm tính finalStats
            playerStats.ComputeFinalStats();

            // In log sau khi compute
            Debug.Log($"[PlayerStats] After compute: finalStats.damage={playerStats.finalStats.damage}");
        }
        else
        {
            // Reset stats về mặc định
        }
        playerStats.ComputeFinalStats();
    }
}

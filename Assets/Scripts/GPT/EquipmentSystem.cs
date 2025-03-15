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
        UpdateStats();
    }

    public void EquipWeapon(WeaponBase newWeapon)
    {
        currentWeapon = newWeapon;
        UpdateStats();
        if (newWeapon != null && newWeapon.overrideController != null)
        {
            var anim = GetComponent<Animator>();
            anim.runtimeAnimatorController = newWeapon.overrideController;
        }
        Debug.Log($"[EquipmentSystem] Equipped weapon: {newWeapon.weaponName}");
    }

    public void EquipSkill(SkillBase newSkill)
    {
        currentSkill = newSkill;
        Debug.Log($"[EquipmentSystem] Equipped skill: {newSkill.skillName}");
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
        }
        else
        {
            // Reset stats về mặc định
        }
        playerStats.ComputeFinalStats();
    }
}

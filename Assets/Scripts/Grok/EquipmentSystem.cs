using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    private CharacterStats characterStats;

    [Header("Equipped Items")]
    public WeaponBase currentWeapon;
    public InnerPowerBase currentInnerPower;
    public SkillBase currentSkill; // Võ kĩ

    void Start()
    {
        characterStats = GetComponent<CharacterStats>();
        UpdateStats();
    }

    public void EquipWeapon(WeaponBase newWeapon)
    {
        // Unequip cũ
        if (currentWeapon != null)
            currentWeapon.OnUnequip(GetComponent<PlayerCombat>());

        currentWeapon = newWeapon;
        if (newWeapon != null)
            newWeapon.OnEquip(GetComponent<PlayerCombat>());

        UpdateStats();
        var anim = GetComponent<Animator>();
        if (newWeapon != null && newWeapon.overrideController != null)
        {
            anim.runtimeAnimatorController = newWeapon.overrideController;
        }
    }

    public void EquipInnerPower(InnerPowerBase newInner)
    {
        currentInnerPower = newInner;
        UpdateStats();
    }

    public void EquipSkill(SkillBase newSkill)
    {
        currentSkill = newSkill;
        // override animator skill? Tùy logic
        UpdateStats();
    }

    public void UpdateStats()
    {
        float wDmg = 0, wHDmg = 0, wAS = 0, wCR = 0, wCDmg = 0, wArmor = 0, wHP = 0;
        if (currentWeapon != null)
        {
            wDmg = currentWeapon.damageBonus;
            wHDmg = currentWeapon.heavyDamageBonus;
            wAS = currentWeapon.attackSpeedBonus;
            wCR = currentWeapon.critRateBonus;
            wCDmg = currentWeapon.critDamageBonus;
            wArmor = currentWeapon.armorBonus;
            wHP = currentWeapon.maxHealthBonus;
        }

        float iDmg = 0, iHDmg = 0, iAS = 0, iCR = 0, iCDmg = 0, iArmor = 0, iHP = 0;
        if (currentInnerPower != null)
        {
            iDmg = currentInnerPower.damageBonus;
            iHDmg = currentInnerPower.heavyDamageBonus;
            iAS = currentInnerPower.attackSpeedBonus;
            iCR = currentInnerPower.critRateBonus;
            iCDmg = currentInnerPower.critDamageBonus;
            iArmor = currentInnerPower.armorBonus;
            iHP = currentInnerPower.maxHealthBonus;
        }

        characterStats.ComputeFinalStats(
            wDmg, wHDmg, wAS, wCR, wCDmg, wArmor, wHP,
            iDmg, iHDmg, iAS, iCR, iCDmg, iArmor, iHP
        );
    }
}

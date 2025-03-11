using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Fist,
    Sword,
    Spear,
    Axe
}

[System.Serializable]
public struct ComboStep
{
    public int animIndex;     // Used to set animator.SetInteger("AttackIndex", animIndex)
    public bool forceCrit;    // Does this step have a 100% crit chance?
    public float damageMultiplier; // Damage multiplier for this step
}


[CreateAssetMenu(fileName = "NewWeapon", menuName = "Equipment/Weapon")]

public class WeaponBase : ScriptableObject
{
    public string weaponName;
    public WeaponType weaponType;

    [Header("Stats Bonus")]
    public float damageBonus = 5f;
    public float heavyDamageBonus = 10f;
    public float attackSpeedBonus = 0f;
    public float critRateBonus = 0f;
    public float critDamageBonus = 0f;
    public float armorBonus = 0f;
    public float maxHealthBonus = 0f;

    [Header("Combo Steps (animIndex)")]
    public List<ComboStep> baseComboSteps;

    [Header("Animator Override")]
    public RuntimeAnimatorController overrideController;

    public void OnEquip(PlayerCombat player)
    {
        // Ví dụ: gán vũ khí cho PlayerCombat
        if (player != null)
        {
            player.equippedWeapon = this;
            // ... logic bổ sung ...
        }
    }
    public void OnUnequip(PlayerCombat player)
    {
        // Ví dụ: Xoá tham chiếu vũ khí trong PlayerCombat
        if (player != null)
        {
            player.equippedWeapon = null;
        }
    }
}

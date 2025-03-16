using UnityEngine;
using System.Collections.Generic;

public enum WeaponType
{
    Fist,
    Sword,
    Spear,
    Axe
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Weapon")]
public class WeaponBase : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    public WeaponType weaponType;
    public Sprite icon;

    [Header("Stats")]
    public float baseDamage = 10f;
    public float attackSpeed = 1.0f;  // Đòn đánh mỗi giây
    public float critChance = 0.1f;
    public float staminaCost = 5f;

    [Header("Default Combo")]
    // Combo mặc định của vũ khí (4 đòn)
    public List<ComboStep> defaultComboSteps;

    [Header("Animator Override")]
    public AnimatorOverrideController overrideController;
}

[System.Serializable]
public class ComboStep
{
    public int animIndex;
    public string animationName;
    public float damageMultiplier = 1f;
    public float attackDelay = 0.3f;
    public int requiredSkillLevel = 0;
}

using UnityEngine;
using System.Collections.Generic;

public enum SkillWeaponRequirement
{
    Any,
    Sword,
    Spear
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "ForbiddenLotus/Skill")]
public class SkillBase : ScriptableObject
{
    public string skillName;
    public SkillWeaponRequirement requiredWeaponType;
    public int skillLevel = 1;
    public int maxLevel = 5;

    [Header("Skill Combo Steps (animIndex)")]
    public List<ComboStep> skillComboSteps; // Danh sách combo (tối đa 9 đòn)

    public bool CanUseSkill(WeaponBase equippedWeapon)
    {
        if (equippedWeapon == null) return false;
        switch (requiredWeaponType)
        {
            case SkillWeaponRequirement.Any:
                return true;
            case SkillWeaponRequirement.Sword:
                return (equippedWeapon.weaponType == WeaponType.Sword);
            case SkillWeaponRequirement.Spear:
                return (equippedWeapon.weaponType == WeaponType.Spear);
        }
        return false;
    }

    public List<ComboStep> GetSkillCombo(int level)
    {
        // Level 1: 5 đòn, Level 2: 6 đòn, ..., Level 5: 9 đòn
        int comboCount = Mathf.Min(skillComboSteps.Count, 5 + (level - 1));
        return skillComboSteps.GetRange(0, comboCount);
    }
}
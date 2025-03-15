using UnityEngine;
using System.Collections.Generic;

public enum SkillType
{
    None,
    SwordSkill,
    SpearSkill,
    FistSkill,
    AxeSkill
}
[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/WeaponSkill")]
public class SkillBase : ScriptableObject
{
    [Header("Skill Info")]
    public string skillName;
    public SkillType skillType;
    public int currentLevel = 1;
    public int maxLevel = 3;

    [Header("EXP")]
    public float expRequired = 100f;
    public float expGrowthRate = 1.5f;

    [Header("Combo Extension")]
    public bool unlockExtendedCombo = false;
    public List<ComboStep> extendedComboSteps = new List<ComboStep>
    {
        new ComboStep { animIndex = 4, animationName = "ExtraSlash1", damageMultiplier = 1.2f, attackDelay = 0.3f, requiredSkillLevel = 2 },
        new ComboStep { animIndex = 5, animationName = "ExtraSlash2", damageMultiplier = 1.5f, attackDelay = 0.3f, requiredSkillLevel = 3 }
    };

    private void LevelUp()
    {
        currentLevel++;
        expRequired *= expGrowthRate;
        if (currentLevel == 2) unlockExtendedCombo = true;

        Debug.Log($"[SkillBase] {skillName} leveled up to {currentLevel}!");
    }

    /// <summary>
    /// Trả về combo mở rộng nếu skill đã unlock, ghép vào combo cơ bản.
    /// </summary>
    public List<ComboStep> GetExtendedCombo(List<ComboStep> baseCombo)
    {
        if (!unlockExtendedCombo) return baseCombo;

        // Tăng số đòn combo dựa trên level
        // Thí dụ: level 2 => +2 đòn, level 3 => +5 đòn (tuỳ ý)
        int extraCount = (currentLevel == 2) ? 2 : (currentLevel >= 3 ? 5 : 0);
        extraCount = Mathf.Min(extraCount, extendedComboSteps.Count);

        List<ComboStep> finalCombo = new List<ComboStep>(baseCombo);
        for (int i = 0; i < extraCount; i++)
        {
            finalCombo.Add(extendedComboSteps[i]);
        }
        return finalCombo;
    }

    /// <summary>
    /// Kiểm tra skill có khớp hệ vũ khí không
    /// </summary>
    public bool IsCompatibleWith(WeaponType wType)
    {
        // Dựa trên skillType & wType, ví dụ:
        // if (skillType == SkillType.SwordSkill && wType == WeaponType.Sword) return true;
        // ...
        switch (skillType)
        {
            case SkillType.SwordSkill:
                return (wType == WeaponType.Sword);
            case SkillType.SpearSkill:
                return (wType == WeaponType.Spear);
            case SkillType.FistSkill:
                return (wType == WeaponType.Fist);
            case SkillType.AxeSkill:
                return (wType == WeaponType.Axe);
            default:
                return false;
        }
    }
}
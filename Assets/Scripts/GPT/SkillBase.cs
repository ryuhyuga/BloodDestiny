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
    public int currentLevel = 1; // Cấp độ skill hiện tại
    public int maxLevel = 3;     // Cấp độ tối đa

    [Header("EXP")]
    public float expRequired = 100f;
    public float expGrowthRate = 1.5f;

    [Header("Combo Extension")]
    public bool unlockExtendedCombo = false;

    /// <summary>
    /// Danh sách các bước combo mở rộng.
    /// Mỗi bước có requiredSkillLevel => skill chỉ thêm bước này
    /// nếu currentLevel >= requiredSkillLevel.
    /// </summary>
    public List<ComboStep> extendedComboSteps = new List<ComboStep>();

    [Header("Animator Override")] // Thêm trường này để dễ quản lý trong Inspector
    public RuntimeAnimatorController overrideController; // Thêm thuộc tính này

    /// <summary>
    /// Gọi khi skill lên cấp (đủ exp).
    /// Bạn có thể thay đổi logic unlockExtendedCombo theo ý muốn.
    /// </summary>
    private void LevelUp()
    {
        currentLevel++;
        if (currentLevel >= 2)
            unlockExtendedCombo = true;

        expRequired *= expGrowthRate;

        Debug.Log($"[SkillBase] {skillName} leveled up to {currentLevel}!");
    }

    /// <summary>
    /// Trả về combo mở rộng (nối vào baseCombo)
    /// nếu skill đã unlock (unlockExtendedCombo = true)
    /// và requiredSkillLevel <= currentLevel.
    /// </summary>
    public List<ComboStep> GetExtendedCombo(List<ComboStep> baseCombo)
    {
        // Nếu skill chưa unlock extended combo => chỉ trả về combo cơ bản
        if (!unlockExtendedCombo) return baseCombo;

        // Tạo finalCombo chứa baseCombo (4 đòn của vũ khí)
        List<ComboStep> finalCombo = new List<ComboStep>(baseCombo);

        // Thêm những step có requiredSkillLevel <= currentLevel
        foreach (var step in extendedComboSteps)
        {
            if (step.requiredSkillLevel <= currentLevel)
            {
                finalCombo.Add(step);
            }
        }

        return finalCombo;
    }

    /// <summary>
    /// Kiểm tra skill có khớp hệ vũ khí không
    /// </summary>
    public bool IsCompatibleWith(WeaponType wType)
    {
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

    /// <summary>
    /// Tăng exp cho skill, nếu đủ => LevelUp()
    /// </summary>
    public void GainExp(float amount)
    {
        if (currentLevel >= maxLevel) return;

        expRequired -= amount;
        if (expRequired <= 0)
        {
            LevelUp();
        }
    }
}
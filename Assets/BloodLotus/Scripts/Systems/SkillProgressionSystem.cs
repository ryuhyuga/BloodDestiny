using UnityEngine;
using System.Collections.Generic;

public class SkillProgressionSystem : MonoBehaviour // Singleton pattern?
{
    // Lưu trữ kinh nghiệm của từng skill (dùng Dictionary hoặc class riêng)
    // Key có thể là SkillData asset hoặc một ID duy nhất
    private Dictionary<SkillData, float> skillExperience = new Dictionary<SkillData, float>();
    private Dictionary<SkillData, int> skillLevels = new Dictionary<SkillData, int>();

    // TODO: Load/Save trạng thái skill (PlayerPrefs, file JSON...)

    private void Start()
    {
        // TODO: Load skill progress
        // Ví dụ đăng ký lắng nghe sự kiện enemy chết
        // EnemyManager.OnEnemyDefeated += HandleEnemyDefeated;
    }

    private void HandleEnemyDefeated(EnemyData enemyData, GameObject playerObject) // Cần sự kiện tương ứng
    {
        var equipment = playerObject.GetComponent<EquipmentComponent>();
        if (!equipment || equipment.CurrentWeapon == null) return;

        // Lấy danh sách skill đang trang bị phù hợp với vũ khí
        foreach (var skill in equipment.EquippedSkills)
        {
            if (skill.compatibleWeaponType == equipment.CurrentWeapon.weaponType)
            {
                AddExpToSkill(skill, CalculateExpGain(enemyData)); // Tính EXP nhận được
            }
        }
    }

    private float CalculateExpGain(EnemyData enemyData)
    {
        // Logic tính exp dựa trên enemy (vd: level, loại enemy...)
        // Có thể dùng công thức tuyến tính/hàm mũ với biến điều chỉnh dễ dàng
        float baseExp = 50f; // Ví dụ
        float levelMultiplier = 1.0f; // Điều chỉnh dựa trên level player vs enemy?
        return baseExp * levelMultiplier;
    }

    private void AddExpToSkill(SkillData skill, float expAmount)
    {
        if (!skillExperience.ContainsKey(skill))
        {
            skillExperience[skill] = 0f;
            skillLevels[skill] = 1; // Level khởi điểm
        }

        skillExperience[skill] += expAmount;
        Debug.Log($"Skill '{skill.skillName}' gained {expAmount} EXP. Total: {skillExperience[skill]}");

        // Kiểm tra lên level
        float expToNextLevel = CalculateExpForNextLevel(skillLevels[skill]);
        while (skillExperience[skill] >= expToNextLevel)
        {
            skillExperience[skill] -= expToNextLevel;
            skillLevels[skill]++;
            Debug.Log($"*** Skill '{skill.skillName}' Leveled Up to Level {skillLevels[skill]}! ***");
            // Trigger sự kiện Skill Leveled Up (để UI cập nhật, mở khóa combo extension...)
            // OnSkillLeveledUp?.Invoke(skill, skillLevels[skill]);
             expToNextLevel = CalculateExpForNextLevel(skillLevels[skill]); // Check for multi-level up
        }
    }

    private float CalculateExpForNextLevel(int currentLevel)
    {
        // Công thức tính EXP cần cho level tiếp theo (vd: tuyến tính, mũ)
        // Ví dụ đơn giản: return 100f * Mathf.Pow(1.5f, currentLevel -1);
         return 100 + (currentLevel * 50); // Tuyến tính đơn giản
    }

    public int GetSkillLevel(SkillData skill)
    {
         return skillLevels.TryGetValue(skill, out int level) ? level : 0; // Trả về 0 nếu chưa học?
    }
}


// --- Các Systems khác ---
// InputSystemManager.cs (Quản lý Unity's Input System)
// GameManager.cs (Quản lý state game, scene loading, pause)
// UIManager.cs (Cập nhật UI dựa trên sự kiện từ các components/systems khác)
// AudioSystemManager.cs (Quản lý phát nhạc nền, SFX)
// LootSystem.cs (Xử lý logic rơi đồ)
// InheritanceSystem.cs (Xử lý chết, kế thừa)
// ArtifactSystem.cs (Quản lý hiệu ứng bảo vật)
// DeathSystem.cs (Xử lý logic khi entity chết - gọi từ OnDied event của StatsComponent)
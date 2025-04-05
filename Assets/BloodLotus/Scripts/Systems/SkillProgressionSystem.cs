using UnityEngine;
using System.Collections.Generic;
using BloodLotus.Data; // Cần cho SkillData, EnemyData
using BloodLotus.Core; // Cần cho WeaponType

namespace BloodLotus.Systems
{
    public class SkillProgressionSystem : MonoBehaviour
    {
        #region Singleton Pattern
        public static SkillProgressionSystem Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSystem();
            }
            else if (Instance != this)
            {
                Debug.LogWarning("Một instance khác của SkillProgressionSystem đã tồn tại. Hủy bỏ instance mới này.");
                Destroy(gameObject);
            }
        }
        #endregion

        private class SkillProgressData
        {
            public int Level = 1;
            public float CurrentXP = 0f;
        }
        private Dictionary<SkillData, SkillProgressData> playerSkillProgress = new Dictionary<SkillData, SkillProgressData>();

        public event System.Action<SkillData, int> OnSkillLeveledUp;

        [Header("EXP Calculation Settings")]
        [Range(0.1f, 1.0f)]
        [Tooltip("Hệ số điều chỉnh tỉ lệ EXP nhận được khi level enemy khác level skill (0.5 = căn bậc hai).")]
        public float expScalingFactorK = 0.5f;

        private void InitializeSystem()
        {
            Debug.Log("Skill Progression System Initialized.");
            LoadSkillProgress();
            Debug.LogWarning("Cần implement cơ chế nhận sự kiện Enemy chết!");
        }

        void OnDestroy()
        {
            // Hủy đăng ký sự kiện nếu có
            SaveSkillProgress();
        }

        // --- Xử Lý EXP ---
        public void HandleEnemyDefeated(EnemyData defeatedEnemyData, GameObject killer)
        {
            // Log khi hàm bắt đầu
            Debug.Log($"--- [SkillSystem] HandleEnemyDefeated CALLED! --- Enemy: {defeatedEnemyData?.enemyName ?? "NULL"}, Killer: {killer?.name ?? "NULL"} (Frame: {Time.frameCount})");

            // Các kiểm tra điều kiện cơ bản
            if (defeatedEnemyData == null || killer == null) {
                Debug.LogWarning("[SkillSystem] HandleEnemyDefeated ignored: Invalid data (Enemy or Killer is null).");
                return;
            }
            if (!killer.CompareTag("Player")) {
                Debug.LogWarning($"[SkillSystem] HandleEnemyDefeated ignored: Killer '{killer.name}' is not tagged 'Player'.");
                return;
            }

            var equipment = killer.GetComponent<EquipmentComponent>();
            if (equipment == null) {
                Debug.LogWarning("[SkillSystem] HandleEnemyDefeated ignored: Killer has no EquipmentComponent.");
                return;
            }

            // Log thông tin vũ khí và số skill
            Debug.Log($"[SkillSystem] Killer weapon: {equipment.CurrentWeapon?.weaponName ?? "None"}");
            Debug.Log($"[SkillSystem] Processing skills for Player '{killer.name}'. Equipped Skills Count: {equipment.EquippedSkills.Count}");

            // Biến đếm số skill đã xử lý thành công
            int processedSkillCount = 0;

            // *** VÒNG LẶP FOREACH DUY NHẤT ***
            foreach (var skill in equipment.EquippedSkills)
            {
                if (skill == null) {
                    Debug.LogWarning($"[SkillSystem] Encountered a null skill in EquippedSkills list.");
                    continue; // Bỏ qua skill null
                }

                // Log bắt đầu xử lý skill này
                Debug.Log($"--- [SkillSystem] Checking Skill '{skill.skillName}' (InstanceID: {skill.GetInstanceID()}) ---");

                // <<< KHAI BÁO VÀ TÍNH weaponCompatible TRONG VÒNG LẶP >>>
                bool weaponCompatible = equipment.CurrentWeapon == null
                                     || skill.compatibleWeaponType == WeaponType.None
                                     || (equipment.CurrentWeapon != null && skill.compatibleWeaponType == equipment.CurrentWeapon.weaponType);

                // Log kết quả tương thích
                Debug.Log($"[SkillSystem] Weapon Compatible: {weaponCompatible}");

                // Chỉ xử lý tiếp nếu vũ khí tương thích
                if (weaponCompatible)
                {
                    int currentSkillLevel = GetSkillLevel(skill);
                    if (currentSkillLevel <= 0) {
                         Debug.LogWarning($"[SkillSystem] Skill '{skill.skillName}' has level {currentSkillLevel}. Assuming level 1.");
                         currentSkillLevel = 1;
                    }

                    // Tính EXP
                    float expGain = CalculateExpGain(defeatedEnemyData, currentSkillLevel);
                    Debug.Log($"[SkillSystem] Calculated EXP for '{skill.skillName}' (Lvl {currentSkillLevel}): {expGain}");

                    // Nếu có EXP để cộng
                    if (expGain > 0)
                    {
                        processedSkillCount++; // Tăng biến đếm
                        // Gọi hàm cộng EXP (đã có log bên trong)
                        AddExpToSkill(skill, expGain);
                    }
                    else
                    {
                         Debug.LogWarning($"[SkillSystem] Calculated EXP for '{skill.skillName}' is zero or negative. Not adding EXP.");
                    }
                }
                // Log kết thúc xử lý skill này
                Debug.Log($"--- [SkillSystem] Finished processing '{skill.skillName}' ---");
            } // <<< KẾT THÚC VÒNG LẶP FOREACH >>>

            // Log cuối cùng nếu không có skill nào được cộng EXP
            if (processedSkillCount == 0) {
                if (equipment.EquippedSkills.Count == 0) {
                     Debug.Log("[SkillSystem] Player has no skills equipped.");
                } else {
                     Debug.LogWarning("[SkillSystem] No compatible/valid skills found to grant EXP to this time.");
                }
            }
        } // <<< KẾT THÚC HÀM HandleEnemyDefeated >>>


        private float CalculateExpGain(EnemyData enemyData, int currentSkillLevel)
        {
            // Log giá trị đầu vào
             Debug.Log($"--- [SkillSystem] CalculateExpGain --- Enemy: {enemyData?.enemyName ?? "NULL"} (Lvl: {enemyData?.enemyLevel ?? 0}), Skill Lvl: {currentSkillLevel}");

            if (enemyData == null || currentSkillLevel <= 0) return 0f;

            float baseExp = enemyData.baseExperienceReward;
            int enemyLevel = enemyData.enemyLevel;

            // Log giá trị gốc
             Debug.Log($"[SkillSystem] Base EXP: {baseExp}, Enemy Level: {enemyLevel}");

            if (baseExp <= 0 || enemyLevel <= 0) return 0f;

            // Tính toán
            float levelRatio = (float)enemyLevel / currentSkillLevel;
            // Debug.Log($"[SkillSystem] Level Ratio (Enemy/Skill): {levelRatio:F3}");

            float expMultiplier = Mathf.Pow(levelRatio, expScalingFactorK);
            // Debug.Log($"[SkillSystem] EXP Multiplier (Ratio^k) before clamp: {expMultiplier:F3} (k={expScalingFactorK})");

            expMultiplier = Mathf.Max(0.1f, expMultiplier); // Giới hạn dưới
            // Debug.Log($"[SkillSystem] EXP Multiplier after Min clamp (0.1): {expMultiplier:F3}");

            float finalExp = baseExp * expMultiplier;
            int roundedFinalExp = Mathf.RoundToInt(finalExp);
             Debug.Log($"[SkillSystem] Final Calculated EXP: {finalExp:F3} -> Rounded: {roundedFinalExp}");

            return roundedFinalExp;
        }

        private void AddExpToSkill(SkillData skill, float expAmount)
        {
            Debug.Log($"--- [SkillSystem] AddExpToSkill --- Skill: {skill?.skillName ?? "NULL"}, Amount: {expAmount}");

            if (expAmount <= 0 || skill == null) return;

            if (!playerSkillProgress.ContainsKey(skill))
            {
                // Debug.Log($"[SkillSystem] Initializing progress data for '{skill.skillName}'.");
                playerSkillProgress[skill] = new SkillProgressData();
            }
            SkillProgressData progressData = playerSkillProgress[skill];

            // Debug.Log($"[SkillSystem] '{skill.skillName}' (Lvl {progressData.Level}) - Before Add: {progressData.CurrentXP} XP");
            progressData.CurrentXP += expAmount;
            Debug.Log($"[SkillSystem] '{skill.skillName}' (Lvl {progressData.Level}) - After Add: {progressData.CurrentXP} XP");

            float expToNextLevel = CalculateExpForNextLevel(progressData.Level);
             Debug.Log($"[SkillSystem] '{skill.skillName}' XP To Next Level ({progressData.Level + 1}): {expToNextLevel}");

            bool leveledUp = false;
            while (progressData.CurrentXP >= expToNextLevel && expToNextLevel > 0)
            {
                leveledUp = true;
                progressData.CurrentXP -= expToNextLevel;
                progressData.Level++;
                Debug.LogError($"--- [SkillSystem] LEVEL UP! --- Skill: {skill.skillName}, New Level: {progressData.Level}, Remaining XP: {progressData.CurrentXP}");
                OnSkillLeveledUp?.Invoke(skill, progressData.Level);
                expToNextLevel = CalculateExpForNextLevel(progressData.Level);
                 Debug.Log($"[SkillSystem] '{skill.skillName}' XP To Next Level ({progressData.Level + 1}): {expToNextLevel}");
            }

             if (!leveledUp && progressData.CurrentXP < expToNextLevel) {
                  Debug.Log($"[SkillSystem] '{skill.skillName}' did not level up. Progress: {progressData.CurrentXP}/{expToNextLevel}");
             }
        }

        private float CalculateExpForNextLevel(int currentLevel)
        {
            if (currentLevel <= 0) return 100;
            return 100 + (currentLevel * 75);
        }

        public int GetSkillLevel(SkillData skill)
        {
            if (skill != null && playerSkillProgress.TryGetValue(skill, out SkillProgressData progressData))
             {
                 return progressData.Level;
             }
            return (skill != null) ? 1 : 0;
        }

        private void LoadSkillProgress() {/* ... */}
        public void SaveSkillProgress() {/* ... */}
    }
}
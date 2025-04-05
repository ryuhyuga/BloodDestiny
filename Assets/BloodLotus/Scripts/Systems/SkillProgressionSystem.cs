using UnityEngine;
using System.Collections.Generic;
using BloodLotus.Data; // Cần cho SkillData, EnemyData
using BloodLotus.Core; // Cần cho các thành phần Core (nếu có)

namespace BloodLotus.Systems // <<< Đặt vào namespace Systems
{
    public class SkillProgressionSystem : MonoBehaviour
    {
        #region Singleton Pattern
        // Đảm bảo chỉ có một instance duy nhất của hệ thống này
        public static SkillProgressionSystem Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Giữ hệ thống này tồn tại giữa các Scene
                InitializeSystem(); // Gọi hàm khởi tạo riêng
            }
            else if (Instance != this) // Nếu đã có instance khác tồn tại
            {
                Debug.LogWarning("Một instance khác của SkillProgressionSystem đã tồn tại. Hủy bỏ instance mới này.");
                Destroy(gameObject); // Hủy bỏ instance thừa
            }
        }
        #endregion

        // Dùng Dictionary để lưu trữ dữ liệu skill của người chơi
        // Key: ScriptableObject SkillData (hoạt động tốt nếu không thay đổi SO lúc runtime)
        // Value: Thông tin cấp độ và kinh nghiệm
        private class SkillProgressData
        {
            public int Level = 1; // Bắt đầu từ level 1 khi học
            public float CurrentXP = 0f;
        }
        private Dictionary<SkillData, SkillProgressData> playerSkillProgress = new Dictionary<SkillData, SkillProgressData>();

        // Sự kiện được phát ra khi một skill lên cấp
        public event System.Action<SkillData, int> OnSkillLeveledUp;

        // Hàm khởi tạo hệ thống
        private void InitializeSystem()
        {
            Debug.Log("Skill Progression System Initialized.");
            LoadSkillProgress(); // Tải dữ liệu skill đã lưu (nếu có)

            // --- Đăng ký lắng nghe sự kiện Enemy chết ---
            // Cách 1: Dùng sự kiện tĩnh (Static Event) - Khuyến nghị nếu bạn có thể thiết kế
             // EnemyHealth.OnStaticEnemyDied += HandleEnemyDefeated; // Giả sử EnemyHealth có sự kiện tĩnh

            // Cách 2: Thông qua một Enemy Manager (Nếu bạn có Manager quản lý tất cả Enemy)
            // if (EnemyManager.Instance != null) {
            //     EnemyManager.Instance.OnEnemyDied += HandleEnemyDefeated;
            // } else {
            //     Debug.LogWarning("Không tìm thấy EnemyManager để đăng ký sự kiện Enemy chết.");
            // }

            // Cách 3: Tạm thời dùng cách tìm kiếm (Không hiệu quả lắm nhưng để test)
            // Chúng ta sẽ cần một cách tốt hơn để biết Enemy nào chết và Player nào giết
            // TODO: Implement cơ chế sự kiện Enemy chết phù hợp hơn
        }

         void OnDestroy() {
             // Hủy đăng ký sự kiện khi hệ thống bị hủy (quan trọng!)
              // EnemyHealth.OnStaticEnemyDied -= HandleEnemyDefeated;
             // if (EnemyManager.Instance != null) {
             //     EnemyManager.Instance.OnEnemyDied -= HandleEnemyDefeated;
             // }
              // Nên có hàm SaveSkillProgress() ở đây hoặc trong OnApplicationQuit
              // SaveSkillProgress();
         }

        /// <summary>
        /// Hàm xử lý khi nhận được thông tin kẻ địch bị tiêu diệt.
        /// </summary>
        /// <param name="defeatedEnemyData">Dữ liệu của kẻ địch bị đánh bại.</param>
        /// <param name="killer">GameObject đã tiêu diệt kẻ địch (thường là Player).</param>
        public void HandleEnemyDefeated(EnemyData defeatedEnemyData, GameObject killer) // Hàm này cần được gọi từ hệ thống khác
        {
            if (defeatedEnemyData == null || killer == null) return;

            // Kiểm tra xem đối tượng giết có phải là Player không (dựa vào tag hoặc component)
            if (!killer.CompareTag("Player")) // <<< Đảm bảo Player có Tag là "Player"
            {
                 // Hoặc kiểm tra sự tồn tại của một component đặc trưng cho Player
                 // if (killer.GetComponent<PlayerInputReceiver>() == null) return;
                 return; // Bỏ qua nếu không phải Player giết
            }


            var equipment = killer.GetComponent<EquipmentComponent>();
            if (equipment == null || equipment.CurrentWeapon == null)
            {
                 Debug.LogWarning("Killer (Player) không có EquipmentComponent hoặc chưa trang bị vũ khí.", killer);
                 return; // Không có trang bị, không thể tăng exp skill
            }


            // Tính lượng EXP nhận được từ kẻ địch này
            float expGain = CalculateExpGain(defeatedEnemyData);
            if (expGain <= 0) return; // Không có exp để cộng

            Debug.Log($"Player đánh bại {defeatedEnemyData.enemyName}, nhận {expGain} EXP cho các skill phù hợp.");

            // Lặp qua các skill Player đang trang bị
            foreach (var skill in equipment.EquippedSkills)
            {
                if (skill == null) continue; // Bỏ qua skill null trong danh sách

                // Kiểm tra xem skill có tương thích với vũ khí đang dùng không
                // (None có nghĩa là tương thích với mọi loại vũ khí hoặc là skill bị động?)
                if (skill.compatibleWeaponType == WeaponType.None || skill.compatibleWeaponType == equipment.CurrentWeapon.weaponType)
                {
                    // Cộng EXP cho skill này
                    AddExpToSkill(skill, expGain);
                }
            }
        }

        /// <summary>
        /// Tính toán lượng kinh nghiệm nhận được từ việc đánh bại một kẻ địch.
        /// </summary>
        private float CalculateExpGain(EnemyData enemyData)
        {
            // TODO: Thêm logic tính toán phức tạp hơn dựa trên level player, level enemy, loại enemy...
            // return enemyData.experienceReward; // Lấy trực tiếp từ EnemyData nếu đã định nghĩa ở đó
             float baseExp = 50f; // Ví dụ
             return baseExp;
        }

        /// <summary>
        /// Cộng kinh nghiệm cho một skill cụ thể và xử lý việc lên cấp.
        /// </summary>
        private void AddExpToSkill(SkillData skill, float expAmount)
        {
            // Nếu chưa có dữ liệu cho skill này, tạo mới (nghĩa là vừa học?)
            if (!playerSkillProgress.ContainsKey(skill))
            {
                playerSkillProgress[skill] = new SkillProgressData(); // Bắt đầu ở level 1, 0 EXP
                 Debug.Log($"Khởi tạo dữ liệu cho Skill '{skill.skillName}'.");
            }

            // Lấy dữ liệu hiện tại của skill
            SkillProgressData progressData = playerSkillProgress[skill];

            // Cộng EXP
            progressData.CurrentXP += expAmount;
            Debug.Log($"Skill '{skill.skillName}' (Level {progressData.Level}) nhận {expAmount} EXP. Tổng EXP hiện tại: {progressData.CurrentXP}");

            // Kiểm tra lên cấp
            float expToNextLevel = CalculateExpForNextLevel(progressData.Level);
            while (progressData.CurrentXP >= expToNextLevel && expToNextLevel > 0) // Thêm kiểm tra expToNextLevel > 0 để tránh vòng lặp vô hạn nếu công thức trả về 0 hoặc âm
            {
                // Trừ EXP đã dùng để lên cấp
                progressData.CurrentXP -= expToNextLevel;
                // Tăng cấp
                progressData.Level++;

                Debug.Log($"*** Skill '{skill.skillName}' ĐÃ LÊN CẤP {progressData.Level}! ***");

                // Phát sự kiện Skill Leveled Up
                OnSkillLeveledUp?.Invoke(skill, progressData.Level);

                // Tính lại EXP cần cho cấp độ MỚI
                expToNextLevel = CalculateExpForNextLevel(progressData.Level);
            }
        }

        /// <summary>
        /// Tính toán lượng EXP cần thiết để đạt được cấp độ tiếp theo.
        /// </summary>
        /// <param name="currentLevel">Cấp độ hiện tại của skill.</param>
        private float CalculateExpForNextLevel(int currentLevel)
        {
            // TODO: Có thể đặt công thức này trong một SO cấu hình hoặc làm nó phức tạp hơn
            if (currentLevel <= 0) return 100; // Mức EXP cho level 1
            // Ví dụ công thức tuyến tính đơn giản:
             return 100 + (currentLevel * 75); // Ví dụ: Level 1 cần 175, Level 2 cần 250...
            // Ví dụ công thức mũ:
            // return 100f * Mathf.Pow(1.4f, currentLevel); // Tăng theo hàm mũ
        }

        /// <summary>
        /// Lấy cấp độ hiện tại của một skill. Trả về 0 nếu skill chưa được học/theo dõi.
        /// </summary>
        public int GetSkillLevel(SkillData skill)
        {
            if (skill != null && playerSkillProgress.TryGetValue(skill, out SkillProgressData progressData))
            {
                return progressData.Level;
            }
            return 0; // Trả về 0 nếu skill chưa có trong dictionary (chưa được học hoặc chưa có exp)
        }

        // --- TODO: Triển Khai Load/Save ---
        private void LoadSkillProgress()
        {
            Debug.Log("Đang tải dữ liệu Skill Progress... (Chưa triển khai)");
            // Logic đọc dữ liệu từ PlayerPrefs, JSON, Binary file...
            // Ví dụ đọc từ PlayerPrefs (rất cơ bản):
            // foreach (var skillAsset in GetAllSkillDataAssets()) { // Cần hàm lấy tất cả SkillData
            //     if (PlayerPrefs.HasKey($"Skill_{skillAsset.name}_Level")) {
            //         int level = PlayerPrefs.GetInt($"Skill_{skillAsset.name}_Level");
            //         float xp = PlayerPrefs.GetFloat($"Skill_{skillAsset.name}_XP");
            //         playerSkillProgress[skillAsset] = new SkillProgressData { Level = level, CurrentXP = xp };
            //     }
            // }
        }

        public void SaveSkillProgress() // Gọi hàm này khi lưu game
        {
             Debug.Log("Đang lưu dữ liệu Skill Progress... (Chưa triển khai)");
             // Logic lưu dữ liệu vào PlayerPrefs, JSON, Binary file...
             // foreach (var kvp in playerSkillProgress) {
             //     PlayerPrefs.SetInt($"Skill_{kvp.Key.name}_Level", kvp.Value.Level);
             //     PlayerPrefs.SetFloat($"Skill_{kvp.Key.name}_XP", kvp.Value.CurrentXP);
             // }
             // PlayerPrefs.Save();
        }

         // (Helper function - cần triển khai nếu dùng cách load/save ở trên)
         // private IEnumerable<SkillData> GetAllSkillDataAssets() {
         //     // Dùng AssetDatabase (chỉ trong Editor) hoặc Resources.LoadAll
         //     // return Resources.LoadAll<SkillData>("Path/To/SkillDataFolder");
         //     yield break; // Trả về rỗng tạm thời
         // }
    }
}
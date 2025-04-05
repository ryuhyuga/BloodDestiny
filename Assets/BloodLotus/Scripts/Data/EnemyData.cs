using UnityEngine;
using BloodLotus.Core; // Cần cho Stats, AIStateType

// Đảm bảo bạn đang using đúng namespace nếu Stats nằm trong namespace khác
// Ví dụ: using BloodLotus.Core;

// Đặt lại menuName nếu cần
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "BloodLotus/Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName = "New Enemy";
    public Sprite portrait; // Ví dụ thêm icon

    [Header("Stats & Combat")]
    [Tooltip("Chỉ số gốc của kẻ địch này.")]
    public Stats baseStats; // Class Stats từ Core
    [Tooltip("Tầm đánh của kẻ địch (nếu có tấn công).")]
    public float attackRange = 1.5f;
    // public List<EnemyAttackData> attackPatterns; // Có thể định nghĩa các kiểu tấn công ở đây

    [Header("AI Behavior")]
    [Tooltip("Trạng thái AI mặc định khi bắt đầu.")]
    public AIStateType defaultAIState = AIStateType.Idle; // Enum AIStateType từ Core
    [Tooltip("Khoảng cách tối đa kẻ địch có thể phát hiện người chơi.")]
    public float detectionRange = 10f;
    [Tooltip("Bán kính đi tuần tra xung quanh điểm gốc (nếu AI là Patrol).")]
    public float patrolRadius = 5f;

    [Header("Visuals & Audio")]
    [Tooltip("Animator Controller điều khiển animation của kẻ địch.")]
    public RuntimeAnimatorController animatorController;
    // public AudioClip deathSound;
    // public AudioClip hurtSound;

    [Header("Rewards")] // <<< THÊM HEADER CHO GỌN GÀNG
    // <<< THÊM TRƯỜNG NÀY >>>
    [Tooltip("Lượng kinh nghiệm người chơi nhận được khi tiêu diệt kẻ địch này (dùng cho Skill Progression).")]
    public int experienceReward = 10; // <<< Giá trị kinh nghiệm

   // [Tooltip("(Tùy chọn) Tham chiếu đến Scriptable Object LootTableData để xác định vật phẩm rơi ra.")]
    //public LootTableData lootTable; // <<< Vẫn giữ lại để dùng sau

    // Thêm các trường khác nếu cần: resistances, movement speed (nếu không lấy từ baseStats)...
}

// --- Lưu ý: Bạn cũng cần tạo SO LootTableData nếu muốn dùng ---
// Ví dụ cơ bản:
// using UnityEngine;
// using System.Collections.Generic;
// [CreateAssetMenu(fileName = "NewLootTable", menuName = "BloodLotus/Data/Loot Table")]
// public class LootTableData : ScriptableObject {
//     public List<LootDropEntry> possibleDrops;
// }
// [System.Serializable]
// public class LootDropEntry {
//     public ItemData item; // Tham chiếu đến SO ItemData
//     [Range(0f, 1f)] public float dropChance;
//     public int minAmount = 1;
//     public int maxAmount = 1;
// }
// // Và tạo cả ItemData SO...
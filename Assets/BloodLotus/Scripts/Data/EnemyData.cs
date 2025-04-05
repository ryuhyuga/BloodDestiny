using UnityEngine;
using BloodLotus.Core; // Cần cho Stats, AIStateType
// using BloodLotus.Data; // Thêm nếu LootTableData, EnemyAttackData... ở namespace này

// Đảm bảo namespace của Stats là BloodLotus.Core hoặc thêm using tương ứng

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "BloodLotus/Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
 [Header("Basic Info")]
    public string enemyName = "New Enemy";
    [Tooltip("Hình ảnh đại diện hoặc dùng trong UI.")]
    public Sprite portrait;
    [Tooltip("Cấp độ của kẻ địch, ảnh hưởng đến độ khó, EXP thưởng...")]
    public int enemyLevel = 1;

    [Header("Stats & Combat")]
    [Tooltip("Chỉ số gốc (Máu, Sát thương, Giáp...).")]
    public Stats baseStats; // Tham chiếu đến class Stats trong Core
    [Tooltip("Tầm tấn công cận chiến hoặc tầm hiệu quả của kỹ năng.")]
    public float attackRange = 1.5f;
    // [Tooltip("Danh sách các kiểu tấn công/kỹ năng mà kẻ địch này có thể sử dụng.")]
    // public List<EnemyAttackData> attackPatterns; // TODO: Định nghĩa EnemyAttackData SO/class

    [Header("AI Behavior")]
    [Tooltip("Trạng thái AI mặc định khi kẻ địch xuất hiện.")]
    public AIStateType defaultAIState = AIStateType.Idle;
    [Tooltip("Khoảng cách tối đa để phát hiện mục tiêu (thường là Player).")]
    public float detectionRange = 10f;
    [Tooltip("Bán kính di chuyển tuần tra quanh điểm xuất phát (nếu dùng AI Patrol).")]
    public float patrolRadius = 5f;

    [Header("Visuals & Audio")]
    [Tooltip("Animator Controller điều khiển hoạt ảnh.")]
    public RuntimeAnimatorController animatorController;
    // public AudioClip deathSound;
    // public AudioClip hurtSound;
    // public GameObject deathVFX;

    [Header("Rewards")]
    [Tooltip("Lượng kinh nghiệm cơ bản nhận được khi đánh bại. Sẽ được điều chỉnh bởi hệ thống Skill Progression.")]
    public int baseExperienceReward = 50; // <<< Đã có trường này
    //[Tooltip("(Tùy chọn) Bảng vật phẩm có thể rơi ra khi bị đánh bại.")]
    //public LootTableData lootTable; // <<< Giữ lại một khai báo

    // Thêm các trường khác nếu cần:
    // public List<DamageTypeResistance> resistances; // Ví dụ: Kháng các loại sát thương
    // public float aggroRadius; // Tầm kéo aggro đồng đội


}

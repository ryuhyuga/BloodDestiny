using UnityEngine;
using BloodLotus.Data; // Cần cho EnemyData
using BloodLotus.Systems; // Cần cho SkillProgressionSystem
using BloodLotus.Core; // Cần cho StatsComponent

// Gắn vào GameObject của Enemy
[RequireComponent(typeof(StatsComponent))]
// [RequireComponent(typeof(EnemyDataHolder))] // Đảm bảo có cách lấy EnemyData
public class EnemyDeathHandler : MonoBehaviour
{
    private StatsComponent stats;
    private EnemyData enemyData;
    private GameObject lastAttacker;

    [Tooltip("Component chứa thông tin EnemyData.")]
    [SerializeField] private EnemyDataHolder dataHolder;

    void Awake()
    {
        stats = GetComponent<StatsComponent>();

        if (dataHolder == null) dataHolder = GetComponent<EnemyDataHolder>();
        if (dataHolder != null)
        {
             enemyData = dataHolder.enemyData;
        }
        if (enemyData == null) Debug.LogError("EnemyData không tìm thấy hoặc chưa gán!", this);

        if (stats != null)
        {
            stats.OnDied += HandleDeath;
        }
        else
        {
             Debug.LogError("StatsComponent không tìm thấy trên Enemy!", this);
        }
    }

     void OnDestroy() {
         if (stats != null) {
             stats.OnDied -= HandleDeath;
         }
     }

    public void SetLastAttacker(GameObject attacker) {
        if (attacker != null) {
            lastAttacker = attacker;
        }
    }

    private void HandleDeath()
    {
        Debug.LogError("--- EnemyDeathHandler HandleDeath CALLED ---"); 
        if (enemyData == null) {
             Debug.LogError("Không thể xử lý chết vì EnemyData là null!", this);
             return;
        }

        Debug.Log($"{gameObject.name} ({enemyData.enemyName}) is defeated by {(lastAttacker != null ? lastAttacker.name : "Unknown")}!");

        // --- Xử lý khi Enemy chết ---
        // 1. Tắt components
        Collider2D mainCollider = GetComponent<Collider2D>();
        if (mainCollider != null) mainCollider.enabled = false;
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
        // GetComponent<AIBrain>()?.enabled = false;
        // GetComponent<MovementComponent>()?.enabled = false;

        // 2. Chơi animation chết
        var animator = GetComponentInChildren<Animator>();
        if (animator != null) {
             try { animator.SetTrigger("Death"); }
             catch (System.Exception e) { Debug.LogWarning($"Không thể kích hoạt trigger 'Death': {e.Message}", this); }
        }

        // --- XỬ LÝ PHẦN THƯỞNG ---
        if (lastAttacker != null) // Chỉ xử lý phần thưởng nếu biết ai giết
        {
             // 3. Gọi SkillProgressionSystem để cộng EXP SKILL
             if (SkillProgressionSystem.Instance != null)
             {
                 SkillProgressionSystem.Instance.HandleEnemyDefeated(enemyData, lastAttacker);
             } else {
                  Debug.LogWarning("SkillProgressionSystem.Instance bị thiếu, không thể cộng EXP Skill.");
             }

             // 4. <<< THÊM LẠI PHẦN NÀY: CỘNG EXP CHO PLAYER >>>
             if (lastAttacker.CompareTag("Player")) // Kiểm tra chắc chắn là Player
             {
                 var playerStats = lastAttacker.GetComponent<StatsComponent>();
                 if (playerStats != null)
                 {
                     float expReward = enemyData.experienceReward; // Lấy EXP từ EnemyData
                     if (expReward > 0)
                     {
                         Debug.Log($"Attempting to add {expReward} EXP to Player.");
                         playerStats.AddExperience(expReward); // Gọi hàm cộng EXP trên StatsComponent của Player
                     } else {
                         Debug.LogWarning($"Enemy {enemyData.name} có experienceReward <= 0.");
                     }
                 } else {
                      Debug.LogWarning($"Không tìm thấy StatsComponent trên lastAttacker (Player) '{lastAttacker.name}'. Không thể cộng EXP Player.");
                 }
             }
             // ----------------------------------------------

              // 5. TODO: Gọi LootSystem để xử lý rơi đồ
             // LootSystem.Instance?.GenerateLoot(enemyData.lootTable, transform.position);
        } else {
             Debug.LogWarning("lastAttacker là null, không thể xử lý phần thưởng (EXP, Loot).");
        }


        // 6. Hủy GameObject sau một khoảng thời gian
        float destroyDelay = 3f;
        // TODO: Lấy thời gian từ animation chết
        Destroy(gameObject, destroyDelay);
    }
}
using UnityEngine;
using BloodLotus.Data;
using BloodLotus.Systems;
using BloodLotus.Core;

[RequireComponent(typeof(StatsComponent))]
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

    public void SetLastAttacker(GameObject attacker)
    {
        // Debug.Log($"SetLastAttacker called with: {attacker?.name ?? "NULL"}"); // Thêm log nếu cần
        // Chỉ gán nếu attacker là Player (hoặc logic khác tùy game)
        if (attacker != null && attacker.CompareTag("Player")) // <<< Thêm kiểm tra Tag ở đây cho chắc
        {
             lastAttacker = attacker;
        } else {
             // Có thể bạn muốn reset lastAttacker nếu bị tấn công bởi thứ khác?
             // lastAttacker = null;
        }
    }
    private bool deathHandled = false; // Thêm biến cờ ở đầu lớp EnemyDeathHandler

    private void HandleDeath()
    {

         if (deathHandled) return; // Nếu đã xử lý chết rồi thì thoát ngay
            deathHandled = true; // Đặt cờ là đã xử lý

        Debug.LogError($"--- [{this.gameObject.name}] EnemyDeathHandler.HandleDeath CALLED! (Instance ID: {this.GetInstanceID()}) ---");

    // ... Kiểm tra enemyData, lastAttacker ...
        Debug.LogError($"--- [{this.gameObject.name}] EnemyDeathHandler.HandleDeath CALLED! ---");

        // --- Kiểm tra điều kiện tiên quyết ---
        if (enemyData == null) {
             Debug.LogError($"[{this.gameObject.name}] HandleDeath ABORTED: enemyData is null.");
             // Có thể hủy ngay lập tức để tránh lỗi thêm
             // Destroy(gameObject);
             return;
        }
         // --- KIỂM TRA lastAttacker NGAY ĐẦU ---
        if (lastAttacker == null) {
            Debug.LogError($"[{this.gameObject.name}] HandleDeath ABORTED: lastAttacker is null. Không thể xử lý phần thưởng.");
             // Vẫn có thể tiếp tục xử lý animation chết và destroy nếu muốn
        }
        // ------------------------------------

        // Log thông tin người giết (sau khi đã kiểm tra)
        Debug.Log($"{gameObject.name} ({enemyData.enemyName}) was defeated by {lastAttacker?.name ?? "Unknown Attacker"}!");

        // --- Xử lý trạng thái chết ---
        // Tắt components
        Collider2D mainCollider = GetComponent<Collider2D>();
        if (mainCollider != null) mainCollider.enabled = false;
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
        // Tắt AI, Movement...
        // GetComponent<AIBrain>()?.enabled = false;
        // GetComponent<MovementComponent>()?.enabled = false;

        // Chơi animation chết
        var animator = GetComponentInChildren<Animator>();
        if (animator != null) { /* ... SetTrigger("Death") ... */ }

        // --- XỬ LÝ PHẦN THƯỞNG (Chỉ khi có lastAttacker) ---
        if (lastAttacker != null)
        {
            // 1. Cộng EXP cho SKILL (Thông qua SkillProgressionSystem)
            if (SkillProgressionSystem.Instance != null)
            {
                 Debug.Log($"[{this.gameObject.name}] Calling SkillProgressionSystem for {enemyData.enemyName}, killer: {lastAttacker.name}");
                 SkillProgressionSystem.Instance.HandleEnemyDefeated(enemyData, lastAttacker);
            }
            else
            {
                  Debug.LogWarning("SkillProgressionSystem.Instance bị thiếu, không thể cộng EXP Skill.");
            }

            // 2. Cộng EXP cho PLAYER LEVEL (BỎ ĐI NẾU CHỈ MUỐN CỘNG EXP SKILL)
            /* // << BẮT ĐẦU COMMENT OUT PHẦN NÀY
            if (lastAttacker.CompareTag("Player"))
            {
                var playerStats = lastAttacker.GetComponent<StatsComponent>();
                if (playerStats != null)
                {
                    float expReward = enemyData.baseExperienceReward;
                    if (expReward > 0)
                    {
                        Debug.Log($"Attempting to add {expReward} EXP to Player Level.");
                        playerStats.AddExperience(expReward);
                    } else { //... log warning ... }
                } else { //... log warning ... }
            }
            */ // << KẾT THÚC COMMENT OUT PHẦN NÀY

            // 3. TODO: Gọi LootSystem
            // ...
        }
        // Không cần else ở đây vì lỗi thiếu attacker đã được log ở trên

        // Hủy GameObject
        float destroyDelay = 3f;
        Destroy(gameObject, destroyDelay);
    }
}
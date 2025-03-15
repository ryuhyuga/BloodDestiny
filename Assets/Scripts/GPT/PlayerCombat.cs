using UnityEngine;
using System.Collections;


public class PlayerCombat : MonoBehaviour
{
    public PlayerStats playerStats;         // Chứa chỉ số nhân vật (damage, speed...)
    public EquipmentSystem equipmentSystem; // Quản lý vũ khí, skill...
    public Animator animator;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    public float attackRange = 0.5f;


    // Thời gian hồi giữa các đòn
    public float attackCooldown { get; private set; }
    // Đang tấn công (chưa reset cờ animation)
    public bool isAttacking { get; private set; } = false;

    void Update()
    {
        // Giảm cooldown theo thời gian
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown < 0f) attackCooldown = 0f;
        }
    }

    public void PerformAttack(ComboStep step)
    {
        // Nếu vẫn còn cooldown => bỏ qua, log
        if (attackCooldown > 0f)
        {
            Debug.Log("[PlayerCombat] Attack on cooldown!");
            return;
        }

        // 2) Tuỳ ý set param AttackIndex, Attack trigger (nếu Animator transitions)
        animator.SetTrigger("Attack");
        // 1) Cắt animation cũ, chơi clip mới ngay
        // Layer = 0, NormalizedTime = 0f => bắt đầu clip từ đầu
        animator.Play(step.animationName, 0, 0.3f);


        // 3) Đặt cờ đang tấn công
        isAttacking = true;

        // 4) Tính cooldown kiểu hack n’ slash (rất ngắn)
        //   - Đòn đầu (animIndex <= 1) => 0.2s
        //   - Đòn combo sau (animIndex > 1) => 0.1s
        //   - Hoặc tuỳ ý logic
        if (step.animIndex > 1)
            attackCooldown = 0.3f;
        else
            attackCooldown = 0.2f;

        // 5) Tính sát thương
        float damage = playerStats.finalStats.damage * step.damageMultiplier;
        Debug.Log($"[PlayerCombat] Perform attack: {step.animationName}, dmg={damage}");

        // 6) Gây sát thương (nếu cần delay)
        StartCoroutine(DealDamageAfterDelay(damage, 0.1f));

        // 7) Reset cờ tấn công sau step.attackDelay
        StartCoroutine(ResetAttackFlag(step.attackDelay));
    }

    private IEnumerator DealDamageAfterDelay(float damage, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Tìm kẻ địch, gây sát thương...
        // e.g. Raycast or OverlapCircle
       

        // Tìm kẻ địch trong vòng tròn
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
     void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    }

    private IEnumerator ResetAttackFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
        Debug.Log("[PlayerCombat] Attack flag reset");
    }
}

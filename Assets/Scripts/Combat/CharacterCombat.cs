using UnityEngine;
using System.Collections;

public class CharacterCombat : MonoBehaviour
{
    [Header("Attack Configuration")]
    public float baseDamage = 10f;
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    public Transform attackPoint;

    [Header("Attack Timing")]
    public float attackCooldown = 0.5f;
    private bool canAttack = true;

    [Header("Combo System")]
    public int maxComboCount = 3;
    private int currentComboIndex = 0;
    private float lastAttackTime;
    private float comboResetTime = 1f;

    [Header("Attack Types")]
    public AttackType[] attackTypes;

    [System.Serializable]
    public class AttackType
    {
        public string animationName;
        public float damage;
        public float knockbackForce;
    }

    private Animator animator;
    private CharacterStats characterStats;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
    }

    public void Attack()
    {
        if (!canAttack) return;

        // Kiểm tra combo
        if (Time.time - lastAttackTime > comboResetTime)
        {
            currentComboIndex = 0;
        }

        // Thực hiện tấn công
        PerformAttack();

        // Cập nhật thời gian tấn công cuối
        lastAttackTime = Time.time;
        currentComboIndex = (currentComboIndex + 1) % maxComboCount;

        // Bắt đầu cooldown
        StartCoroutine(AttackCooldown());
    }

    void PerformAttack()
    {
        // Chọn loại tấn công dựa trên combo index
        AttackType currentAttack = attackTypes[currentComboIndex];

        // Trigger animation
        if (animator != null)
        {
            animator.SetTrigger(currentAttack.animationName);
        }

        // Phát hiện kẻ địch
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // Xử lý sát thương
        foreach (Collider2D enemy in hitEnemies)
        {
            // Tính toán sát thương
            float finalDamage = CalculateDamage(currentAttack.damage);

            // Gây sát thương cho kẻ địch
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(finalDamage);
            }

            // Áp dụng knockback
            ApplyKnockback(enemy.transform, currentAttack.knockbackForce);
        }
    }

    float CalculateDamage(float baseDamageValue)
    {
        // Tính toán sát thương cuối cùng
        float criticalMultiplier = Random.value < characterStats.criticalRate
            ? characterStats.criticalDamage
            : 1f;

        return baseDamageValue * criticalMultiplier * characterStats.damage;
    }

    void ApplyKnockback(Transform enemyTransform, float knockbackForce)
    {
        // Xác định hướng knockback
        Vector2 knockbackDirection = (enemyTransform.position - transform.position).normalized;

        // Lấy Rigidbody của kẻ địch
        Rigidbody2D enemyRb = enemyTransform.GetComponent<Rigidbody2D>();

        if (enemyRb != null)
        {
            enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Hỗ trợ debug
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

// Script đơn giản cho máu của Enemy
public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Xử lý khi enemy chết
        Destroy(gameObject);
    }
}
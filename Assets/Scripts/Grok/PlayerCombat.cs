using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public WeaponBase equippedWeapon;
    public SkillBase currentSkill;

    private CharacterStats charStats;
    private Animator animator;

    [Header("Attack")]
    public float attackRange = 1f;
    public LayerMask enemyLayer;
    private float attackCooldown = 0f;

    private int requiredLevelForSkillCombo = 2; // Đồng bộ với ComboSystem

    void Start()
    {
        charStats = GetComponent<CharacterStats>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        attackCooldown -= Time.deltaTime;
    }

    public void PerformAttack(ComboStep step)
    {
        if (attackCooldown > 0f) return;

        // Xác định liệu combo có phải là skill hay không
        bool isSkillCombo = currentSkill != null &&
                            currentSkill.CanUseSkill(equippedWeapon) &&
                            currentSkill.skillLevel >= requiredLevelForSkillCombo;

        // Set parameter cho Animator
        animator.SetInteger("AttackIndex", step.animIndex);
        animator.SetBool("IsSkillActive", isSkillCombo);
        animator.SetTrigger("Attack");

        // Tính sát thương
        float baseDmg = step.damageMultiplier * charStats.finalStats.damage;
        if (step.forceCrit)
        {
            baseDmg *= charStats.finalStats.critDamage;
        }
        else
        {
            bool isCrit = (Random.value < charStats.finalStats.critRate);
            if (isCrit) baseDmg *= charStats.finalStats.critDamage;
        }

        // Gây sát thương
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D e in enemies)
        {
            e.GetComponent<EnemyHealth>()?.TakeDamage(baseDmg);
        }

        attackCooldown = 1f / charStats.finalStats.attackSpeed;
    }
}
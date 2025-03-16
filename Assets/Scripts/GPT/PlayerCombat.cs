using UnityEngine;
using System.Collections;


public class PlayerCombat : MonoBehaviour
{
    public WeaponBase currentWeapon;
    public SkillBase currentSkill;
    private PlayerCombat playerCombat;

    public PlayerStats playerStats;
    public EquipmentSystem equipmentSystem;
    public Animator playerAnimator;
    public LayerMask enemyLayers;
    public Transform attackPoint;
    public float attackRange = 0.5f;

    public float attackCooldown { get; private set; }
    public bool isAttacking { get; private set; } = false;

    private void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        equipmentSystem = GetComponent<EquipmentSystem>(); // Gán giá trị EquipmentSys
    }

    private void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>(); // Tìm Animator trong con
    }

    void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown < 0f) attackCooldown = 0f;
        }
    }

    // Phương thức để trang bị vũ khí
    public void EquipWeapon(WeaponBase weapon)
    {
        currentWeapon = weapon;
        if (weapon != null && weapon.overrideController != null)
        {
            playerAnimator.runtimeAnimatorController = weapon.overrideController;
            Debug.Log("[PlayerCombat] Animator overridden by weapon: " + weapon.weaponName);
        }
        else
        {
            Debug.LogWarning("[PlayerCombat] No override controller for weapon: " + weapon.weaponName);

        }
    }

    // Phương thức để trang bị skill (đã hợp nhất)
    public void EquipSkill(SkillBase skill)
    {
        currentSkill = skill; // Cập nhật skill hiện tại
        if (skill != null && equipmentSystem?.currentWeapon != null && skill.IsCompatibleWith(equipmentSystem.currentWeapon.weaponType))
        {
            if (skill.overrideController != null)
            {
                playerAnimator.runtimeAnimatorController = skill.overrideController;
                Debug.Log("[PlayerCombat] Animator overridden by skill: " + skill.skillName);
            }
            else
            {
                Debug.LogWarning("[PlayerCombat] No override controller for skill: " + skill.skillName);
            }
        }
        else
        {
            Debug.Log("[PlayerCombat] Skill not compatible with current weapon or no weapon equipped.");
        }
    }

    public void PerformAttack(ComboStep step)
    {
        if (attackCooldown > 0f)
        {
            Debug.Log("[PlayerCombat] Attack on cooldown!");
            return;
        }

        playerAnimator.SetTrigger("Attack");
        playerAnimator.Play(step.animationName, 0, 0.3f);

        isAttacking = true;
        attackCooldown = step.animIndex > 1 ? 0.3f : 0.2f;

        float damage = playerStats.finalStats.damage * step.damageMultiplier;
        Debug.Log($"[PlayerCombat] Perform attack: {step.animationName}, dmg={damage}");

        StartCoroutine(DealDamageAfterDelay(damage, 0.1f));
        StartCoroutine(ResetAttackFlag(step.attackDelay));
    }

    private IEnumerator DealDamageAfterDelay(float damage, float delay)
    {
        yield return new WaitForSeconds(delay);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private IEnumerator ResetAttackFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
        Debug.Log("[PlayerCombat] Attack flag reset");
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
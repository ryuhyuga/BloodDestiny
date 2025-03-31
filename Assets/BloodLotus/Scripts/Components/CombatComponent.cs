using UnityEngine;
using BloodLotus.Core;
using BloodLotus.Data;

[RequireComponent(typeof(StatsComponent))]
public class CombatComponent : MonoBehaviour
{    [Header("Component References")]
    private StatsComponent stats;
    private EquipmentComponent equipment;

    [Header("Hitbox Configuration")]
    // KHÔNG cần attackHitboxObject nữa
    // [SerializeField] private GameObject attackHitboxObject;
    // private bool isHitboxActive = false;

    [Header("Combat State")]
    private ComboStepData currentActiveStep;
    private float hitstopTimer = 0f;
    public bool IsPerformingAttackAction { get; private set; } = false;

    void Awake()
    {
        stats = GetComponent<StatsComponent>();
        equipment = GetComponent<EquipmentComponent>();

        // KHÔNG cần kiểm tra attackHitboxObject nữa
        // if (attackHitboxObject == null) { ... }
    }

    void Update()
    {
        if (hitstopTimer > 0)
        {
            if (Time.timeScale > 0f) Time.timeScale = 0f;
            hitstopTimer -= Time.unscaledDeltaTime;
            if (hitstopTimer <= 0)
            {
                Time.timeScale = 1.0f;
            }        }
    }

    public void InitiateAttackStep(ComboStepData stepData)
    {
        currentActiveStep = stepData;
        IsPerformingAttackAction = true;
        PerformAttack(); // Gọi PerformAttack ngay khi bắt đầu step
    }

    private void PerformAttack()
    {
        if (equipment == null || equipment.CurrentWeapon == null) return;

        WeaponData weapon = equipment.CurrentWeapon;

        switch (weapon.weaponType)
        {
            case WeaponType.Bow: // Hoặc WeaponType.Staff nếu bạn dùng Staff làm vũ khí tầm xa
                FireProjectile(weapon);
                break;
            default: // Các loại vũ khí cận chiến
                PerformMeleeAttack(weapon);
                break;
        }
    }

    private void PerformMeleeAttack(WeaponData weapon)
    {
        // Logic tấn công cho vũ khí cận chiến (Sword, Spear, Dagger)
        float attackRange = weapon.meleeAttackRange; // Lấy từ WeaponData
        Vector2 attackOrigin = transform.position; // Vị trí bắt đầu tấn công
        // Debug.Log($"Performing Melee Attack with range: {attackRange}");

        // Tạo hitbox hình tròn và tìm các đối tượng trong phạm vi
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackOrigin, attackRange, LayerMask.GetMask("Enemy"));

        // Duyệt qua các đối tượng trúng đòn và gây sát thương
        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null && hitCollider.gameObject != this.gameObject)
            {
                // Tính toán sát thương (ví dụ đơn giản)
                float damageAmount = stats.Damage * currentActiveStep.damageMultiplier;
                damageable.TakeDamage(damageAmount, DamageType.Physical, this.gameObject);

                // Áp dụng hiệu ứng và hitstop (nếu có)
                ApplyEffects(hitCollider.gameObject);                ApplyHitstop(currentActiveStep.hitstopDuration);

                // Hiệu ứng hình ảnh/âm thanh
                if (currentActiveStep.vfxOnHitPrefab != null)
                {
                    Vector3 hitPoint = hitCollider.ClosestPoint(transform.position);
                    Instantiate(currentActiveStep.vfxOnHitPrefab, hitPoint, Quaternion.identity);
                }
                if (currentActiveStep.sfxOnHit != null)
                {
                    AudioSource.PlayClipAtPoint(currentActiveStep.sfxOnHit, transform.position);
                }
            }
        }
    }

    private void FireProjectile(WeaponData weapon)
    {
        // Logic bắn đạn cho vũ khí tầm xa (Bow, Staff)
        if (weapon.projectilePrefab == null)
        {
            Debug.LogWarning($"Vũ khí {weapon.weaponName} thiếu projectilePrefab!");
            return;
        }

        // Tạo projectile
        GameObject projectile = Instantiate(weapon.projectilePrefab, transform.position, transform.rotation); // Dùng rotation của player
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        // Thiết lập các thông số cho projectile
        if (projectileComponent != null)
        {
            projectileComponent.damage = stats.Damage * currentActiveStep.damageMultiplier; // Sát thương từ stats và step
            projectileComponent.lifespan = weapon.projectileLifespan; // Thời gian tồn tại
            projectileComponent.owner = this.gameObject; // Gán người sở hữu (để tránh tự bắn vào mình)
        }

        // Bắn projectile theo hướng của nhân vật
        if (projectileRb != null)
        {            projectileRb.linearVelocity = transform.right * weapon.projectileSpeed; // Dùng transform.right
        }
    }

    private void ApplyEffects(GameObject target)
    {
        bool effectApplied = false;

        if (currentActiveStep != null && currentActiveStep.effectType != EffectType.None)
        {
            if (Random.value <= currentActiveStep.effectChance)
            {
                Debug.Log($"Applying effect {currentActiveStep.effectType} from Combo Step to {target.name}");
                // target.GetComponent<StatusEffectComponent>()?.ApplyEffect(currentActiveStep);
                effectApplied = true;
            }
        }

        if (!effectApplied && equipment?.CurrentInnerPower?.effectOnHit != EffectType.None)
        {
            if (Random.value <= equipment.CurrentInnerPower.effectChance)
            {                Debug.Log($"Applying effect {equipment.CurrentInnerPower.effectOnHit} from Inner Power to {target.name}");
                // target.GetComponent<StatusEffectComponent>()?.ApplyEffect(equipment.CurrentInnerPower);
                effectApplied = true;
            }
        }
    }    private void ApplyHitstop(float duration)
    {
        if (duration > 0 && Time.timeScale > 0)
        {
            hitstopTimer = duration;
        }
    }

    public void OnAttackAnimationEnd()
    {
        IsPerformingAttackAction = false;
    }

    public void FinishAttackSequence()
    {
        // Logic dọn dẹp nếu cần
    }

    // Vẽ phạm vi tấn công trong Scene View
    private void OnDrawGizmosSelected()
    {
        if (equipment != null && equipment.CurrentWeapon != null)
        {
            WeaponData weapon = equipment.CurrentWeapon;
            Gizmos.color = Color.red;
            if (weapon.weaponType != WeaponType.Bow)
            {
                Gizmos.DrawWireSphere(transform.position, weapon.meleeAttackRange);            }
            // Không vẽ gizmo cho projectile
        }
    }
}
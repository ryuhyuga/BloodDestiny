using UnityEngine;
using BloodLotus.Core;
using BloodLotus.Data;


[RequireComponent(typeof(StatsComponent))]
public class CombatComponent : MonoBehaviour
{
    [Header("Component References")]
    private StatsComponent stats;
    private EquipmentComponent equipment;
    // Không cần tham chiếu PlayerAnimationComponent ở đây nữa nếu các hàm AE gọi trực tiếp

    [Header("Melee Hitbox Configuration")]
    [Tooltip("GameObject chứa Collider Hitbox cận chiến (là con của Player). Sẽ được bật/tắt bởi Animation Events.")]
    [SerializeField] private GameObject meleeHitboxObject; // Đổi tên cho rõ
    private bool isMeleeHitboxActive = false;

    [Header("Combat State")]
    private ComboStepData currentActiveStep; // Step đang có hiệu lực (cho cả melee và projectile)
    private float hitstopTimer = 0f;
    public bool IsPerformingAttackAction { get; private set; } = false; // Đang trong hành động tấn công

    void Awake()
    {
        stats = GetComponent<StatsComponent>();
        equipment = GetComponent<EquipmentComponent>();

        // Kiểm tra và tắt hitbox melee ban đầu
        if (meleeHitboxObject == null)
        {
            Debug.LogWarning("Melee Hitbox Object chưa được gán trong CombatComponent! Tấn công cận chiến sẽ không hoạt động.", this);
        }
        else
        {
            meleeHitboxObject.SetActive(false);
            isMeleeHitboxActive = false;
        }
    }

    void Update()
    {
        // Xử lý Hitstop (giữ nguyên)
        if (hitstopTimer > 0)
        {
            if (Time.timeScale > 0f) Time.timeScale = 0f;
            hitstopTimer -= Time.unscaledDeltaTime;
            if (hitstopTimer <= 0)
            {
                Time.timeScale = 1.0f;
            }
        }
    }

    /// <summary>
    /// Khởi tạo thông tin cho bước tấn công. Được gọi bởi ComboComponent.
    /// </summary>
    public void InitiateAttackStep(ComboStepData stepData)
    {
        currentActiveStep = stepData;
        IsPerformingAttackAction = true;
        // Không bật/tắt hitbox hay bắn đạn ở đây. Chờ Animation Event.
    }

    // --- Xử Lý Tấn Công Cận Chiến (Melee) ---

    /// <summary>
    /// Được gọi bởi Animation Event (AE_ActivateHitbox) để bật hitbox cận chiến.
    /// </summary>
    public void ActivateMeleeHitbox() // Đổi tên hàm cho rõ
    {
        // Chỉ bật hitbox nếu đang thực hiện tấn công và có hitbox object
        if (IsPerformingAttackAction && meleeHitboxObject != null)
        {
            // Debug.Log("Activating Melee Hitbox");
            meleeHitboxObject.SetActive(true);
            isMeleeHitboxActive = true;
            // Reset danh sách mục tiêu đã trúng trong cú vung này (nếu cần)
            // hitTargetsThisSwing.Clear();
        }
    }

    /// <summary>
    /// Được gọi bởi Animation Event (AE_DeactivateHitbox) để tắt hitbox cận chiến.
    /// </summary>
    public void DeactivateMeleeHitbox() // Đổi tên hàm cho rõ
    {
        if (meleeHitboxObject != null)
        {
             // Debug.Log("Deactivating Melee Hitbox");
            meleeHitboxObject.SetActive(false);
            isMeleeHitboxActive = false;
        }
    }

    /// <summary>
    /// Được gọi bởi script Hitbox.cs khi có va chạm Trigger (chỉ dành cho Melee).
    /// </summary>
    public void HandleMeleeHit(Collider2D other) // Đổi tên hàm cho rõ
    {
        Debug.LogError($"[CombatComp] HandleMeleeHit called with {other.name}. IsPerforming: {IsPerformingAttackAction}, Step: {currentActiveStep?.stepName ?? "NULL"}"); // Dùng LogError
        // Chỉ xử lý nếu hitbox melee đang active và đang thực hiện tấn công
        if (!isMeleeHitboxActive || !IsPerformingAttackAction || currentActiveStep == null) 
        {
               Debug.LogWarning("[CombatComp] HandleMeleeHit: Ignoring hit - Not Performing Attack or Step is Null.");
        
            // TODO: Kiểm tra nếu `other` đã bị đánh trúng trong cú vung này chưa?
             return;
        }
      

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject != this.gameObject)
        {
            
            Debug.LogError($"[CombatComp] >>> Calling TakeDamage on {other.name} <<<");
            // Tính sát thương dựa trên Step hiện tại
            float baseDmg = stats.Damage;
            float multiplier = currentActiveStep.damageMultiplier;
            float finalDamage = baseDmg * multiplier; // Thêm các bonus khác nếu cần

             Debug.Log($"[CombatComp] Calculated Damage: {finalDamage} (Base: {baseDmg}, Multiplier: {multiplier})"); // Thêm log chi tiết
            damageable.TakeDamage(finalDamage, DamageType.Physical, this.gameObject); // Truyền finalDamage vào đây

            Debug.Log($"Melee Hit {other.name}! Dealing {finalDamage} Physical damage.");
            damageable.TakeDamage(finalDamage, DamageType.Physical, this.gameObject);

            ApplyEffects(other.gameObject); // Áp dụng hiệu ứng
            ApplyHitstop(currentActiveStep.hitstopDuration); // Áp dụng hitstop

            // Tạo VFX/SFX tại điểm va chạm
            if (currentActiveStep.vfxOnHitPrefab != null)
            {
                Vector3 hitPoint = other.ClosestPoint(meleeHitboxObject.transform.position);
                Instantiate(currentActiveStep.vfxOnHitPrefab, hitPoint, Quaternion.identity);
            }
             if (currentActiveStep.sfxOnHit != null) AudioSource.PlayClipAtPoint(currentActiveStep.sfxOnHit, transform.position);

            // Không tắt hitbox ở đây, để AE_DeactivateMeleeHitbox xử lý
        }
    }

    // --- Xử Lý Tấn Công Tầm Xa (Projectile) ---

    /// <summary>
    /// Được gọi bởi Animation Event (ví dụ: AE_FireProjectile) để bắn đạn.
    /// </summary>
    public void FireProjectileNow()
    {
        // Chỉ bắn nếu đang thực hiện tấn công và có thông tin step
        if (!IsPerformingAttackAction || currentActiveStep == null || equipment?.CurrentWeapon == null) return;

        WeaponData weapon = equipment.CurrentWeapon;

        // Kiểm tra xem vũ khí có đúng là loại bắn đạn và có prefab không
        if ((weapon.weaponType == WeaponType.Bow /*|| weapon.weaponType == WeaponType.Staff*/) && weapon.projectilePrefab != null)
        {
             Debug.Log("Firing Projectile!");
            // Tạo projectile
            GameObject projectileGO = Instantiate(weapon.projectilePrefab, CalculateProjectileSpawnPoint(), CalculateProjectileRotation());
            Projectile projectileComponent = projectileGO.GetComponent<Projectile>();
            Rigidbody2D projectileRb = projectileGO.GetComponent<Rigidbody2D>();

            // Thiết lập thông số projectile
            if (projectileComponent != null)
            {
                // Sát thương có thể lấy từ step hoặc vũ khí gốc tùy thiết kế
                projectileComponent.damage = stats.Damage * currentActiveStep.damageMultiplier;
                projectileComponent.lifespan = weapon.projectileLifespan;
                projectileComponent.owner = this.gameObject;
                // projectileComponent.damageType = DamageType.Physical; // Hoặc lấy từ weapon/step
            }

            // Bắn đạn
            if (projectileRb != null)
            {
                // Lấy hướng bắn (thường là hướng player đang nhìn)
                Vector2 fireDirection = transform.right; // Dùng transform.right nếu flip bằng Rotate(0,180,0)
                // Hoặc: Vector2 fireDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left; // Nếu flip bằng scale

                projectileRb.linearVelocity = fireDirection * weapon.projectileSpeed;
            }
        }
        else
        {
             Debug.LogWarning("Attempted to fire projectile, but weapon type is not ranged or projectilePrefab is missing.", this);
        }
    }

    // (Optional) Hàm tính toán điểm spawn projectile (ví dụ: trước mặt nhân vật)
    private Vector3 CalculateProjectileSpawnPoint()
    {
        // Ví dụ: Spawn cách tâm nhân vật 0.5 unit về phía trước
        return transform.position + transform.right * 0.5f;
    }

     // (Optional) Hàm tính toán hướng quay ban đầu của projectile
     private Quaternion CalculateProjectileRotation() {
         // Quay projectile theo hướng bắn (hướng transform.right)
         float angle = Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;
         return Quaternion.Euler(0f, 0f, angle);
         // Hoặc đơn giản là: return transform.rotation; nếu prefab đã quay đúng hướng
     }


    // --- Hàm Chung ---

    private void ApplyEffects(GameObject target) // Giữ nguyên logic ApplyEffects
    {
        bool effectApplied = false;
        InnerPowerData currentPower = equipment?.CurrentInnerPower;

        if (currentActiveStep != null && currentActiveStep.effectType != EffectType.None)
        {
            if (Random.value <= currentActiveStep.effectChance)
            {
                // Debug.Log(...);
                 target.GetComponent<StatusEffectReceiver>()?.ApplyStatusEffect(currentActiveStep);
                effectApplied = true;
            }
        }
        if (!effectApplied && currentPower != null && currentPower.effectOnHit != EffectType.None)
        {
            if (Random.value <= currentPower.effectChance)
            {
                // Debug.Log(...);
                target.GetComponent<StatusEffectReceiver>()?.ApplyStatusEffect(currentPower);
            }
        }
    }

    private void ApplyHitstop(float duration) // Giữ nguyên logic ApplyHitstop
    {
        if (duration > 0 && Time.timeScale > 0)
        {
            hitstopTimer = duration;
        }
    }

    /// <summary>
    /// Được gọi bởi Animation Event (AE_AttackAnimationFinished) khi animation tấn công kết thúc.
    /// </summary>
    public void OnAttackAnimationEnd() // Giữ nguyên logic OnAttackAnimationEnd
    {
        // Debug.Log("Attack Animation Ended");
        IsPerformingAttackAction = false;
        // Không tắt hitbox ở đây, AE_DeactivateMeleeHitbox đã làm
    }

    // Không cần FinishAttackSequence nữa
    // public void FinishAttackSequence() { }

    // --- Gizmos ---
    private void OnDrawGizmosSelected() // Giữ nguyên để vẽ phạm vi Melee
    {
        if (equipment != null && equipment.CurrentWeapon != null)
        {
            WeaponData weapon = equipment.CurrentWeapon;
            if (weapon.weaponType != WeaponType.Bow /*&& weapon.weaponType != WeaponType.Staff*/)
            {
                 Gizmos.color = Color.red;
                 Gizmos.DrawWireSphere(transform.position, weapon.meleeAttackRange);
            }
        }
    }
}
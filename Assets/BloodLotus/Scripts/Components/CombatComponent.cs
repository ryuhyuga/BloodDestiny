using UnityEngine;
using BloodLotus.Core; // Cần cho IDamageable, DamageType, EffectType
using BloodLotus.Data; // Cần cho ComboStepData

// Gắn vào cùng GameObject với StatsComponent, ComboComponent, PlayerAnimationComponent
[RequireComponent(typeof(StatsComponent))]
public class CombatComponent : MonoBehaviour
{
    [Header("Component References")]
    private StatsComponent stats;
    private EquipmentComponent equipment; // Cần để lấy InnerPower

    [Header("Hitbox Configuration")]
    [SerializeField] private GameObject attackHitboxObject; // GameObject chứa Collider Hitbox (là con của Player)
    private bool isHitboxActive = false; // Theo dõi trạng thái hitbox

    [Header("Combat State")]
    private ComboStepData currentActiveStep; // Lưu thông tin của bước combo đang có hiệu lực (để HandleHit dùng)
    private float hitstopTimer = 0f;
    // Biến IsAttacking giờ đây có thể mang ý nghĩa "đang trong một hành động tấn công"
    // hơn là "không thể nhận lệnh tấn công mới". Tùy thuộc bạn muốn dùng nó để làm gì.
    // Ví dụ: ngăn di chuyển khi đang tấn công?
    public bool IsPerformingAttackAction { get; private set; } = false;


    void Awake()
    {
        stats = GetComponent<StatsComponent>();
        equipment = GetComponent<EquipmentComponent>(); // Lấy EquipmentComponent

        if (attackHitboxObject == null)
        {
            Debug.LogError("Attack Hitbox Object chưa được gán trong CombatComponent!", this);
        }
        else
        {
            // Đảm bảo hitbox tắt khi bắt đầu
            attackHitboxObject.SetActive(false);
            isHitboxActive = false;
        }
    }

    void Update()
    {
        // Xử lý Hitstop (Giữ nguyên)
        if (hitstopTimer > 0)
        {
            // Nên kiểm tra nếu game đã pause vì lý do khác
            if (Time.timeScale > 0f) Time.timeScale = 0f; // Chỉ dừng nếu chưa dừng
            hitstopTimer -= Time.unscaledDeltaTime;
            if (hitstopTimer <= 0)
            {
                Time.timeScale = 1.0f; // Chỉ resume nếu nó đang bị dừng bởi hitstop
            }
        }
    }

    /// <summary>
    /// Khởi tạo thông tin cho bước tấn công sắp tới. Được gọi bởi ComboComponent.
    /// </summary>
    /// <param name="stepData">Thông tin của bước combo.</param>
    public void InitiateAttackStep(ComboStepData stepData)
    {
        // Lưu lại thông tin step này để HandleHit và các hàm khác sử dụng
        currentActiveStep = stepData;
        IsPerformingAttackAction = true; // Đánh dấu đang thực hiện hành động tấn công
        // Không bật hitbox ở đây, chờ Animation Event
    }

    /// <summary>
    /// Được gọi bởi Animation Event (AE_ActivateHitbox) để bật hitbox.
    /// </summary>
    public void ActivateCurrentAttackHitbox()
    {
        if (attackHitboxObject != null && currentActiveStep != null) // Chỉ bật khi có step hợp lệ
        {
            // Debug.Log("Activating Hitbox");
            attackHitboxObject.SetActive(true);
            isHitboxActive = true;
            // Reset danh sách đối tượng đã đánh trúng trong lần kích hoạt này (nếu cần để tránh đánh 1 đối tượng nhiều lần/1 hit)
            // hitTargetsThisSwing.Clear();
        }
    }

    /// <summary>
    /// Được gọi bởi Animation Event (AE_DeactivateHitbox) để tắt hitbox.
    /// </summary>
    public void DeactivateCurrentAttackHitbox()
    {
        if (attackHitboxObject != null)
        {
             // Debug.Log("Deactivating Hitbox");
            attackHitboxObject.SetActive(false);
            isHitboxActive = false;
        }
    }

    /// <summary>
    /// Được gọi bởi script Hitbox.cs khi có va chạm Trigger.
    /// </summary>
    public void HandleHit(Collider2D other)
    {
        // Chỉ xử lý hit nếu hitbox đang active và có thông tin step hiện tại
        if (!isHitboxActive || currentActiveStep == null) return;

        // TODO: Thêm cơ chế kiểm tra xem đối tượng 'other' đã bị đánh trúng trong lần vung này chưa (nếu cần)
        // if (hitTargetsThisSwing.Contains(other.gameObject)) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        // Kiểm tra xem có phải đối tượng nhận sát thương không và không phải là chính mình
        if (damageable != null && other.gameObject != this.gameObject)
        {
             // Đánh dấu đã đánh trúng đối tượng này trong lần vung này
             // hitTargetsThisSwing.Add(other.gameObject);

            // --- Tính Toán Sát Thương ---
            float baseDmg = stats != null ? stats.Damage : 0f;
            // float magicDmg = stats != null ? stats.MagicDamage : 0f; // Lấy từ skill?
            float multiplier = currentActiveStep.damageMultiplier;
            // TODO: Tính thêm bonus từ InnerPower, Artifacts, Crit...
            float finalPhysicalDamage = baseDmg * multiplier;

            Debug.Log($"Hit {other.name}! Dealing {finalPhysicalDamage} Physical damage.");

            // --- Gây Sát Thương ---
            damageable.TakeDamage(finalPhysicalDamage, DamageType.Physical, this.gameObject);
            // if (magicDmg > 0) damageable.TakeDamage(magicDmg, DamageType.Magical, this.gameObject);

            // --- Áp Dụng Hiệu Ứng ---
            ApplyEffects(other.gameObject); // Áp dụng hiệu ứng từ Inner Power / Step hiện tại

            // --- Kích Hoạt Hitstop ---
            ApplyHitstop(currentActiveStep.hitstopDuration);

            // --- Tạo Hiệu Ứng Hình Ảnh/Âm Thanh ---
            if (currentActiveStep.vfxOnHitPrefab != null)
            {
                // Lấy điểm va chạm gần nhất để tạo hiệu ứng chính xác hơn
                Vector3 hitPoint = other.ClosestPoint(attackHitboxObject.transform.position);
                Instantiate(currentActiveStep.vfxOnHitPrefab, hitPoint, Quaternion.identity);
            }
            if (currentActiveStep.sfxOnHit != null)
            {
                // Nên dùng hệ thống Audio Manager thay vì PlayClipAtPoint nếu có thể
                 AudioSource.PlayClipAtPoint(currentActiveStep.sfxOnHit, transform.position);
            }

            // KHÔNG nên tắt hitbox ngay lập tức ở đây, vì một cú vung có thể trúng nhiều địch.
            // Hãy để Animation Event AE_DeactivateHitbox xử lý việc tắt.
            // DeactivateCurrentAttackHitbox();
        }
    }

    private void ApplyEffects(GameObject target)
    {
        bool effectApplied = false; // Cờ để tránh áp dụng nhiều hiệu ứng cùng lúc?

        // 1. Hiệu ứng từ Combo Step Data (Ưu tiên?)
        if (currentActiveStep != null && currentActiveStep.effectType != EffectType.None)
        {
            if (Random.value <= currentActiveStep.effectChance)
            {
                Debug.Log($"Applying effect {currentActiveStep.effectType} from Combo Step to {target.name}");
                // target.GetComponent<StatusEffectComponent>()?.ApplyEffect(currentActiveStep); // Cần class/struct định nghĩa hiệu ứng
                effectApplied = true;
            }
        }

        // 2. Hiệu ứng từ Inner Power (Chỉ áp dụng nếu chưa có hiệu ứng từ Step?)
        if (!effectApplied && equipment?.CurrentInnerPower?.effectOnHit != EffectType.None)
        {
            if (Random.value <= equipment.CurrentInnerPower.effectChance)
            {
                Debug.Log($"Applying effect {equipment.CurrentInnerPower.effectOnHit} from Inner Power to {target.name}");
                // target.GetComponent<StatusEffectComponent>()?.ApplyEffect(equipment.CurrentInnerPower);
                effectApplied = true;
            }
        }

        // TODO: Áp dụng hiệu ứng từ SkillData nếu combo được mở rộng bởi skill
    }

    private void ApplyHitstop(float duration)
    {
        if (duration > 0 && Time.timeScale > 0) // Chỉ áp dụng nếu có thời gian và game chưa bị dừng
        {
            hitstopTimer = duration; // Bộ đếm sẽ xử lý trong Update
        }
    }

    /// <summary>
    /// Được gọi bởi Animation Event (AE_AttackAnimationFinished) khi animation tấn công kết thúc.
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        // Debug.Log("Attack Animation Ended");
        // Reset trạng thái đang thực hiện hành động tấn công.
        // Điều này cho phép các hành động khác (như di chuyển bình thường) có thể được thực hiện trở lại.
        IsPerformingAttackAction = false;

        // Quan trọng: Không nên reset currentActiveStep ở đây,
        // vì HandleHit có thể vẫn cần thông tin step nếu hitbox còn active trong vài frame cuối.
        // Việc reset currentActiveStep nên xảy ra khi InitiateAttackStep được gọi cho bước MỚI,
        // hoặc khi combo hoàn toàn reset bởi ComboComponent.

        // Cũng không nên tắt hitbox ở đây vì AE_DeactivateHitbox đã làm việc đó.
    }

     // Hàm FinishAttack cũ có thể không cần thiết nữa.
     // public void FinishAttack() { ... }

     // Hàm FinishAttackSequence được gọi từ ComboComponent cũng có thể không cần.
     public void FinishAttackSequence() {
         // Có thể dùng để dọn dẹp gì đó nếu cần, nhưng với logic mới thì ít tác dụng.
         // IsPerformingAttackAction = false; // Đã được xử lý bởi OnAttackAnimationEnd
     }
}
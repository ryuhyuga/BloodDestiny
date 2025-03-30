using UnityEngine;

using BloodLotus.Core; // Thư viện chứa các enum, struct, class dùng chung
using BloodLotus.Data; // Thư viện chứa các ScriptableObject dùng chung
public class CombatComponent : MonoBehaviour
{
    private StatsComponent stats;
    private ComboComponent comboComponent;
    private PlayerAnimationComponent playerAnim; // Hoặc AnimationComponent chung

    public bool IsAttacking { get; private set; } = false;
    private ComboStepData currentAttackStep;
    private float hitstopTimer = 0f;

    // Tham chiếu đến các Hitbox (có thể là child object với Collider2D và script Hitbox.cs)
    [SerializeField] private GameObject attackHitbox; // Hoặc list các hitbox

    private void Awake()
    {
        stats = GetComponent<StatsComponent>();
        comboComponent = GetComponent<ComboComponent>();
        playerAnim = GetComponent<PlayerAnimationComponent>();
        if (attackHitbox) attackHitbox.SetActive(false); // Tắt hitbox ban đầu
    }

     private void Update()
     {
        // Xử lý Hitstop
        if (hitstopTimer > 0)
        {
            Time.timeScale = 0f; // Dừng game tạm thời
            hitstopTimer -= Time.unscaledDeltaTime; // Đếm ngược bằng thời gian thực
            if (hitstopTimer <= 0)
            {
                Time.timeScale = 1.0f; // Trả lại tốc độ game
            }
        }
     }

    public bool CanAttack()
    {
        // Kiểm tra điều kiện có thể tấn công (không bị stun, không đang làm hành động khác ưu tiên hơn...)
        return !IsAttacking && Time.timeScale > 0f; // Chưa tấn công và không bị hitstop
    }

    // Gọi từ ComboComponent khi một bước combo được thực thi
    public void ProcessAttackStep(ComboStepData stepData)
    {
        IsAttacking = true;
        currentAttackStep = stepData;
        // Kích hoạt Hitbox vào đúng thời điểm trong animation (có thể dùng Animation Event)
         // Ví dụ: StartCoroutine(ActivateHitboxForDuration(0.1f, 0.3f)); // Kích hoạt từ 0.1s đến 0.3s
        ActivateHitbox(); // Kích hoạt tạm thời, cần cơ chế chính xác hơn
    }

    // Gọi từ Animation Event hoặc Coroutine
    public void ActivateHitbox()
    {
        if (attackHitbox) attackHitbox.SetActive(true);
    }

    // Gọi từ Animation Event hoặc Coroutine
    public void DeactivateHitbox()
    {
        if (attackHitbox) attackHitbox.SetActive(false);
    }

    // Gọi khi Hitbox va chạm với đối tượng có Hurtbox (IDamageable)
    public void HandleHit(Collider2D other)
    {
        if (!IsAttacking || currentAttackStep == null) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && other.gameObject != this.gameObject) // Đảm bảo không tự đánh mình
        {
            // Tính sát thương cuối cùng
            float baseDmg = stats ? stats.Damage : 0; // Lấy damage vật lý
            float magicDmg = stats ? stats.MagicDamage : 0; // Lấy damage phép từ skill?
            float multiplier = currentAttackStep.damageMultiplier;
            // TODO: Thêm logic tính damage từ Inner Power, Artifacts, Crit...
            float finalPhysicalDamage = baseDmg * multiplier;
            // float finalMagicalDamage = magicDmg; // Damage phép từ skill

            Debug.Log($"Dealing {finalPhysicalDamage} Physical damage to {other.name}");
            damageable.TakeDamage(finalPhysicalDamage, DamageType.Physical, this.gameObject);
            // if (finalMagicalDamage > 0) damageable.TakeDamage(finalMagicalDamage, DamageType.Magical, this.gameObject);

             // Áp dụng hiệu ứng từ Inner Power/Skill
             ApplyEffects(other.gameObject);

            // Kích hoạt Hitstop
            ApplyHitstop(currentAttackStep.hitstopDuration);

            // Tạo hiệu ứng VFX, SFX tại điểm va chạm
            if (currentAttackStep.vfxOnHitPrefab)
                Instantiate(currentAttackStep.vfxOnHitPrefab, other.transform.position, Quaternion.identity);
            // if (currentAttackStep.sfxOnHit) AudioSource.PlayClipAtPoint(currentAttackStep.sfxOnHit, transform.position);

            // Tắt hitbox sau khi đánh trúng? Hoặc để animation event xử lý?
             DeactivateHitbox(); // Tắt ngay để tránh đánh nhiều lần trong 1 frame animation?
        }
    }

     private void ApplyEffects(GameObject target)
     {
         // Lấy Inner Power từ EquipmentComponent
         var equipment = GetComponent<EquipmentComponent>();
         if (equipment?.CurrentInnerPower?.effectOnHit != EffectType.None)
         {
             float chance = equipment.CurrentInnerPower.effectChance;
             if (Random.value <= chance) // Check %
             {
                 // Áp dụng hiệu ứng (Stun, Poison...) lên target
                 // Cần một component quản lý hiệu ứng trên target (StatusEffectComponent)
                 Debug.Log($"Applying effect {equipment.CurrentInnerPower.effectOnHit} to {target.name}");
                 // target.GetComponent<StatusEffectComponent>()?.ApplyEffect(equipment.CurrentInnerPower);
             }
         }
          // TODO: Áp dụng hiệu ứng từ SkillData nếu có
     }

     private void ApplyHitstop(float duration)
     {
         if (duration > 0)
         {
             hitstopTimer = duration;
         }
     }


    // Gọi khi animation tấn công kết thúc (Animation Event) hoặc ComboComponent reset
    public void FinishAttack()
    {
        IsAttacking = false;
        currentAttackStep = null;
        DeactivateHitbox();
        // Có thể gọi ResetCombo ở đây nếu animation xong mà không có input tiếp
        // comboComponent?.ResetCombo();
    }
}
using UnityEngine;
using System.Collections.Generic; // For Dictionary
using BloodLotus.Core; // For IDamageable, DamageType
public class StatsComponent : MonoBehaviour, IDamageable
{
    [Header("Base Stats (From Data)")]
    // Tham chiếu đến SO nếu cần load base stats từ data (ví dụ: EnemyData)
    // Hoặc định nghĩa trực tiếp ở đây cho player/custom entities
    public float baseMaxHealth = 100f;
    public float baseDamage = 10f;
    public float baseMagicDamage = 0f;
    public float baseAttackSpeed = 1.0f;
    public float baseMovementSpeed = 5f;
    public float baseJumpHeight = 5f; // Có thể tính dựa trên lực nhảy và trọng lực
    public float baseStamina = 100f;
    public float baseCriticalRate = 0.05f; // 5%
    public float baseEffectResistance = 0f; // Kháng hiệu ứng (vd: stun, slow...)
    // ... các base stats khác ...

    [Header("Current Stats")]
    [SerializeField] private float currentHealth;
    // Thêm các chỉ số hiện tại khác nếu chúng thay đổi (vd: stamina)

    // Chỉ số cuối cùng sau khi tính toán bonus
    public float MaxHealth { get; private set; }
    public float Damage { get; private set; }
    public float MagicDamage { get; private set; }
    public float AttackSpeed { get; private set; }
    public float MovementSpeed { get; private set; }
    public float JumpHeight { get; private set; }
    public float Stamina { get; private set; }
    public float CriticalRate { get; private set; }
    public float EffectResistance { get; private set; }

    // Sự kiện để UI hoặc hệ thống khác lắng nghe
    public System.Action<float, float> OnHealthChanged; // current, max
    public System.Action OnDied;

    private void Awake()
    {
        // Khởi tạo ban đầu
        CalculateFinalStats();
        currentHealth = MaxHealth;
    }

    public void CalculateFinalStats()
    {
        // Logic tính toán chỉ số cuối cùng dựa trên base stats,
        // trang bị (EquipmentComponent), nội công (InnerPower),
        // bảo vật (Artifacts), buff/debuff...
        // Ví dụ đơn giản:
        MaxHealth = baseMaxHealth; // + bonus từ equipment, inner power...
        Damage = baseDamage; // + bonus...
        // ... Tính toán các chỉ số khác ...

        // Clamp giá trị nếu cần
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
    }

    public void TakeDamage(float amount, DamageType type, GameObject attacker)
    {
        if (currentHealth <= 0) return; // Đã chết

        // Tính toán sát thương thực tế (vd: trừ giáp, kháng...)
        float actualDamage = amount; // Placeholder

        currentHealth -= actualDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);

        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
        // Debug.Log($"{gameObject.name} took {actualDamage} {type} damage from {attacker?.name}. Current HP: {currentHealth}/{MaxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Trigger hiệu ứng bị đánh (animation, sound)
            // Có thể gọi một component khác như EffectHandlerComponent
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        OnDied?.Invoke();
        // Trigger animation chết, tắt collider, thông báo cho hệ thống (vd: DeathSystem)...
        // gameObject.SetActive(false); // Cách đơn giản
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0) return;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }
}
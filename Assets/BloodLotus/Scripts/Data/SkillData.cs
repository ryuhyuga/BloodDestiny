using UnityEngine;
using System.Collections.Generic;
using BloodLotus.Core; // Thư viện chứa các enum, struct, class dùng chung
using BloodLotus.Data; // Thư viện chứa các enum, struct, class dùng chung

[CreateAssetMenu(fileName = "NewSkillData", menuName = "Kiem Hiep/Data/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName = "New Skill";
    public WeaponType compatibleWeaponType; // Skill chỉ dùng với loại vũ khí này
    public int requiredLevel = 1; // Level yêu cầu để mở khóa extension
    public List<ComboStepData> comboExtensionSteps; // Các bước combo mở rộng

    [Tooltip("Cấp độ yêu cầu của skill này để các bước combo mở rộng được kích hoạt.")]
    public int levelToUnlockExtension = 1; // <<< Đặt giá trị mặc định (ví dụ: level 1)

    public float magicDamage = 5f;
    public DamageType damageType = DamageType.Magical;
    public EffectType effectOnHit = EffectType.None; // Hiệu ứng thêm (Stun, Poison)
    public float effectChance = 0f;
    public float effectDuration = 0f;
     [Tooltip("Sức mạnh/Giá trị của hiệu ứng (ví dụ: damage mỗi giây của Poison, lượng slow, lượng heal mỗi giây).")]
    public float effectPotency = 0f; // <<< Giá trị sức mạnh hiệu ứng
    
    [Header("Passive Bonuses (Optional)")]
    [Tooltip("Các chỉ số cộng thêm vĩnh viễn khi skill này được trang bị.")]
    public List<StatModifier> passiveStatBonuses; // <<< Khai báo List StatModifier
    public AnimatorOverrideController animationOverrideController; // Override animation khi dùng skill
    public Sprite icon;
    [TextArea] public string description;
    
    [Header("Resource Costs")]
    public float manaCost = 30f;
    public float staminaCost = 10f;

    [Header("Cooldown")]
    public float cooldown = 2f;
    public bool useChargeSystem = false;
    public int maxCharges = 3;
    public float chargeRegenRate = 1f; // Tốc độ hồi phục 1 charge

    [Header("Animation")]
    public string animationTriggerName; // Trigger name to activate skill animation

    [Header("VFX & SFX")]
    public GameObject vfxPrefab; // Visual effect prefab
    public AudioClip sfxClip; // Sound effect clip

    [Header("Skill Behavior")]
    public SkillTargetType targetType = SkillTargetType.Enemy;
    public float skillRange = 5f;
    public float aoeRadius = 0f; // Area of effect radius (0 for single target)
    public bool applyForce = false;
    public Vector2 forceDirection = Vector2.zero;
    public float forceMagnitude = 10f;

    // Thêm: cooldown, mana cost...
}

public enum SkillTargetType
{
    Self,
    Enemy,
    Location
}
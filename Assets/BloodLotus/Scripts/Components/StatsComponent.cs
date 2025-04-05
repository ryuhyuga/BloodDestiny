using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BloodLotus.Core;
using BloodLotus.Data;

public class StatsComponent : MonoBehaviour, IDamageable
{
    [Header("Base Stats")]
    [Tooltip("Chỉ số gốc của đối tượng này. Sẽ được ghi đè bởi EnemyData nếu tìm thấy EnemyDataHolder, nếu không sẽ dùng giá trị nhập ở đây.")]
    [SerializeField] private Stats baseStats = new Stats(); // Chỉ số gốc

    [Header("Current State")]
    [SerializeField] private float currentHealth;

    [Header("Component References (Optional)")]
    [SerializeField] private EquipmentComponent equipment;
     [Header("Player Level & Experience (Nếu là Player)")] // <<< THÊM MỤC NÀY
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentExperience = 0f;
    [SerializeField] private float experienceToNextLevel = 100f; // EXP cần cho level tiếp theo

    private List<StatModifier> temporaryModifiers = new List<StatModifier>();
    private Dictionary<StatType, float> finalStats = new Dictionary<StatType, float>();
    public event System.Action<int> OnPlayerLevelUp;
    public System.Action<float, float> OnHealthChanged;
    public System.Action OnDied;
    public event System.Action OnStatsChanged;
    public int CurrentLevel => currentLevel;
    public float CurrentExperience => currentExperience;
    public float ExperienceToNextLevel => experienceToNextLevel;
    // Properties truy cập chỉ số cuối (Giữ nguyên các properties khác)
    public float MaxHealth => GetFinalStatValue(StatType.Health);
    public float Damage => GetFinalStatValue(StatType.Damage);
    public float MagicDamage => GetFinalStatValue(StatType.MagicDamage);
    public float AttackSpeed => GetFinalStatValue(StatType.AttackSpeed);
    public float MovementSpeed => GetFinalStatValue(StatType.MovementSpeed);
    public float JumpHeight => GetFinalStatValue(StatType.JumpHeight);
    public float Stamina => GetFinalStatValue(StatType.Stamina);
    public float CriticalRate => GetFinalStatValue(StatType.CriticalRate);
    public float CriticalDamage => GetFinalStatValue(StatType.CriticalDamage);
    public float Armor => GetFinalStatValue(StatType.Armor);
    public float MagicResist => GetFinalStatValue(StatType.MagicResist);
    public float HealthRegen => GetFinalStatValue(StatType.HealthRegen);
    public float StaminaRegen => GetFinalStatValue(StatType.StaminaRegen);
    public float ManaRegen => GetFinalStatValue(StatType.ManaRegen);
     // Thêm các chỉ số khác nếu có trong StatType và Stats class


    void Awake()
    {
        // Luôn thử lấy EquipmentComponent, không cần gán trong Inspector trừ khi nó ở chỗ khác
        if (equipment == null) equipment = GetComponent<EquipmentComponent>();

        if (equipment != null)
        {
            CalculateExperienceToNextLevel(); // Tính EXP cần cho level hiện tại
            // TODO: Load Level và EXP từ Save Game ở đây
            equipment.OnEquipmentChanged += HandleEquipmentChanged;
        }

        // Load Base Stats
        var dataHolder = GetComponent<EnemyDataHolder>();
        if (dataHolder != null && dataHolder.enemyData != null && dataHolder.enemyData.baseStats != null)
        {
            this.baseStats = CloneStats(dataHolder.enemyData.baseStats);
            // Debug.Log($"Loaded base stats from EnemyData: {dataHolder.enemyData.enemyName}", this);
        }
        else
        {
            // Sử dụng giá trị đã được serialize từ Inspector (nếu là Player hoặc Enemy thiếu Data)
             // Debug.Log("Using base stats set in Inspector or default.", this);
             // Đảm bảo baseStats không null nếu chưa được khởi tạo ở khai báo
             if(this.baseStats == null) this.baseStats = new Stats();
        }

        CalculateFinalStats();
        currentHealth = MaxHealth; // Đặt máu ban đầu sau khi tính MaxHealth
    }

     private Stats CloneStats(Stats original) // Giữ nguyên
    {
        // ... code copy stats ...
         if (original == null) return new Stats();
         return new Stats {
             Health = original.Health, Stamina = original.Stamina, Mana = original.Mana,
             Damage = original.Damage, MagicDamage = original.MagicDamage, AttackSpeed = original.AttackSpeed,
             CriticalRate = original.CriticalRate, CriticalDamage = original.CriticalDamage, Armor = original.Armor,
             MagicResist = original.MagicResist, MovementSpeed = original.MovementSpeed, JumpHeight = original.JumpHeight,
             HealthRegen = original.HealthRegen, StaminaRegen = original.StaminaRegen, ManaRegen = original.ManaRegen
         };
    }
    public void AddExperience(float amount)
    {
        // Chỉ cộng EXP nếu đây là Player (có EquipmentComponent?) và amount > 0
        if (equipment == null || amount <= 0) return;

        currentExperience += amount;
        Debug.Log($"Player nhận được {amount} EXP. Tổng EXP: {currentExperience}/{experienceToNextLevel}");

        // Kiểm tra lên cấp
        while (currentExperience >= experienceToNextLevel)
        {
            // Trừ EXP đã dùng để lên cấp
            currentExperience -= experienceToNextLevel;
            // Tăng Level
            currentLevel++;
            Debug.LogError($"*** PLAYER LEVEL UP TO LEVEL {currentLevel}! ***"); // Dùng LogError cho nổi bật

            // Tính lại EXP cần cho level mới
            CalculateExperienceToNextLevel();

            // Xử lý các hiệu ứng khi lên cấp
            HandleLevelUpEffects();

            // Phát sự kiện lên cấp
            OnPlayerLevelUp?.Invoke(currentLevel);
        }
         // TODO: Cập nhật UI EXP/Level bar
    }
    private void CalculateExperienceToNextLevel()
    {
        // Công thức ví dụ: Tuyến tính đơn giản
        experienceToNextLevel = 100 + (currentLevel * 50);
        // Hoặc công thức mũ:
        // experienceToNextLevel = 100 * Mathf.Pow(1.2f, currentLevel);
         Debug.Log($"EXP cần cho Level {currentLevel + 1}: {experienceToNextLevel}");
    }

    /// <summary>
    /// Xử lý các thay đổi khi Player lên cấp.
    /// </summary>
    private void HandleLevelUpEffects()
    {
        // 1. Hồi đầy máu/mana? (Tùy chọn)
        Heal(MaxHealth); // Hồi đầy máu
        // currentMana = MaxMana; // Nếu có Mana

        // 2. Tăng chỉ số cơ bản? (Ví dụ)
        // Cần truy cập và sửa đổi trực tiếp baseStats
        baseStats.Health += 10; // Ví dụ tăng 10 máu gốc
        baseStats.Damage += 2;  // Ví dụ tăng 2 damage gốc
        // ... tăng các chỉ số khác ...

        // 3. Tính toán lại chỉ số cuối cùng NGAY LẬP TỨC sau khi tăng chỉ số gốc
        CalculateFinalStats();

        // 4. TODO: Cộng điểm tiềm năng? Mở khóa skill mới? (Cần các hệ thống khác)
        // PotentialPointsSystem.Instance?.AddPoints(5);
        // SkillTreeSystem.Instance?.UnlockSkillsForLevel(currentLevel);

         // 5. TODO: Hiệu ứng hình ảnh/âm thanh khi lên cấp
         // Instantiate(levelUpVFXPrefab, transform.position, Quaternion.identity);
         // AudioManager.Instance?.PlaySound("LevelUp");
    }


    // ... (TakeDamage, Die, Heal, AddModifier, RemoveModifier giữ nguyên) ...
    // Lưu ý: Hàm Heal đã có sẵn, có thể dùng trong HandleLevelUpEffects

    void OnDestroy() // Giữ nguyên
    {
        if (equipment != null)
        {
            equipment.OnEquipmentChanged -= HandleEquipmentChanged;
        }
    }

    private void HandleEquipmentChanged() { // Giữ nguyên
        CalculateFinalStats();
    }

    public void CalculateFinalStats() // Giữ nguyên logic tính toán
    {
        Stats baseValues = this.baseStats;
        if (baseValues == null) { /* ... Log Error ... */ baseValues = new Stats(); }

        finalStats.Clear();

        foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
        {
            float baseValue = GetBaseValue(baseValues, statType);
            List<StatModifier> relevantModifiers = GetAllModifiersForStat(statType);
            float finalValue = baseValue;
            float percentAddSum = 0;
            relevantModifiers.Sort((a, b) => ((int)a.Type).CompareTo((int)b.Type));
            foreach (var modifier in relevantModifiers) { /* ... logic tính flat, percentadd, percentmult ... */ }
            finalValue *= (1 + percentAddSum);
            finalStats[statType] = finalValue;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth); // Clamp máu hiện tại
        OnStatsChanged?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    // <<< HOÀN THIỆN HÀM NÀY >>>
    private float GetBaseValue(Stats baseValues, StatType type)
    {
        if (baseValues == null) return 0f;
        switch (type)
        {
            case StatType.Health: return baseValues.Health;
            case StatType.Stamina: return baseValues.Stamina;
            case StatType.Mana: return baseValues.Mana;
            case StatType.Damage: return baseValues.Damage;
            case StatType.MagicDamage: return baseValues.MagicDamage;
            case StatType.AttackSpeed: return baseValues.AttackSpeed;
            case StatType.CriticalRate: return baseValues.CriticalRate;
            case StatType.CriticalDamage: return baseValues.CriticalDamage;
            case StatType.Armor: return baseValues.Armor;
            case StatType.MagicResist: return baseValues.MagicResist;
            case StatType.MovementSpeed: return baseValues.MovementSpeed;
            case StatType.JumpHeight: return baseValues.JumpHeight;
            case StatType.HealthRegen: return baseValues.HealthRegen;
            case StatType.StaminaRegen: return baseValues.StaminaRegen;
            case StatType.ManaRegen: return baseValues.ManaRegen;
            // Thêm các case khác cho các StatType bạn đã định nghĩa trong enum và class Stats
            // case StatType.Accuracy: return baseValues.Accuracy; // Ví dụ
            // case StatType.Evasion: return baseValues.Evasion; // Ví dụ
            default:
                // Debug.LogWarning($"Base value for StatType '{type}' not defined in GetBaseValue.");
                return 0f;
        }
    }

    // <<< HOÀN THIỆN HÀM NÀY >>>
    private List<StatModifier> GetAllModifiersForStat(StatType statType)
    {
        List<StatModifier> modifiers = new List<StatModifier>();

        // Chỉ lấy modifier từ equipment nếu equipment component tồn tại
        if (equipment != null)
        {
            // 1. Từ Vũ Khí Hiện Tại
            if (equipment.CurrentWeapon?.directStatBonuses != null) // Dùng ?. để an toàn
            {
                modifiers.AddRange(equipment.CurrentWeapon.directStatBonuses.Where(m => m.Stat == statType));
            }

            // 2. Từ Các Skill Đang Trang Bị
            if (equipment.EquippedSkills != null) // Không cần ?. vì list được khởi tạo
            {
                foreach (var skill in equipment.EquippedSkills)
                {
                    if (skill?.passiveStatBonuses != null) // Dùng ?. cho skill
                    {
                        modifiers.AddRange(skill.passiveStatBonuses.Where(m => m.Stat == statType));
                    }
                }
            }

            // 3. Từ Nội Công Hiện Tại
            if (equipment.CurrentInnerPower?.passiveStatBonuses != null) // Dùng ?.
            {
                modifiers.AddRange(equipment.CurrentInnerPower.passiveStatBonuses.Where(m => m.Stat == statType));
            }

            // 4. TODO: Từ Bảo Vật (Artifacts)
            // if (equipment.EquippedArtifacts != null) { ... }
        }

        // 5. Từ Buff/Debuff Tạm Thời (Luôn áp dụng)
        modifiers.AddRange(temporaryModifiers.Where(m => m.Stat == statType));

        return modifiers;
    }

    // GetFinalStatValue giữ nguyên logic, nhưng cần đảm bảo GetBaseValue đã hoàn thiện
    public float GetFinalStatValue(StatType type)
    {
        return finalStats.TryGetValue(type, out float value) ? value : GetBaseValue(this.baseStats, type);
    }

    // TakeDamage, Die, Heal giữ nguyên
    public void TakeDamage(float amount, DamageType type, GameObject attacker)
    {
        Debug.LogError($"--- {this.gameObject.name} StatsComponent.TakeDamage CALLED! --- Amount: {amount}, Attacker: {attacker?.name}");
        GetComponent<EnemyDeathHandler>()?.SetLastAttacker(attacker);

        if (currentHealth <= 0) return;

        // <<< KHAI BÁO damageTaken Ở NGOÀI NÀY >>>
        float damageTaken = amount; // Khởi tạo bằng giá trị gốc

        // --- Tính Toán Giảm Trừ Sát Thương ---
        if (type == DamageType.Physical)
        {
            float armor = Armor; // Lấy giá trị armor đã tính
            // Công thức giảm trừ từ Armor (ví dụ)
            float reduction = armor / (armor + 100); // Ví dụ: 100 Armor giảm 50%
            damageTaken *= (1f - reduction); // <<< Giờ chỉ gán giá trị cho biến đã khai báo
            // Debug.Log($"[{this.gameObject.name}] Physical damage reduced by {reduction * 100}% due to armor.");
        }
        else if (type == DamageType.Magical)
        {
             float magicResist = MagicResist; // Lấy giá trị resist đã tính
             // Công thức giảm trừ từ Magic Resist (ví dụ)
            float reduction = magicResist / (magicResist + 100);
            damageTaken *= (1f - reduction); // <<< Giờ chỉ gán giá trị cho biến đã khai báo
             // Debug.Log($"[{this.gameObject.name}] Magical damage reduced by {reduction * 100}% due to magic resist.");
        }
        // True damage: damageTaken sẽ giữ nguyên giá trị gốc 'amount'

        // Đảm bảo sát thương không âm SAU KHI đã tính giảm trừ
        damageTaken = Mathf.Max(0, damageTaken);
        Debug.Log($"[{this.gameObject.name}] Final Damage Taken: {damageTaken}");


        // --- Trừ máu và cập nhật ---
        // <<< Giờ đây 'damageTaken' đã được biết đến >>>
        currentHealth -= damageTaken;
        Debug.Log($"[{this.gameObject.name}] Health AFTER taking damage: {currentHealth}");


        currentHealth = Mathf.Clamp(currentHealth, 0, MaxHealth);
        Debug.Log($"[{this.gameObject.name}] Health AFTER Clamp: {currentHealth} / {MaxHealth}");

        OnHealthChanged?.Invoke(currentHealth, MaxHealth);

        if (currentHealth <= 0)
        {
             Debug.LogError($"[{this.gameObject.name}] HEALTH <= 0! Calling Die().");
            Die();
        }
    }
    
    
    private void Die() 
    { 
        Debug.LogError("--- ENEMY DIED EVENT INVOKED ---");
        OnDied?.Invoke(); 
        
    }    
    public void Heal(float amount) { /* ... */ OnHealthChanged?.Invoke(currentHealth, MaxHealth); }


    // AddModifier, RemoveModifier giữ nguyên
    public void AddModifier(StatModifier modifier) { /* ... */ CalculateFinalStats(); }
    public void RemoveModifier(StatModifier modifier) { /* ... */ CalculateFinalStats(); }
}
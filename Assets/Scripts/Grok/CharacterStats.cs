using UnityEngine;

[System.Serializable]
public struct FinalStats
{
    public float damage;       // Light Attack
    public float heavyDamage;  // Heavy Attack
    public float attackSpeed;
    public float critRate;
    public float critDamage;
    public float maxHealth;
    public float armor;
    // ... có thể thêm moveSpeed ...
}

public class CharacterStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseDamage = 10f;
    public float baseHeavyDamage = 20f;
    public float baseAttackSpeed = 1f;
    public float baseCritRate = 0.1f;
    public float baseCritDamage = 1.5f;
    public float baseMaxHealth = 100f;
    public float baseArmor = 5f;

    // Lưu final stats sau khi cộng dồn
    public FinalStats finalStats;

    void Start()
    {
        ComputeFinalStats();
    }

    // Hàm tính finalStats, EquipmentSystem sẽ gọi khi equip/unequip
    public void ComputeFinalStats(
        float weaponDamage = 0f,
        float weaponHeavyDamage = 0f,
        float weaponAttackSpeed = 0f,
        float weaponCritRate = 0f,
        float weaponCritDamage = 0f,
        float weaponArmor = 0f,
        float weaponMaxHealth = 0f,

        float innerDamage = 0f,
        float innerHeavyDamage = 0f,
        float innerAttackSpeed = 0f,
        float innerCritRate = 0f,
        float innerCritDamage = 0f,
        float innerArmor = 0f,
        float innerMaxHealth = 0f
    )
    {
        finalStats.damage = baseDamage + weaponDamage + innerDamage;
        finalStats.heavyDamage = baseHeavyDamage + weaponHeavyDamage + innerHeavyDamage;
        finalStats.attackSpeed = baseAttackSpeed + weaponAttackSpeed + innerAttackSpeed;
        finalStats.critRate = baseCritRate + weaponCritRate + innerCritRate;
        finalStats.critDamage = baseCritDamage + weaponCritDamage + innerCritDamage;
        finalStats.maxHealth = baseMaxHealth + weaponMaxHealth + innerMaxHealth;
        finalStats.armor = baseArmor + weaponArmor + innerArmor;

        // Giới hạn tỉ lệ critRate <= 100%
        if (finalStats.critRate > 1f) finalStats.critRate = 1f;
    }
}

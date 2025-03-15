using UnityEngine;

[System.Serializable]
public class FinalStats
{
    public float damage;
    public float heavyDamage;
    public float attackSpeed;
    public float critRate;
    public float critDamage;
    public float maxHealth;
    public float armor;
}

public class PlayerStats : MonoBehaviour
{
    [Header("Basic Stats")]
    public float baseDamage = 10f;
    public float baseHeavyDamage = 20f;
    public float baseAttackSpeed = 1f;
    public float baseCritRate = 0.1f;
    public float baseCritDamage = 1.5f;
    public float baseMaxHealth = 100f;
    public float baseArmor = 5f;

    [Header("Character Level")]
    public int level = 1;
    public float currentExp = 0f;
    public float expToNextLevel = 100f;
    public float expGrowthRate = 1.2f;

    [Header("Calculated Final Stats")]
    public FinalStats finalStats;
    public float currentHealth;

    void Start()
    {
        currentHealth = baseMaxHealth;
        ComputeFinalStats();
    }

    public void GainExp(float amount)
    {
        currentExp += amount;
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        currentExp = 0f;
        expToNextLevel *= expGrowthRate;

        // Tăng chỉ số mỗi khi lên cấp
        baseMaxHealth += 10;
        baseDamage += 2;
        baseArmor += 1;

        Debug.Log($"[PlayerStats] Level Up! New Level: {level}");
    }

    public void ComputeFinalStats()
    {
        finalStats.damage = baseDamage;
        finalStats.heavyDamage = baseHeavyDamage;
        finalStats.attackSpeed = baseAttackSpeed;
        finalStats.critRate = baseCritRate;
        finalStats.critDamage = baseCritDamage;
        finalStats.maxHealth = baseMaxHealth;
        finalStats.armor = baseArmor;
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(0, damage - finalStats.armor);
        currentHealth -= finalDamage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("[PlayerStats] Player has died!");
        // Thêm logic hồi sinh hoặc game over
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, finalStats.maxHealth);
    }
}

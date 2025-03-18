using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{
    [Header("Core Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float damage = 10f;
    public float magicDamage = 5f;

    [Header("Movement Stats")]
    public float movementSpeed = 1f;
    public float jumpHeight = 1f;
    public float attackSpeed = 1f;
    public float stamina = 100f;
    public float maxStamina = 100f;

    [Header("Combat Stats")]
    public float criticalRate = 0.1f;
    public float criticalDamage = 2f;
    public float effectResistance = 0.1f;
    public float armor = 10f;
    public float magicResistance = 5f;

    [Header("Level System")]
    public int currentLevel = 1;
    public float currentEXP = 0f;
    public float requiredEXP = 100f;

    // Status Effects
    public event Action<StatusEffect> OnStatusEffectApplied;
    public event Action<StatusEffect> OnStatusEffectRemoved;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        // Damage calculation with armor reduction
        float finalDamage = Mathf.Max(0, amount - armor);
        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    public void GainEXP(float exp)
    {
        currentEXP += exp;

        if (currentEXP >= requiredEXP)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentEXP -= requiredEXP;
        requiredEXP *= 1.5f;

        // Increase stats on level up
        maxHealth += 10;
        damage += 2;
        armor += 1;

        currentHealth = maxHealth;
    }

    void Die()
    {
        // Implement death logic
        Debug.Log("Character Died");
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        // Check effect resistance
        if (UnityEngine.Random.value > effectResistance)
        {
            OnStatusEffectApplied?.Invoke(effect);
        }
    }
}

// Status Effect Class
[Serializable]
public class StatusEffect
{
    public enum EffectType
    {
        Stun,
        Poison,
        Burn,
        Slow,
        Weaken
    }

    public EffectType type;
    public float duration;
    public float intensity;
}
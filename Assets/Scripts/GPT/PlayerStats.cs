using UnityEngine;
using System.Collections;


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

    [Header("Animation & Effects")]
    public Animator animator;       // Gán Animator trong Inspector
    public bool isStunned = false;  // Trạng thái choáng

    void Start()
    {
        currentHealth = baseMaxHealth;
        ComputeFinalStats();
    }

    //-------------- LEVEL & EXP --------------
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
        ComputeFinalStats();
    }

    //-------------- STATS CALC --------------
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

    //-------------- DAMAGE & DEATH --------------
    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(0, damage - finalStats.armor);
        currentHealth -= finalDamage;

        // Gọi animator Hurt (nếu animator != null)
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        // Hiển thị popup damage (nếu có DamagePopupManager)
        if (DamagePopupManager.Instance != null)
        {
            DamagePopupManager.Instance.SpawnDamageText(finalDamage, transform.position);
        }

        Debug.Log($"[PlayerStats] Took {finalDamage} damage, remain {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("[PlayerStats] Player has died!");
        // Gọi animator Die (nếu animator != null)
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        // Thêm logic hồi sinh hoặc game over, Destroy...
    }

    //-------------- HEAL --------------
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, finalStats.maxHealth);
        Debug.Log($"[PlayerStats] Healed {amount}, currentHealth={currentHealth}");
    }

    //-------------- STUN --------------
    public void Stun(float duration)
    {
        if (!isStunned)
        {
            isStunned = true;
            Debug.Log("[PlayerStats] Player is stunned!");
            if (animator != null)
            {
                animator.SetBool("isStunned", true);
            }
            // Tắt input, movement... tuỳ logic
            Invoke(nameof(EndStun), duration);
        }
    }

    private void EndStun()
    {
        isStunned = false;
        Debug.Log("[PlayerStats] Player is no longer stunned!");
        if (animator != null)
        {
            animator.SetBool("isStunned", false);
        }
        // Bật input, movement...
    }
}

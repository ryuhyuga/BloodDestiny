
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 50f;
    public float armor = 5f;

    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(0, damage - armor);
        health -= finalDamage;
        Debug.Log($"[EnemyHealth] Took {finalDamage} damage, remain {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("[EnemyHealth] Enemy died.");
        Destroy(gameObject);
    }
}

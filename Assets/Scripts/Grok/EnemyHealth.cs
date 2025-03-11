using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 50f; // Máu kẻ thù
    public float armor = 5f;   // Giáp kẻ thù (nếu cần)

    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(0, damage - armor);
        health -= finalDamage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Trigger animation chết, v.v.
        Destroy(gameObject);
    }
}

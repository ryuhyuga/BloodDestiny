using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 50f;
    public float armor = 5f;
    public bool isStunned = false;
    public Animator animator;

    // Tùy logic: set animator param "isStunned", "Hurt" trigger

    public void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(0, damage - armor);
        health -= finalDamage;

        // Gọi state hurt
        animator.SetTrigger("Hurt"); // => chuyển sang HurtState
        Debug.Log($"[EnemyHealth] Took {finalDamage} damage, remain {health}");

        Vector3 enemyPos = transform.position;
        DamagePopupManager.Instance.SpawnDamageText(finalDamage, enemyPos);

        if (health <= 0)
        {
            Die();
        }
    }

    public void Stun(float duration)
    {
        isStunned = true;
        animator.SetBool("isStunned", true);
        // tắt AI movement
        Invoke(nameof(EndStun), duration);
    }

    private void EndStun()
    {
        isStunned = false;
        animator.SetBool("isStunned", false);
    }

    private void Die()
    {
        Debug.Log("[EnemyHealth] Enemy died.");
        // Gọi animator "Die" state, 
        // hoặc Destroy(gameObject)
        animator.SetTrigger("Die");
        // Tùy logic: Destroy(gameObject, 1f);
        Destroy(gameObject, 2f);
    }
}

using UnityEngine;
using BloodLotus.Core;

public class Projectile : MonoBehaviour
{
    public float damage = 20f; // Sát thương của đạn
    public float lifespan = 2f; // Thời gian tồn tại (tự hủy)
    public GameObject owner; // Người bắn ra đạn (để tránh tự bắn vào mình)
    private float timeAlive = 0f;

    void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifespan)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu va chạm với chính mình hoặc không có IDamageable
        if (other.gameObject == owner) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, DamageType.Physical, owner);
            Destroy(gameObject); // Hủy đạn sau khi trúng mục tiêu
        }
    }
}
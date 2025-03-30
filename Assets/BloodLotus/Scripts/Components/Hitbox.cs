using UnityEngine;

// Gắn script này vào GameObject chứa Collider Hitbox (là con của Player)
public class Hitbox : MonoBehaviour
{
    // Tham chiếu đến CombatComponent của đối tượng cha (Player)
    // Cách 1: Gán thủ công trong Inspector
    [SerializeField] private CombatComponent combatComponent;

    // Cách 2: Tự động lấy trong Awake (nếu Hitbox luôn là con trực tiếp)
    // private CombatComponent combatComponent;
    // void Awake() {
    //     combatComponent = GetComponentInParent<CombatComponent>();
    //     if (combatComponent == null) {
    //         Debug.LogError("Không tìm thấy CombatComponent ở đối tượng cha!", this);
    //     }
    // }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem combatComponent có tồn tại và có đang trong trạng thái tấn công không
        if (combatComponent != null /*&& combatComponent.IsAttacking*/) // Kiểm tra IsAttacking nếu cần
        {
            // Gọi hàm xử lý va chạm trên CombatComponent
            combatComponent.HandleHit(other);

            // Tùy chọn: Tắt hitbox ngay sau khi đánh trúng một đối tượng để tránh đánh nhiều lần trong một cú vung?
            // gameObject.SetActive(false);
        }
    }
}
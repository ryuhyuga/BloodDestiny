using UnityEngine;

// Gắn script này vào GameObject chứa Collider 2D được đặt là "Is Trigger"
// GameObject này thường là con của đối tượng tấn công (Player, Enemy)
[RequireComponent(typeof(Collider2D))] // Đảm bảo luôn có Collider2D
public class Hitbox : MonoBehaviour
{
    [Header("Required Reference")]
    [Tooltip("Tham chiếu đến CombatComponent của đối tượng sở hữu Hitbox này (Player hoặc Enemy). Phải được gán trong Inspector hoặc tự lấy.")]
    [SerializeField] private CombatComponent ownerCombatComponent;

    // Biến cờ để chỉ xử lý va chạm khi hitbox thực sự được kích hoạt bởi CombatComponent
    // (Mặc dù CombatComponent đã bật/tắt GameObject, kiểm tra này thêm một lớp an toàn)
    // private bool isCurrentlyActive = false; // Có thể không cần thiết nếu dựa vào SetActive
    void OnTriggerEnter2D(Collider2D other)
{
    Debug.LogError($"[Hitbox] OnTriggerEnter2D with: {other.gameObject.name}"); // Dùng LogError để nổi bật
    if (ownerCombatComponent != null)
    {
        Debug.Log("[Hitbox] Calling HandleMeleeHit...");
        ownerCombatComponent.HandleMeleeHit(other);
    } else { Debug.LogError("[Hitbox] ownerCombatComponent is NULL!"); }
}
    void Awake()
    {
        // --- Tùy chọn: Tự động tìm CombatComponent ở đối tượng cha ---
        // Bỏ comment đoạn này nếu bạn muốn nó tự tìm thay vì gán tay
        // if (ownerCombatComponent == null)
        // {
        //     ownerCombatComponent = GetComponentInParent<CombatComponent>();
        // }
        // -------------------------------------------------------------

        // Đảm bảo CombatComponent đã được gán
        if (ownerCombatComponent == null)
        {
            Debug.LogError($"CombatComponent chưa được gán hoặc không tìm thấy ở cha cho Hitbox trên GameObject '{this.gameObject.name}'!", this);
            // Tắt script này đi nếu thiếu tham chiếu quan trọng
            this.enabled = false;
        }

        // Đảm bảo Collider là Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"Collider2D trên Hitbox '{this.gameObject.name}' chưa được đặt là 'Is Trigger'. Sẽ tự động đặt.", this);
            col.isTrigger = true;
        }
    }

    /// <summary>
    /// Được gọi bởi hệ thống vật lý của Unity khi một Collider 2D khác
    /// đi vào vùng Trigger của Collider 2D trên GameObject này.
    /// </summary>
    /// <param name="other">Collider 2D của đối tượng đã đi vào.</param>
   

    /* // Các hàm OnEnable/OnDisable có thể dùng để quản lý cờ isCurrentlyActive nếu cần
    void OnEnable() {
        // Được gọi khi GameObject chứa Hitbox được SetActive(true)
        // isCurrentlyActive = true;
         // Debug.Log("Hitbox Enabled");
    }

    void OnDisable() {
        // Được gọi khi GameObject chứa Hitbox được SetActive(false)
        // isCurrentlyActive = false;
         // Debug.Log("Hitbox Disabled");
    }
    */
}
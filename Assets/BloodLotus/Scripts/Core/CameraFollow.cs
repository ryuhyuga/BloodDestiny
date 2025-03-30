using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Đối tượng mà camera sẽ đi theo (Thường là Player).")]
    public Transform target; // Kéo Player GameObject vào đây trong Inspector

    [Header("Following Settings")]
    [Tooltip("Tốc độ làm mượt chuyển động của camera. Giá trị nhỏ hơn sẽ theo sát hơn, lớn hơn sẽ mượt hơn.")]
    public float smoothTime = 0.3f;

    [Tooltip("Độ lệch vị trí theo trục Y so với Target (ví dụ: để camera cao hơn đầu nhân vật một chút).")]
    public float yOffset = 1.0f;

    // Biến nội bộ để lưu trữ vận tốc hiện tại của camera (cần cho SmoothDamp)
    private Vector3 velocity = Vector3.zero;
    private Camera cam; // Tham chiếu đến component Camera

    void Awake()
    {
        cam = GetComponent<Camera>(); // Lấy component Camera trên cùng GameObject
        if (cam == null)
        {
            Debug.LogError("CameraFollow script cần được gắn vào GameObject có component Camera!", this);
        }
    }


    // LateUpdate được gọi sau khi tất cả các hàm Update và FixedUpdate đã chạy xong trong frame đó.
    // Đây là nơi lý tưởng để cập nhật vị trí camera theo sau đối tượng đã di chuyển.
    void LateUpdate()
    {
        // Kiểm tra xem có target để đi theo không
        if (target == null)
        {
            // Bạn có thể thêm logic tìm target tự động ở đây nếu muốn, ví dụ:
            // GameObject player = GameObject.FindWithTag("Player");
            // if (player != null) target = player.transform;
            // else {
                 Debug.LogWarning("Target chưa được gán cho CameraFollow và không tìm thấy Player!", this);
                 return; // Không làm gì nếu không có target
            // }
        }

        // Tính toán vị trí mục tiêu mà camera muốn đến
        // Lấy vị trí X, Y của target, cộng thêm yOffset, và giữ nguyên vị trí Z hiện tại của camera
        // (Giữ Z rất quan trọng để camera không bị di chuyển ra xa hoặc lại gần mặt phẳng 2D)
        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y + yOffset,
            transform.position.z // <<< Giữ nguyên Z của camera
        );

        // Sử dụng SmoothDamp để di chuyển camera đến vị trí mục tiêu một cách mượt mà
        // Nó sẽ tính toán vị trí mới dựa trên vị trí hiện tại, vị trí mục tiêu, vận tốc hiện tại và thời gian làm mượt.
        transform.position = Vector3.SmoothDamp(
            transform.position, // Vị trí hiện tại của camera
            targetPosition,     // Vị trí camera muốn đến
            ref velocity,       // Vận tốc hiện tại của camera (được cập nhật bởi hàm này - dùng ref)
            smoothTime          // Thời gian để camera "đuổi kịp" target
        );
    }
}
using UnityEngine;
using BloodLotus.Data; // <<< Cần thiết để sử dụng EnemyData

// Gắn component này vào GameObject của Enemy
public class EnemyDataHolder : MonoBehaviour
{
    [Tooltip("Gán EnemyData Scriptable Object tương ứng cho kẻ địch này vào đây.")]
    public EnemyData enemyData; // Biến public để chứa tham chiếu đến SO

    // (Tùy chọn) Thêm một hàm kiểm tra trong Awake hoặc Start để đảm bảo data đã được gán
    void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError($"EnemyData chưa được gán vào EnemyDataHolder trên GameObject '{this.gameObject.name}'!", this);
            // Bạn có thể thêm logic khác ở đây, ví dụ tự hủy hoặc tắt đối tượng nếu thiếu data nghiêm trọng
            // this.enabled = false;
        }
    }
}
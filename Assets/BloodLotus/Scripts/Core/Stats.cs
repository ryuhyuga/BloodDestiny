using UnityEngine;


namespace BloodLotus.Core // <<< Đặt trong namespace Core
{
    /// <summary>
    /// Lưu trữ tập hợp các chỉ số cơ bản cho một thực thể (Player, Enemy).
    /// Các giá trị này thường là giá trị "gốc" (base value) trước khi áp dụng các modifier.
    /// </summary>
    [System.Serializable] // <<< QUAN TRỌNG: Để có thể thấy và sửa trong Inspector khi nhúng vào SO hoặc Component khác.
    public class Stats // Dùng class để dễ dàng tham chiếu và có thể null nếu muốn
    {
        // Lưu ý: Các tên trường ở đây nên khớp với các giá trị trong enum StatType
        // để việc ánh xạ hoặc tính toán sau này dễ dàng hơn (nếu cần).

        [Header("Base Stats")]
        public float Health = 100f;
        public float Stamina = 100f;
        public float Mana = 50f; // Nếu dùng

        [Header("Offensive Stats")]
        public float Damage = 10f;
        public float MagicDamage = 0f;
        [Range(0.5f, 3.0f)] // Giới hạn hợp lý cho tốc độ đánh multiplier
        public float AttackSpeed = 1.0f;
        [Range(0f, 1f)]
        public float CriticalRate = 0.05f; // 5%
        public float CriticalDamage = 1.5f; // 150%

        [Header("Defensive Stats")]
        public float Armor = 5f;
        public float MagicResist = 5f;

        [Header("Utility Stats")]
        public float MovementSpeed = 5f;
        public float JumpHeight = 5f; // Có thể tính dựa trên lực nhảy và trọng lực

        [Header("Regeneration")]
        public float HealthRegen = 0f;
        public float StaminaRegen = 5f;
        public float ManaRegen = 1f; // Nếu dùng

        // Bạn có thể thêm các chỉ số khác từ StatType nếu muốn chúng có giá trị cơ bản riêng
        // public float Accuracy = 0.9f;
        // public float Evasion = 0.05f;
        // ...

        // (Optional) Constructor nếu muốn tạo Stats từ code
        // public Stats() { /* Khởi tạo giá trị mặc định nếu cần */ }
    }
}
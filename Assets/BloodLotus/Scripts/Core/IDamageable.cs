// File: Assets/BloodLotus/Scripts/Core/IDamageable.cs

using UnityEngine; // Cần thiết vì phương thức TakeDamage sử dụng GameObject

namespace BloodLotus.Core // <<< Đặt trong namespace Core
{
    /// <summary>
    /// Interface cho bất kỳ đối tượng nào có thể nhận sát thương.
    /// Defines a contract for any entity that can receive damage.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Xử lý việc nhận sát thương.
        /// Processes incoming damage.
        /// </summary>
        /// <param name="amount">Lượng sát thương gốc. Base damage amount.</param>
        /// <param name="type">Loại sát thương (Physical, Magical, etc.). Damage type.</param>
        /// <param name="attacker">GameObject đã gây ra sát thương (có thể null). The GameObject that dealt the damage (can be null).</param>
        void TakeDamage(float amount, DamageType type, GameObject attacker);

        // Bạn có thể thêm các phương thức hoặc thuộc tính khác vào interface này nếu cần
        // Ví dụ:
        // bool IsDead { get; } // Thuộc tính để kiểm tra nhanh đối tượng đã chết chưa
        // Transform transform { get; } // Yêu cầu đối tượng phải có Transform (MonoBehaviour đã có sẵn)
    }
}
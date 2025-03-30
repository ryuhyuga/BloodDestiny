// File: Assets/BloodLotus/Scripts/Core/StatModifier.cs

using UnityEngine; // Có thể cần cho Tooltip hoặc các thuộc tính khác sau này

namespace BloodLotus.Core // <<< Đặt trong cùng namespace với các Enums
{
    // Không cần using BloodLotus.Core ở đây vì struct này ĐANG được định nghĩa bên trong namespace đó.

    /// <summary>
    /// Đại diện cho một sự thay đổi lên một chỉ số cụ thể (StatType).
    /// Mô tả cách thay đổi (ModifierType) và giá trị thay đổi (Value).
    /// Được sử dụng bởi trang bị, buff, debuff, nội công, bảo vật,...
    /// </summary>
    [System.Serializable] // <<< QUAN TRỌNG: Để Unity lưu và hiển thị trong Inspector khi dùng trong List/Array.
    public struct StatModifier // Dùng struct thường hiệu quả cho kiểu dữ liệu này.
    {
        [Tooltip("Chỉ số (StatType) bị ảnh hưởng bởi modifier này.")]
        public StatType Stat; // Sử dụng enum StatType đã định nghĩa trong Enums.cs

        [Tooltip("Giá trị của sự thay đổi. Ví dụ: 10 cho +10 Flat, hoặc 0.15 cho +15% Percent.")]
        public float Value;

        [Tooltip("Cách thức thay đổi chỉ số (ModifierType): Flat, PercentAdd, PercentMult.")]
        public ModifierType Type; // Sử dụng enum ModifierType đã định nghĩa trong Enums.cs

        // --- Các phần tùy chọn (Optional) ---

        [Tooltip("(Tùy chọn) Thứ tự áp dụng modifier. Các modifier có Order nhỏ hơn sẽ được tính trước. Mặc định dựa trên Type (Flat=100, P.Add=200, P.Mult=300).")]
        public int Order; // Có thể không cần nếu bạn luôn tính theo thứ tự Type

        [Tooltip("(Tùy chọn) Nguồn gốc của modifier này (ví dụ: tên vũ khí, tên buff) để dễ debug hoặc quản lý việc cộng/xóa.")]
        public object Source; // Dùng object để linh hoạt, nhưng thường sẽ cần ép kiểu khi sử dụng. Có thể dùng string hoặc một interface/class cụ thể hơn.

        /// <summary>
        /// Constructor cơ bản để tạo StatModifier.
        /// </summary>
        /// <param name="stat">Chỉ số bị ảnh hưởng.</param>
        /// <param name="type">Cách thức thay đổi.</param>
        /// <param name="value">Giá trị thay đổi.</param>
        /// <param name="source">(Tùy chọn) Nguồn gốc của modifier.</param>
        public StatModifier(StatType stat, ModifierType type, float value, object source = null)
        {
            Stat = stat;
            Type = type;
            Value = value;
            Source = source;

            // Gán Order mặc định dựa trên Type nếu không dùng constructor đầy đủ
            Order = (int)type;
        }

        /// <summary>
        /// Constructor đầy đủ (Nếu bạn muốn tùy chỉnh Order).
        /// </summary>
        public StatModifier(StatType stat, ModifierType type, float value, int order, object source = null)
        {
            Stat = stat;
            Type = type;
            Value = value;
            Order = order;
            Source = source;
        }
    }
}
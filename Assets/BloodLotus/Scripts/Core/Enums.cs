// File: Assets/BloodLotus/Scripts/Core/Enums.cs (Đổi tên file nếu muốn)

namespace BloodLotus.Core // Sử dụng namespace Core nếu bạn đang dùng
{
    /// <summary>
    /// Định nghĩa các loại sát thương trong game.
    /// </summary>
    public enum DamageType
    {
        Physical,   // Sát thương vật lý (Bị ảnh hưởng bởi giáp)
        Magical,    // Sát thương phép (Bị ảnh hưởng bởi kháng phép)
        True        // Sát thương chuẩn (Bỏ qua giáp/kháng phép)
    }

    /// <summary>
    /// Định nghĩa các loại hiệu ứng trạng thái có trong game.
    /// </summary>
    public enum EffectType
    {
        None,        // Không có hiệu ứng đặc biệt
        Stun,        // Choáng (Không thể hành động)
        Poison,      // Trúng độc (Mất máu theo thời gian - Physical/True Dmg?)
        HealOverTime,// Hồi máu theo thời gian
        Slow,        // Làm chậm (Giảm tốc độ di chuyển/tấn công)
        Burn,        // Thiêu đốt (Mất máu theo thời gian - Magical Dmg?)
        Freeze,      // Đóng băng (Không thể hành động, có thể tăng sát thương nhận vào?)
        Bleed,       // Chảy máu (Mất máu theo thời gian - Physical Dmg?)
        Knockback,   // Đẩy lùi
        Silence,     // Câm lặng (Không thể dùng kỹ năng)
        Fear         // Sợ hãi (Chạy lung tung)
        // Thêm các loại hiệu ứng khác bạn cần ở đây...
    }

    /// <summary>
    /// Định nghĩa các loại vũ khí chính.
    /// </summary>
    public enum WeaponType
    {
        None,     // Không vũ khí
        Sword,    // Kiếm
        Spear,    // Thương
        Axe,      // Rìu
        Fist,     // Quyền (tay không hoặc vũ khí dạng găng)
        Bow,      // Cung (Ví dụ nếu mở rộng)
        Dagger    // Dao găm (Ví dụ nếu mở rộng)
    }

    /// <summary>
    /// Định nghĩa các chỉ số cơ bản và thứ cấp của thực thể.
    /// </summary>
    public enum StatType
    {
        // Chỉ số cơ bản (Base Stats)
        Health,         // Máu tối đa
        Stamina,        // Thể lực tối đa
        Mana,           // Nội lực/Năng lượng tối đa (Nếu cần cho skill)

        // Chỉ số tấn công (Offensive Stats)
        Damage,         // Sát thương vật lý cơ bản
        MagicDamage,    // Sát thương phép cơ bản
        AttackSpeed,    // Tốc độ đánh (vd: animation speed multiplier)
        CriticalRate,   // Tỷ lệ chí mạng (0.0 tới 1.0)
        CriticalDamage, // Sát thương chí mạng (vd: multiplier 1.5 = 150%)
        Accuracy,       // Độ chính xác (Nếu có hệ thống né tránh)

        // Chỉ số phòng thủ (Defensive Stats)
        Armor,          // Giáp (Giảm sát thương vật lý nhận vào)
        MagicResist,    // Kháng phép (Giảm sát thương phép nhận vào)
        Evasion,        // Né tránh (Tỷ lệ né hoàn toàn đòn đánh vật lý)
        BlockChance,    // Tỷ lệ đỡ đòn (Nếu có cơ chế đỡ)
        BlockReduction, // % sát thương giảm khi đỡ thành công

        // Chỉ số tiện ích (Utility Stats)
        MovementSpeed,  // Tốc độ di chuyển
        JumpHeight,     // Độ cao nhảy
        EffectResistance,// Kháng hiệu ứng (Giảm thời gian/khả năng bị dính hiệu ứng xấu)
        CooldownReduction,// Giảm thời gian hồi chiêu (%)
        Lifesteal,      // Hút máu (%)
        ManaSteal,      // Hút mana (%)

        // Chỉ số hồi phục (Regeneration Stats)
        HealthRegen,    // Hồi máu mỗi giây
        StaminaRegen,   // Hồi thể lực mỗi giây
        ManaRegen       // Hồi mana mỗi giây
    }

    /// <summary>
    /// Định nghĩa cách thức một chỉ số bị thay đổi bởi StatModifier.
    /// </summary>
    public enum ModifierType
    {
        Flat = 100,         // Cộng/trừ trực tiếp một giá trị cố định.
        PercentAdd = 200,   // Cộng/trừ một tỷ lệ % dựa trên giá trị GỐC (Base). Cộng dồn các PercentAdd.
        PercentMult = 300   // Nhân/chia một tỷ lệ % dựa trên giá trị SAU KHI tính Flat và PercentAdd. Nhân dồn các PercentMult.
    }

    /// <summary>
    /// Định nghĩa các trạng thái cơ bản cho AI (Trí tuệ nhân tạo).
    /// </summary>
    public enum AIStateType
    {
        Idle,       // Đứng yên/Không làm gì
        Patrol,     // Đi tuần tra theo lộ trình
        Chase,      // Đuổi theo mục tiêu
        Attack,     // Tấn công mục tiêu
        Flee,       // Bỏ chạy khỏi mục tiêu
        Dead        // Đã chết
    }
}
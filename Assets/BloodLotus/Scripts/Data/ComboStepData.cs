using UnityEngine; // Cần cho GameObject, AudioClip
using BloodLotus.Core; // Cần cho EffectType
using BloodLotus.Systems; // Cần cho SkillProgressionSystem
using BloodLotus.Data; // Cần cho ComboStepData, SkillData, WeaponData

namespace BloodLotus.Data
{
    [System.Serializable]
    public class ComboStepData
    {
        // --- Các trường bạn đã có ---
        [Tooltip("Tên gợi nhớ cho bước combo này (dùng cho debug hoặc editor).")]
        public string stepName = "Attack Step";

        [Tooltip("Tên của Trigger Parameter trong Animator Controller để kích hoạt animation cho bước này.")]
        public string animationTrigger = "Attack1"; // Nên có giá trị mặc định

        [Tooltip("Khoảng thời gian (giây) tối đa cho phép người chơi nhập lệnh cho bước combo TIẾP THEO. Sau thời gian này, combo sẽ reset.")]
        [Range(0.1f, 1.5f)]
        public float inputWindow = 0.5f;

        [Header("Combat Properties")] // Dùng Header để nhóm các trường trong Inspector
        [Tooltip("Hệ số nhân sát thương áp dụng cho bước này (nhân với sát thương cơ bản của vũ khí/skill). Ví dụ: 1.0 = 100%, 1.5 = 150%.")]
        public float damageMultiplier = 1.0f;

        // --- BỔ SUNG CÁC TRƯỜNG BỊ THIẾU ---
        [Tooltip("Thời gian (giây) game bị dừng (hitstop) khi đòn đánh của bước này trúng kẻ địch, tạo cảm giác 'impact'.")]
        [Range(0f, 0.5f)]
        public float hitstopDuration = 0.05f; // <<< THÊM DÒNG NÀY

        [Header("Effects & Sound")] // Dùng Header để nhóm lại
        [Tooltip("Prefab hiệu ứng hình ảnh (VFX) sẽ được tạo ra tại vị trí va chạm khi đòn đánh trúng.")]
        public GameObject vfxOnHitPrefab; // <<< THÊM DÒNG NÀY

        [Tooltip("Âm thanh (SFX) sẽ được phát khi đòn đánh trúng.")]
        public AudioClip sfxOnHit; // <<< THÊM CẢ CÁI NÀY (CombatComponent có thể sẽ dùng)

        [Tooltip("Âm thanh (SFX) sẽ được phát khi thực hiện động tác vung vũ khí (kể cả khi không trúng).")]
        public AudioClip sfxOnSwing; // <<< THÊM CẢ CÁI NÀY (CombatComponent hoặc AnimationComponent có thể dùng)

        // --- Các trường hiệu ứng bạn đã có (có thể đặt trong nhóm Effects) ---
        [Tooltip("Hiệu ứng trạng thái (EffectType) có thể gây ra bởi bước này.")]
        public EffectType effectType = EffectType.None;

        [Tooltip("Tỉ lệ phần trăm (0.0 đến 1.0) để gây ra hiệu ứng trạng thái khi đòn đánh trúng.")]
        [Range(0f, 1f)]
        public float effectChance = 0.0f;

        [Tooltip("Thời gian hiệu ứng trạng thái kéo dài (giây).")]
        public float effectDuration = 0.0f;

        [Tooltip("Sức mạnh/Giá trị của hiệu ứng (ví dụ: lượng damage mỗi giây của Poison, lượng slow).")]
        public float effectPotency = 0f; // <<< Thêm cái này nếu cần cho hiệu ứng
    }
}
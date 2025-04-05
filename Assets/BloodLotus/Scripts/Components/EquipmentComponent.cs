using UnityEngine;
using System.Collections.Generic;
using BloodLotus.Data; // Cần cho WeaponData, SkillData...
using BloodLotus.Core;  // Cần cho StatsComponent...

[RequireComponent(typeof(StatsComponent))]
public class EquipmentComponent : MonoBehaviour
{
    [Header("Current Equipment")]
    [field: SerializeField] public WeaponData CurrentWeapon { get; private set; }
    [field: SerializeField] public List<SkillData> EquippedSkills { get; private set; } = new List<SkillData>(); // Đảm bảo đã khởi tạo List
    [field: SerializeField] public InnerPowerData CurrentInnerPower { get; private set; }
    // public List<ArtifactData> EquippedArtifacts { get; private set; } = new List<ArtifactData>();

    [Header("Component References")]
    private Animator animator;
    private PlayerAnimationComponent playerAnim;
    private StatsComponent stats;
    private RuntimeAnimatorController baseAnimatorController;

    [Header("Starting Equipment (Optional)")]
    public WeaponData startingWeapon;

    [Header("Available Debug Items")] // Đổi tên nhóm để chứa cả vũ khí và skill debug
    [Tooltip("Danh sách các WeaponData để thử nghiệm bằng phím số (1-9).")]
    public List<WeaponData> debugWeaponsList = new List<WeaponData>();
    [Tooltip("Danh sách các SkillData để thử nghiệm bằng phím Shift + số (1-9).")]
    public List<SkillData> debugSkillsList = new List<SkillData>(); // <<< THÊM DANH SÁCH SKILL DEBUG

    // Sự kiện khi trang bị thay đổi
    public event System.Action OnEquipmentChanged;

    // --- Awake, Start, EquipWeapon, UnequipWeapon giữ nguyên như trước ---
     void Awake()
    {
        // Lấy các component cần thiết
        stats = GetComponent<StatsComponent>();
        playerAnim = GetComponent<PlayerAnimationComponent>(); // Hoặc GetComponentInChildren nếu cần
        animator = GetComponentInChildren<Animator>(); // Tìm Animator ở con hoặc cùng cấp

        // Kiểm tra và lưu trữ Animator Controller gốc
        if (animator != null)
        {
            baseAnimatorController = animator.runtimeAnimatorController;
            if (baseAnimatorController == null)
            {
                Debug.LogError("Animator trên Player/Child không có RuntimeAnimatorController gốc được gán!", this);
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy Animator component cho EquipmentComponent! Animation override sẽ không hoạt động.", this);
        }
    }

    private void Start()
    {
        // Trang bị vũ khí khởi đầu nếu có
        if (startingWeapon != null)
        {
            EquipWeapon(startingWeapon);
        }
    }
     public void EquipWeapon(WeaponData newWeapon)
    {
        // Không làm gì nếu vũ khí mới là null hoặc giống hệt vũ khí đang cầm
        if (newWeapon == null || newWeapon == CurrentWeapon)
        {
             if (newWeapon == CurrentWeapon) Debug.LogWarning($"Đã trang bị {newWeapon.weaponName} rồi.");
             return;
        }

        if (CurrentWeapon != null)
        {
             Debug.Log($"Thay thế {CurrentWeapon.weaponName} bằng {newWeapon.weaponName}");
        } else {
             Debug.Log($"Trang bị vũ khí: {newWeapon.weaponName}");
        }


        CurrentWeapon = newWeapon; // Gán vũ khí mới

        ApplyAnimationOverrides(); // Áp dụng animation mới
        RecalculateStats();       // Tính lại chỉ số
        OnEquipmentChanged?.Invoke(); // Thông báo thay đổi
    }

    public void UnequipWeapon()
    {
        if (CurrentWeapon == null) return; // Không có gì để gỡ

        Debug.Log($"Gỡ bỏ vũ khí: {CurrentWeapon.weaponName}");
        CurrentWeapon = null; // <<< Bước quan trọng nhất

        ApplyAnimationOverrides(); // <<< Gọi để xóa override
        RecalculateStats();       // <<< Tính lại chỉ số khi không có vũ khí
        OnEquipmentChanged?.Invoke(); // <<< Thông báo thay đổi
    }

    // --- CẬP NHẬT HÀM QUẢN LÝ SKILL ---

    public void EquipSkill(SkillData newSkill)
    {
        if (newSkill == null)
        {
            Debug.LogWarning("Cố gắng trang bị một Skill null.");
            return;
        }

        // (Tùy chọn) Giới hạn số lượng skill có thể trang bị
        // int maxSkillSlots = 4; // Ví dụ giới hạn 4 skill
        // if (EquippedSkills.Count >= maxSkillSlots) {
        //     Debug.LogWarning($"Đã đạt giới hạn số lượng skill ({maxSkillSlots}). Không thể trang bị thêm.");
        //     return;
        // }

        if (!EquippedSkills.Contains(newSkill))
        {
            EquippedSkills.Add(newSkill);
            Debug.Log($"Trang bị Skill: {newSkill.skillName} (Tổng: {EquippedSkills.Count})");
            // TODO: Gọi ApplyAnimationOverrides() nếu skill có thể override animation?
            // ApplyAnimationOverrides();
            RecalculateStats(); // Gọi nếu skill có thể cộng chỉ số bị động
            OnEquipmentChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Skill '{newSkill.skillName}' đã được trang bị rồi.");
        }
    }

    public void UnequipSkill(SkillData skillToRemove)
    {
        if (skillToRemove == null)
        {
             Debug.LogWarning("Cố gắng gỡ một Skill null.");
             return;
        }

        bool removed = EquippedSkills.Remove(skillToRemove); // Thử xóa và xem kết quả

        if (removed)
        {
            Debug.Log($"Gỡ bỏ Skill: {skillToRemove.skillName} (Còn lại: {EquippedSkills.Count})");
            // TODO: Gọi ApplyAnimationOverrides() nếu cần cập nhật animation
            // ApplyAnimationOverrides();
            RecalculateStats(); // Tính lại chỉ số
            OnEquipmentChanged?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Skill '{skillToRemove.skillName}' không được trang bị nên không thể gỡ.");
        }
    }

    // --- Equip/Unequip InnerPower giữ nguyên như trước ---
    public void EquipInnerPower(InnerPowerData newPower)
    {
         if (newPower == CurrentInnerPower) return;
         CurrentInnerPower = newPower;
         Debug.Log($"Trang bị Inner Power: {newPower?.powerName ?? "None"}");
         RecalculateStats();
         OnEquipmentChanged?.Invoke();
    }

     public void UnequipInnerPower() // Thêm hàm gỡ nếu cần
     {
         if (CurrentInnerPower == null) return;
         Debug.Log($"Gỡ bỏ Inner Power: {CurrentInnerPower.powerName}");
         CurrentInnerPower = null;
         RecalculateStats();
         OnEquipmentChanged?.Invoke();
     }

    // --- ApplyAnimationOverrides, RecalculateStats giữ nguyên như trước ---
     private void ApplyAnimationOverrides()
    {
        // Kiểm tra điều kiện cần thiết
        if (animator == null || baseAnimatorController == null)
        {
            return;
        }

        AnimatorOverrideController finalOverrideController = null;

        // 1. Lấy Override từ Vũ khí (Ưu tiên?)
        if (CurrentWeapon != null && CurrentWeapon.animationOverrideController != null)
        {
            finalOverrideController = CurrentWeapon.animationOverrideController;
        }

        // 2. TODO: Logic kết hợp/ghi đè bởi Skill
        // foreach (var skill in EquippedSkills) { ... }


        // 3. Áp dụng Controller cuối cùng hoặc quay về gốc
        if (finalOverrideController != null)
        {
            var instanceOverride = new AnimatorOverrideController(baseAnimatorController);
            List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            finalOverrideController.GetOverrides(overrides);
            instanceOverride.ApplyOverrides(overrides);
            animator.runtimeAnimatorController = instanceOverride;
        }
        else
        {
             if (animator.runtimeAnimatorController != baseAnimatorController) {
                 animator.runtimeAnimatorController = baseAnimatorController;
             }
        }
        playerAnim?.CacheAnimationHashes();
    }

    private void RecalculateStats()
    {
        stats?.CalculateFinalStats();
    }


    // --- CẬP NHẬT HÀM DEBUG UPDATE ---
    void Update()
    {
        // --- DEBUG VŨ KHÍ (Phím số 1-9) ---
        for (int i = 0; i < debugWeaponsList.Count; i++)
        {
            if (i >= 9) break; // Chỉ hỗ trợ phím 1-9
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (debugWeaponsList[i] != null)
                {
                    EquipWeapon(debugWeaponsList[i]);
                    break;
                }
            }
        }
        // Gỡ vũ khí (Phím U)
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnequipWeapon();
        }

        // --- DEBUG SKILL (Shift + Phím số 1-9) ---
        for (int i = 0; i < debugSkillsList.Count; i++)
        {
             if (i >= 9) break; // Chỉ hỗ trợ phím 1-9
            // Kiểm tra cả Left Shift và Right Shift
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (debugSkillsList[i] != null)
                {
                    SkillData skillToToggle = debugSkillsList[i];
                    // Kiểm tra xem skill đã trang bị chưa để Equip hoặc Unequip
                    if (EquippedSkills.Contains(skillToToggle))
                    {
                        UnequipSkill(skillToToggle);
                    }
                    else
                    {
                        EquipSkill(skillToToggle);
                    }
                    break; // Chỉ xử lý một skill mỗi frame
                }
            }
        }
         // Ví dụ: Gỡ Inner Power (Shift + U ?)
         // if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.U)) {
         //     UnequipInnerPower();
         // }
    }
}
using UnityEngine;
using System.Collections.Generic;
using BloodLotus.Data; // Giả sử WeaponData, SkillData... ở đây
using BloodLotus.Core;  // Giả sử StatsComponent ở đây

// Nên yêu cầu có StatsComponent vì RecalculateStats cần nó
[RequireComponent(typeof(StatsComponent))]
public class EquipmentComponent : MonoBehaviour
{
    [Header("Current Equipment")]
    // Sử dụng [field: SerializeField] để có thể thấy giá trị private trong Inspector (chủ yếu để debug)
    [field: SerializeField] public WeaponData CurrentWeapon { get; private set; }
    // Tương tự cho các trang bị khác nếu muốn debug trong Inspector
    [field: SerializeField] public List<SkillData> EquippedSkills { get; private set; } = new List<SkillData>();
    [field: SerializeField] public InnerPowerData CurrentInnerPower { get; private set; }
    // public List<ArtifactData> EquippedArtifacts { get; private set; } = new List<ArtifactData>(); // Bỏ qua Artifact nếu chưa cần

    [Header("Component References")]
    private Animator animator;
    private PlayerAnimationComponent playerAnim;
    private StatsComponent stats;
    private RuntimeAnimatorController baseAnimatorController; // << QUAN TRỌNG: Lưu Controller gốc

    [Header("Starting Equipment (Optional)")]
    [Tooltip("Vũ khí mặc định khi game bắt đầu. Có thể để trống.")]
    public WeaponData startingWeapon;

    // Sự kiện khi trang bị thay đổi
    public event System.Action OnEquipmentChanged;

    private void Awake()
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
                // Bạn PHẢI gán một Animator Controller vào component Animator trong Inspector
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

    // --- Quản Lý Vũ Khí ---

    public void EquipWeapon(WeaponData newWeapon)
    {
        // Không làm gì nếu vũ khí mới là null hoặc giống hệt vũ khí đang cầm
        if (newWeapon == null || newWeapon == CurrentWeapon)
        {
             if (newWeapon == CurrentWeapon) Debug.LogWarning($"Đã trang bị {newWeapon.weaponName} rồi.");
             return;
        }

        // (Tùy chọn) Tự động gỡ vũ khí cũ trước khi trang bị mới
        if (CurrentWeapon != null)
        {
             // Không cần gọi Unequip đầy đủ ở đây, vì các bước cập nhật sẽ được gọi ngay sau đó
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

    // --- Quản Lý Skill (Ví dụ cơ bản) ---

    public void EquipSkill(SkillData newSkill)
    {
        if (newSkill != null && !EquippedSkills.Contains(newSkill))
        {
            EquippedSkills.Add(newSkill);
            Debug.Log($"Trang bị Skill: {newSkill.skillName}");
            // TODO: Có thể cần ApplyAnimationOverrides() nếu skill ảnh hưởng animation
            RecalculateStats(); // Nếu skill cộng chỉ số
            OnEquipmentChanged?.Invoke();
        }
    }

    public void UnequipSkill(SkillData skillToRemove)
    {
        if (skillToRemove != null && EquippedSkills.Contains(skillToRemove))
        {
            EquippedSkills.Remove(skillToRemove);
            Debug.Log($"Gỡ bỏ Skill: {skillToRemove.skillName}");
            // TODO: Có thể cần ApplyAnimationOverrides()
            RecalculateStats();
            OnEquipmentChanged?.Invoke();
        }
    }

    // --- Quản Lý Inner Power (Ví dụ) ---
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


    // ... các hàm Equip/Unequip khác nếu cần ...


    // --- Hàm xử lý cốt lõi ---

    private void ApplyAnimationOverrides()
    {
        // Kiểm tra điều kiện cần thiết
        if (animator == null || baseAnimatorController == null)
        {
            // Đã log lỗi trong Awake, không cần log lại liên tục
            return;
        }

        AnimatorOverrideController finalOverrideController = null;

        // 1. Lấy Override từ Vũ khí (Ưu tiên?)
        if (CurrentWeapon != null && CurrentWeapon.animationOverrideController != null)
        {
            finalOverrideController = CurrentWeapon.animationOverrideController;
        }

        // 2. TODO: Logic kết hợp/ghi đè bởi Skill
        // Ví dụ đơn giản: Nếu có skill nào đó có override, dùng của skill thay vì vũ khí?
        // foreach (var skill in EquippedSkills) {
        //     if (skill.animationOverrideController != null && skill.IsActive) { // Giả sử có IsActive
        //         finalOverrideController = skill.animationOverrideController;
        //         break; // Lấy của skill đầu tiên tìm thấy?
        //     }
        // }


        // 3. Áp dụng Controller cuối cùng hoặc quay về gốc
        if (finalOverrideController != null)
        {
            // Có override -> Tạo instance mới từ gốc và áp dụng các thay đổi
            var instanceOverride = new AnimatorOverrideController(baseAnimatorController);
            List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            finalOverrideController.GetOverrides(overrides);
            instanceOverride.ApplyOverrides(overrides);
            animator.runtimeAnimatorController = instanceOverride;
             // Debug.Log($"Applied Override: {finalOverrideController.name}");
        }
        else
        {
            // Không có override nào được áp dụng -> Dùng Controller gốc
            // Chỉ gán lại nếu controller hiện tại không phải là gốc
             if (animator.runtimeAnimatorController != baseAnimatorController) {
                 animator.runtimeAnimatorController = baseAnimatorController;
                 // Debug.Log("Reverted to Base Animator Controller.");
             }
        }

        // Thông báo cho PlayerAnimationComponent cập nhật cache nếu cần thiết
        // Điều này quan trọng nếu bạn cache hash của các animation state/trigger
        playerAnim?.CacheAnimationHashes();
    }

    private void RecalculateStats()
    {
        // Yêu cầu StatsComponent tính toán lại tất cả chỉ số
        // StatsComponent sẽ tự lấy thông tin từ EquipmentComponent này
        stats?.CalculateFinalStats();
    }

    // --- Hàm Debug (Thêm vào để test bằng phím) ---
    void Update()
    {
        // Ví dụ: Nhấn U để gỡ vũ khí
        if (Input.GetKeyDown(KeyCode.U))
        {
            UnequipWeapon();
        }
        // Ví dụ: Nhấn E để trang bị lại vũ khí khởi đầu
        if (Input.GetKeyDown(KeyCode.E) && startingWeapon != null && CurrentWeapon == null)
        {
             EquipWeapon(startingWeapon);
        }
    }
}
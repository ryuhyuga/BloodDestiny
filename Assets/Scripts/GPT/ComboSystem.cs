using UnityEngine;
using System.Collections.Generic;


public class ComboSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EquipmentSystem equipmentSystem;
    [SerializeField] private PlayerCombat playerCombat;

    private List<ComboStep> currentCombo = new List<ComboStep>();
    private int comboIndex = 0;
    private bool isComboActive = false;

    // Hàng đợi input (mỗi input = true)
    private Queue<bool> attackQueue = new Queue<bool>();

    private void Awake()
    {
        if (equipmentSystem == null)
            equipmentSystem = GetComponent<EquipmentSystem>();
        if (playerCombat == null)
            playerCombat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        // Mỗi khung hình, nếu có input trong queue, 
        // và PlayerCombat đã sẵn sàng => thực hiện combo step tiếp
        if (attackQueue.Count > 0 && !playerCombat.isAttacking && playerCombat.attackCooldown <= 0f)
        {
            // Lấy 1 input từ queue
            attackQueue.Dequeue();
            TryComboAttack();
        }
    }

    /// <summary>
    /// Gọi từ PlayerInput mỗi khi người chơi bấm tấn công.
    /// </summary>
    public void EnqueueAttackInput()
    {
        attackQueue.Enqueue(true);
    }

    private void TryComboAttack()
    {
        // 1) Nếu combo chưa active => xây combo, bắt đầu
        if (!isComboActive)
        {
            BuildCombo();
            comboIndex = 0;
            isComboActive = true;
            Debug.Log($"[ComboSystem] Start combo with {currentCombo.Count} steps.");
        }
        else
        {
            // Nếu comboIndex >= comboCount => combo đã xong
            if (comboIndex >= currentCombo.Count)
            {
                EndCombo();
                return;
            }
        }

        // 2) Thực hiện đòn kế tiếp
        PerformNextStep();
    }

    private void BuildCombo()
    {
        currentCombo.Clear();

        if (equipmentSystem == null || equipmentSystem.currentWeapon == null)
        {
            Debug.LogWarning("[ComboSystem] No weapon or eqSys!");
            return;
        }

        WeaponBase weapon = equipmentSystem.currentWeapon;
        SkillBase skill = equipmentSystem.currentSkill;

        // Mở rộng combo nếu skill hợp hệ + unlock
        if (skill != null && skill.IsCompatibleWith(weapon.weaponType) && skill.unlockExtendedCombo)
        {
            currentCombo = skill.GetExtendedCombo(weapon.defaultComboSteps);
        }
        else
        {
            currentCombo = new List<ComboStep>(weapon.defaultComboSteps);
        }
    }

    private void PerformNextStep()
    {
        if (comboIndex >= currentCombo.Count)
        {
            EndCombo();
            return;
        }

        var step = currentCombo[comboIndex];

        // Kiểm tra skill level
        if (equipmentSystem.currentSkill != null && step.requiredSkillLevel > equipmentSystem.currentSkill.currentLevel)
        {
            Debug.LogWarning("[ComboSystem] Skill level too low for this step!");
            EndCombo();
            return;
        }

        playerCombat.PerformAttack(step);
        comboIndex++;
    }

    private void EndCombo()
    {
        Debug.Log("[ComboSystem] Combo ended.");
        isComboActive = false;
        comboIndex = 0;
        currentCombo.Clear();
    }
}

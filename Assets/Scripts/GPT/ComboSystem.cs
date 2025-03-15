using UnityEngine;
using System.Collections.Generic;

public class ComboSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EquipmentSystem equipmentSystem;
    [SerializeField] private PlayerCombat playerCombat;

    private List<ComboStep> currentCombo;
    private int comboIndex = 0;
    public bool isComboActive { get; private set; } = false;

    private void Awake()
    {
        // Tự động lấy EquipmentSystem, PlayerCombat nếu chưa gán trong Inspector
        if (equipmentSystem == null)
            equipmentSystem = GetComponent<EquipmentSystem>();

        if (playerCombat == null)
            playerCombat = GetComponent<PlayerCombat>();
    }

    public void StartCombo()
    {
        // Kiểm tra null references
        if (equipmentSystem == null)
        {
            Debug.LogError("[ComboSystem] equipmentSystem is NULL. Cannot start combo!");
            return;
        }

        if (equipmentSystem.currentWeapon == null)
        {
            Debug.LogWarning("[ComboSystem] No weapon equipped. Cannot start combo!");
            return;
        }

        // Lấy vũ khí và skill
        WeaponBase weapon = equipmentSystem.currentWeapon;
        SkillBase skill = equipmentSystem.currentSkill;

        // Nếu skill phù hợp vũ khí & unlock => combo mở rộng
        if (skill != null && skill.IsCompatibleWith(weapon.weaponType) && skill.unlockExtendedCombo)
        {
            currentCombo = skill.GetExtendedCombo(weapon.defaultComboSteps);
        }
        else
        {
            // Dùng combo mặc định của vũ khí
            currentCombo = new List<ComboStep>(weapon.defaultComboSteps);
        }

        comboIndex = 0;
        isComboActive = true;
        Debug.Log("[ComboSystem] Starting combo with " + currentCombo.Count + " steps.");

        PerformNextStep();
    }

    public void ContinueCombo()
    {
        if (!isComboActive || currentCombo == null)
        {
            Debug.Log("[ComboSystem] Combo not active or no combo steps. End combo.");
            EndCombo();
            return;
        }

        if (comboIndex >= currentCombo.Count)
        {
            EndCombo();
            return;
        }

        // Kiểm tra playerCombat null
        if (playerCombat == null)
        {
            Debug.LogError("[ComboSystem] playerCombat is NULL!");
            EndCombo();
            return;
        }

        // Nếu đang tấn công, chờ xong
        if (playerCombat.isAttacking)
        {
            Debug.Log("[ComboSystem] Still attacking, cannot continue combo yet.");
            return;
        }

        PerformNextStep();
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
            Debug.Log("[ComboSystem] Skill level too low for this step!");
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
    }
}

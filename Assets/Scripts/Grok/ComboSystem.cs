using UnityEngine;
using System.Collections.Generic;

public class ComboSystem : MonoBehaviour
{
    public List<ComboStep> currentCombo;
    private int currentStepIndex;
    private float bufferTimer;
    private bool isComboActive;

    [Header("Combo Timing")]
    public float inputBufferTime = 0.5f; // Thời gian chờ giữa các đòn

    private PlayerCombat playerCombat;
    private EquipmentSystem equipSys;

    // Level tối thiểu để mở khóa combo skill
    private int requiredLevelForSkillCombo = 2; // Ví dụ: cần level 2 để mở combo skill

    void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
        equipSys = GetComponent<EquipmentSystem>();
        ResetCombo();
    }

    void Update()
    {
        if (isComboActive)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    public void RegisterAttack(bool isHeavy)
    {
        if (!isComboActive)
        {
            StartCombo();
            return;
        }

        if (currentStepIndex < currentCombo.Count)
        {
            ContinueCombo();
        }
        else
        {
            ResetCombo();
        }
    }

    private void StartCombo()
    {
        currentCombo = GetComboSteps();
        if (currentCombo.Count > 0)
        {
            isComboActive = true;
            currentStepIndex = 0;
            bufferTimer = inputBufferTime;
            ExecuteComboStep();
        }
    }

    private void ContinueCombo()
    {
        if (isComboActive && currentStepIndex < currentCombo.Count)
        {
            bufferTimer = inputBufferTime;
            ExecuteComboStep();
        }
    }

    private void ExecuteComboStep()
    {
        if (currentStepIndex < currentCombo.Count)
        {
            ComboStep step = currentCombo[currentStepIndex];
            playerCombat.PerformAttack(step);
            currentStepIndex++;
        }
        else
        {
            ResetCombo();
        }
    }

    private List<ComboStep> GetComboSteps()
    {
        // 1) Kiểm tra skill và level để mở khóa combo skill
        if (equipSys.currentSkill != null &&
            equipSys.currentSkill.CanUseSkill(equipSys.currentWeapon) &&
            equipSys.currentSkill.skillLevel >= requiredLevelForSkillCombo)
        {
            return equipSys.currentSkill.GetSkillCombo(equipSys.currentSkill.skillLevel);
        }

        // 2) Nếu không đủ điều kiện skill, trả về 4 combo steps cơ bản của vũ khí
        if (equipSys.currentWeapon != null && equipSys.currentWeapon.baseComboSteps != null)
        {
            return new List<ComboStep>(
    equipSys.currentWeapon.baseComboSteps.GetRange(
        0, Mathf.Min(4, equipSys.currentWeapon.baseComboSteps.Count)
    )
);
        }

        // 3) Mặc định Fist (4 đòn cơ bản)
        return new List<ComboStep> {
            new ComboStep { animIndex = 1, forceCrit = false, damageMultiplier = 1f },
            new ComboStep { animIndex = 2, forceCrit = false, damageMultiplier = 1f },
            new ComboStep { animIndex = 3, forceCrit = false, damageMultiplier = 1f },
            new ComboStep { animIndex = 4, forceCrit = true, damageMultiplier = 1f }
        };
    }

    private void ResetCombo()
    {
        isComboActive = false;
        currentStepIndex = 0;
        bufferTimer = 0f;
        currentCombo = new List<ComboStep>();
    }
}
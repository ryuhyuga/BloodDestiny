using UnityEngine;
using BloodLotus.Core; // Cần cho EffectType, StatModifier...
using BloodLotus.Data; // Cần cho ComboStepData, InnerPowerData, SkillData...

// Gắn component này vào các đối tượng có thể nhận hiệu ứng (Player, Enemy)
[RequireComponent(typeof(StatsComponent))] // Thường đi kèm với StatsComponent
public class StatusEffectReceiver : MonoBehaviour
{
    // Danh sách lưu trữ các hiệu ứng đang hoạt động trên đối tượng này
    // private List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>(); // TODO: Cần định nghĩa class ActiveStatusEffect

    private StatsComponent stats;

    void Awake()
    {
        stats = GetComponent<StatsComponent>();
    }

    // TODO: Implement Update() để xử lý thời gian hiệu lực, áp dụng DoT/HoT...
    // void Update() { ProcessActiveEffects(); }

    /// <summary>
    /// Áp dụng hiệu ứng trạng thái từ một ComboStepData.
    /// </summary>
    public void ApplyStatusEffect(ComboStepData sourceStep)
    {
        if (sourceStep == null || sourceStep.effectType == EffectType.None) return;

        // TODO: Kiểm tra kháng hiệu ứng (EffectResistance) từ StatsComponent
        // float resistance = stats.GetFinalStatValue(StatType.EffectResistance);
        // if (Random.value < resistance) { Debug.Log("Effect Resisted!"); return; }

        Debug.Log($"Applying effect '{sourceStep.effectType}' from Combo Step on {gameObject.name} (Duration: {sourceStep.effectDuration}, Potency: {sourceStep.effectPotency})");

        // TODO: Logic cụ thể cho từng loại hiệu ứng
        // Ví dụ: Tạo một đối tượng ActiveStatusEffect, thêm vào list, bắt đầu bộ đếm thời gian...
        // HandleEffectStart(sourceStep.effectType, sourceStep.effectDuration, sourceStep.effectPotency);
    }

    /// <summary>
    /// Áp dụng hiệu ứng trạng thái từ một InnerPowerData.
    /// </summary>
    public void ApplyStatusEffect(InnerPowerData sourcePower)
    {
        if (sourcePower == null || sourcePower.effectOnHit == EffectType.None) return;

        // TODO: Kiểm tra kháng hiệu ứng
        Debug.Log($"Applying effect '{sourcePower.effectOnHit}' from Inner Power on {gameObject.name} (Duration: {sourcePower.effectDuration}, Potency: {sourcePower.effectPotency})");

        // TODO: Logic cụ thể
        // HandleEffectStart(sourcePower.effectOnHit, sourcePower.effectDuration, sourcePower.effectPotency);
    }

     /// <summary>
     /// Áp dụng hiệu ứng trạng thái từ một SkillData (Ví dụ: nếu skill có hiệu ứng riêng).
     /// </summary>
     public void ApplyStatusEffect(SkillData sourceSkill)
     {
         if (sourceSkill == null || sourceSkill.effectOnHit == EffectType.None) return;
         // TODO: Kiểm tra kháng, áp dụng logic
         Debug.Log($"Applying effect '{sourceSkill.effectOnHit}' from Skill on {gameObject.name} (Duration: {sourceSkill.effectDuration}, Potency: {sourceSkill.effectPotency})");
         // HandleEffectStart(sourceSkill.effectOnHit, sourceSkill.effectDuration, sourceSkill.effectPotency);
     }

     // --- Các hàm xử lý hiệu ứng nội bộ (TODO) ---
     // private void HandleEffectStart(EffectType type, float duration, float potency) { ... }
     // private void ProcessActiveEffects() { ... } // Xử lý trong Update
     // private void HandleEffectEnd(ActiveStatusEffect effect) { ... }
}

// --- TODO: Định nghĩa cấu trúc dữ liệu cho hiệu ứng đang hoạt động ---
// public class ActiveStatusEffect {
//     public EffectType Type;
//     public float DurationRemaining;
//     public float Potency;
//     public float TickTimer; // Cho DoT/HoT
//     public object Source; // Nguồn gốc hiệu ứng (để tránh stack?)
// }
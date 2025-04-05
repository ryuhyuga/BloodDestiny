using UnityEngine;
using System.Collections.Generic;
using BloodLotus.Core; // Thư viện chứa các enum, struct, class dùng chung

[CreateAssetMenu(fileName = "NewInnerPowerData", menuName = "Kiem Hiep/Data/Inner Power Data")]
public class InnerPowerData : ScriptableObject
{
    public string powerName = "New Inner Power";
    public List<StatModifier> passiveStatBonuses; // Bonus chỉ số cộng thẳng
    public EffectType effectOnHit = EffectType.None; // Hiệu ứng thêm vào đòn đánh thường/combo
    public float effectChance = 0f;
    public float effectDuration = 0f;
     [Tooltip("Sức mạnh/Giá trị của hiệu ứng (ví dụ: damage DoT, lượng stun duration cộng thêm?).")]
    public float effectPotency = 0f; // <<< Giá trị sức mạnh hiệu ứng
    public Sprite icon;
    [TextArea] public string description;
    // Thêm: Hiệu ứng đặc biệt khác (vd: hồi máu khi đánh crit...)
}
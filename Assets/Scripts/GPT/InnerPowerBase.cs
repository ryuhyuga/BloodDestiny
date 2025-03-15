using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "NewInnerPower", menuName = "ForbiddenLotus/InnerPower")]
public class InnerPowerBase : ScriptableObject
{
    public string powerName;
    public string description;

    [Header("Stats Bonus")]
    public float damageBonus;
    public float heavyDamageBonus;
    public float attackSpeedBonus;
    public float critRateBonus;
    public float critDamageBonus;
    public float armorBonus;
    public float maxHealthBonus;

    [Header("Effects")]
    public bool canStun;
    public bool canPoison;
    public bool canHealOverTime;

    public void ApplyPassive(PlayerStats stats)
    {
        stats.baseDamage += damageBonus;
        stats.baseHeavyDamage += heavyDamageBonus;
        stats.baseAttackSpeed += attackSpeedBonus;
        stats.baseCritRate += critRateBonus;
        stats.baseCritDamage += critDamageBonus;
        stats.baseArmor += armorBonus;
        stats.baseMaxHealth += maxHealthBonus;

        stats.ComputeFinalStats();
        Debug.Log($"[InnerPower] {powerName} applied to {stats.gameObject.name}");
    }
}

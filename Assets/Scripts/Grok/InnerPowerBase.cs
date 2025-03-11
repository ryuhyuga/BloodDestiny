using UnityEngine;

[CreateAssetMenu(fileName = "NewInnerPower", menuName = "ForbiddenLotus/InnerPower")]
public class InnerPowerBase : ScriptableObject
{
    public string powerName;

    [Header("Stats Bonus")]
    public float damageBonus = 0f;
    public float heavyDamageBonus = 0f;
    public float attackSpeedBonus = 0f;
    public float critRateBonus = 0f;
    public float critDamageBonus = 0f;
    public float armorBonus = 0f;
    public float maxHealthBonus = 0f;

    [Header("Level System")]
    public int currentLevel = 1;
    public int maxLevel = 5;
    public float[] levelExpRequirement; // Mảng exp cho mỗi cấp

    // Tùy logic nâng cấp
    public void LevelUp()
    {
        if (currentLevel < maxLevel)
            currentLevel++;
    }
}

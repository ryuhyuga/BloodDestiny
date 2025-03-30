// Ví dụ: WeaponData.cs
using UnityEngine;
using System.Collections.Generic;
using BloodLotus.Core;
using BloodLotus.Data; // Thư viện chứa các enum, struct, class dùng chung

public enum WeaponType
{
    Sword,
    Spear,
    Dagger,
    Bow,
    Staff
}

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Kiem Hiep/Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "New Weapon";
    public WeaponType weaponType = WeaponType.Sword;
    public float baseDamage = 10f;
    public List<ComboStepData> baseComboSequence; // List các bước combo cơ bản
    public AnimatorOverrideController animationOverrideController; // Để thay đổi animation
    public Sprite icon; // Icon hiển thị trong UI/Inventory
    public float attackRange = 1.5f; // Tầm đánh của vũ khí
    public float attackSpeed = 1.0f; // Tốc độ tấn công
    public EffectType specialEffect = EffectType.None; // Hiệu ứng đặc biệt của vũ khí
    public float effectChance = 0f; // Tỉ lệ kích hoạt hiệu ứng
    public float effectDuration = 0f; // Thời gian hiệu ứng kéo dài
}




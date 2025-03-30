using System.Collections.Generic;
using UnityEngine;
using BloodLotus.Core; // Thư viện chứa các enum, struct, class dùng chung

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Kiem Hiep/Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName = "New Enemy";
    public Stats baseStats; // Sử dụng struct/class Stats định nghĩa ở Core
    public AIStateType defaultAIState = AIStateType.Patrol;
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    // public LootTableData lootTable; // Tham chiếu đến SO LootTableData
    public RuntimeAnimatorController animatorController; // Animator gốc của enemy
    // Thêm: Tốc độ di chuyển, kiểu tấn công (melee/range), resistances...
}
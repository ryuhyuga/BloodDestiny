// WeaponHitboxConfig.cs
using UnityEngine;

[System.Serializable]
public class WeaponHitboxConfig
{
    public enum HitboxType { Circle, Box, Projectile }
    
    public HitboxType hitboxType = HitboxType.Circle;
    
    // Cho Circle
    public float radius = 1f;
    public Vector2 offset = Vector2.zero;
    
    // Cho Box
    public Vector2 size = Vector2.one;
    
    // Cho Projectile
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    
    // Chung
    public LayerMask targetLayers;
    public float duration = 0.2f;
}
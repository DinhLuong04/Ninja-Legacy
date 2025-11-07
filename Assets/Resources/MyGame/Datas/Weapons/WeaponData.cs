using UnityEngine;

public enum WeaponType { Sword, Shuriken }

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public WeaponType weaponType;

    public Sprite weaponSprite;
    public Vector3 localPosition;
    public Vector3 localRotation;
    public Vector3 localScale = Vector3.one;

    // Dùng cho Shuriken
    public GameObject projectilePrefab;
    public float projectileSpeed = 12f;
    public int damage = 10;
    public float fireRate = 0.3f;
    public bool useMouseAim = true;
}


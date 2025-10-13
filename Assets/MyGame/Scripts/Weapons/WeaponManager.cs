using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Transform weaponSlot;
    [SerializeField] private WeaponData[] weapons;

    private SpriteRenderer weaponRenderer;
    private int currentWeaponIndex = 0;

    // Sự kiện để UI lắng nghe
    public static Action<Sprite,String> OnWeaponChanged;

    void Start()
    {
        weaponRenderer = weaponSlot.GetComponent<SpriteRenderer>();
        if (weapons.Length > 0)
            EquipWeapon(0);
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        currentWeaponIndex = index;
        WeaponData weapon = weapons[index];

        // Đổi sprite trên Player
        weaponRenderer.sprite = weapon.weaponSprite;

        // Căn chỉnh
        weaponSlot.localPosition = weapon.localPosition;
        weaponSlot.localEulerAngles = weapon.localRotation;
        weaponSlot.localScale = weapon.localScale;

       
        OnWeaponChanged?.Invoke(weapon.weaponSprite,weapon.weaponName);
    }

    public WeaponData GetCurrentWeaponData()
    {
        if (currentWeaponIndex < 0 || currentWeaponIndex >= weapons.Length)
            return null;
        return weapons[currentWeaponIndex];
    }

    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);
    }
}

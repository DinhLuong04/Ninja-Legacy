using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Transform weaponSlot;
    [SerializeField] private WeaponData[] weapons;

    private SpriteRenderer weaponRenderer;
    private int currentWeaponIndex = 0;

    // Sự kiện để UI cập nhật icon/tên vũ khí
    public static Action<Sprite, string> OnWeaponChanged;

    void Start()
    {
        weaponRenderer = weaponSlot.GetComponent<SpriteRenderer>();

        if (weapons.Length > 0)
            EquipWeapon(0);
        else
            Debug.LogWarning("[WeaponManager] ⚠️ Chưa gán vũ khí nào!");
    }

    void Update()
    {
        // Dùng Q để chuyển vũ khí kế tiếp
        if (Input.GetKeyDown(KeyCode.Q))
        {
            NextWeapon();
        }
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        currentWeaponIndex = index;
        WeaponData weapon = weapons[index];

        if (weapon == null)
        {
            Debug.LogWarning("[WeaponManager] ⚠️ WeaponData null!");
            return;
        }

        // Cập nhật sprite + transform
        weaponRenderer.sprite = weapon.weaponSprite;
        weaponSlot.localPosition = weapon.localPosition;
        weaponSlot.localEulerAngles = weapon.localRotation;
        weaponSlot.localScale = weapon.localScale;

        // Gửi sự kiện để UI cập nhật icon/tên
        OnWeaponChanged?.Invoke(weapon.weaponSprite, weapon.weaponName);

        Debug.Log($"[WeaponManager] Equipped: {weapon.weaponName}");

        // Báo cho TutorialManager biết người chơi đã đổi vũ khí
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.NotifyWeaponSwitched();
    }

    private void NextWeapon()
    {
        if (weapons == null || weapons.Length == 0) return;

        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Length)
            currentWeaponIndex = 0;

        EquipWeapon(currentWeaponIndex);
    }

    public WeaponData GetCurrentWeaponData()
    {
        if (weapons == null || currentWeaponIndex < 0 || currentWeaponIndex >= weapons.Length)
            return null;

        return weapons[currentWeaponIndex];
    }
}

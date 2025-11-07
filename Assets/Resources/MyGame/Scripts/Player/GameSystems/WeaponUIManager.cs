using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class WeaponUIManager : MonoBehaviour
{
    [SerializeField] private Image weaponUIImage;
    [SerializeField] private TextMeshProUGUI weaponNameText; 

    void OnEnable()
    {
        WeaponManager.OnWeaponChanged += UpdateWeaponUI;
    }

    void OnDisable()
    {
        WeaponManager.OnWeaponChanged -= UpdateWeaponUI;
    }

    private void UpdateWeaponUI(Sprite weaponIcon, string weaponName)
    {
        weaponUIImage.sprite = weaponIcon;
        weaponUIImage.enabled = (weaponIcon != null);
        weaponNameText.text = weaponName ; 
    }
}
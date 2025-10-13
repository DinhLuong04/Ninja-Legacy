using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    public Text amountText;
    private int amount;

    public void SetItem(Sprite sprite, int count)
    {
        icon.sprite = sprite;
        icon.enabled = true;
        amount = count;
        UpdateUI();
    }

    public void AddItem(int count)
    {
        amount += count;
        UpdateUI();
    }

    public void UseItem()
    {
        if (amount > 0)
        {
            amount--;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        amountText.text = amount.ToString();
        if (amount <= 0)
        {
            icon.enabled = false;
            amountText.text = "";
        }
    }
}

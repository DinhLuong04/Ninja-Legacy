using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopPanelManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button buyButton;
    public Button closeButton;

    [Header("Item Info Display")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescText;
    public TextMeshProUGUI itemPriceText;

    private ShopItemSlot selectedSlot;

    private void Start()
    {
        buyButton.onClick.AddListener(OnBuyButton);
        closeButton.onClick.AddListener(CloseShop);
        ClearItemInfo();
    }

    public void SelectItem(ShopItemSlot slot)
{
    if (slot == null || slot.itemData == null)
    {
        Debug.LogWarning("Slot hoặc itemData null khi chọn!");
        return;
    }

    Debug.Log($"Chọn item: {slot.itemData.itemName}");
    selectedSlot = slot;

    itemNameText.text = slot.itemData.itemName;
    itemDescText.text = slot.itemData.description;
    itemPriceText.text = $"{slot.itemData.price} Yên";

    buyButton.interactable = true;
}

    private void OnBuyButton()
    {
        if (selectedSlot == null || selectedSlot.itemData == null)
        {
            Debug.LogWarning("Chưa chọn item để mua!");
            return;
        }

        var player = PlayerStats.Instance;
        var inventory = InventoryManager.Instance;

        if (player == null || inventory == null)
        {
            Debug.LogWarning("Thiếu PlayerStats hoặc InventoryManager!");
            return;
        }

        int price = selectedSlot.itemData.price;

        if (player.gold >= price)
        {
            player.SpendGold(price);
            inventory.AddItem(selectedSlot.itemData);
            Debug.Log($"Mua thành công {selectedSlot.itemData.itemName}");
        }
        else
        {
            Debug.Log("Không đủ tiền để mua!");
        }
    }

    private void CloseShop()
    {
        gameObject.SetActive(false);
    }

    private void ClearItemInfo()
    {
        itemNameText.text = "";
        itemDescText.text = "";
        itemPriceText.text = "";
        buyButton.interactable = false;
    }
}

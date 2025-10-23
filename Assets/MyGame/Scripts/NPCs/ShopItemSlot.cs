using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour
{
    public ItemData itemData;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI descriptionText;
    public Button selectButton; 

    private ShopPanelManager shopManager;
     private void Start()
    {
        if (itemData != null)
        {
            if (shopManager == null)
                shopManager = FindObjectOfType<ShopPanelManager>();

            Setup(itemData, shopManager);
        }
    }
    public void Setup(ItemData data, ShopPanelManager manager)
    {
        itemData = data;
        shopManager = manager;

        icon.sprite = data.icon;
        nameText.text = data.itemName;
        priceText.text = data.price + " Yên";

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => shopManager.SelectItem(this));
    }
}

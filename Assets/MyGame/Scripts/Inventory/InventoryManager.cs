using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    [Header("Inventory Slots")]
    public Transform grid;                  // Grid chứa các slot
    private GameObject[] slots;             // Danh sách slot
    private ItemData[] slotItems;           // Item tương ứng mỗi slot
    public int slotCount = 20;

    [Header("Item Info Panel")]
    public GameObject itemInfoPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Button useButton;

    [Header("References")]
    public PlayerStats playerStats;
    public ItemUseHandler itemUseHandler;

     private
    void Awake()
{
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    if (itemUseHandler == null)
        itemUseHandler = FindObjectOfType<ItemUseHandler>();
}
    void Start()
    {
        if (grid == null)
        {
            Debug.LogError(" Grid chưa được gán trong InventoryManager!");
            return;
        }

        slotCount = Mathf.Min(slotCount, grid.childCount);
        slots = new GameObject[slotCount];
        slotItems = new ItemData[slotCount];

        // Gán slot và sự kiện click
        for (int i = 0; i < slotCount; i++)
        {
            slots[i] = grid.GetChild(i).gameObject;
            int index = i;
            Button btn = slots[i].GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => ShowItemInfo(index));

            // Clear UI ban đầu
            Transform itemT = slots[i].transform.Find("Item");
            Transform countT = slots[i].transform.Find("Count");
            if (itemT) itemT.GetComponent<Image>().sprite = null;
            if (countT) countT.GetComponent<TextMeshProUGUI>().text = "";
        }

        itemInfoPanel.SetActive(false);
    }

    public void AddItem(ItemData newItem)
    {
        for (int i = 0; i < slotCount; i++)
        {
            Image itemImage = slots[i].transform.Find("Item").GetComponent<Image>();
            TextMeshProUGUI countText = slots[i].transform.Find("Count").GetComponent<TextMeshProUGUI>();

            if (itemImage.sprite == null)
            {
                // add item mới vào slot trống
                itemImage.sprite = newItem.icon;
                itemImage.enabled = true;
                countText.text = "";
                slotItems[i] = newItem;
                Debug.Log($"Added {newItem.itemName} to slot {i}");
                return;
            }
            else if (slotItems[i] == newItem)
            {
                // Nếu item đã tồn tại, tăng count
                int count = string.IsNullOrEmpty(countText.text) ? 1 : int.Parse(countText.text);
                count++;
                countText.text = count >= 2 ? count.ToString() : "";
                Debug.Log($" Increased {newItem.itemName} count to {count}");
                return;
            }
        }

        Debug.Log("Inventory full!");
    }

    public void ShowItemInfo(int index)
    {
        if (index < 0 || index >= slotCount || slotItems[index] == null)
            return;

        ItemData item = slotItems[index];
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;

        useButton.gameObject.SetActive(item.usable);
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() => UseItem(index));

        itemInfoPanel.SetActive(true);
    }

    public void UseItem(int index)
{
    if (slotItems[index] == null) return;

    ItemData item = slotItems[index];
    Debug.Log($"Using {item.itemName}");

    if (item.usable)
        itemUseHandler.UseItem(item);

    DecreaseItemCount(index);
    itemInfoPanel.SetActive(false);
}

    private void DecreaseItemCount(int index)
    {
        Image itemImage = slots[index].transform.Find("Item").GetComponent<Image>();
        TextMeshProUGUI countText = slots[index].transform.Find("Count").GetComponent<TextMeshProUGUI>();

        int count = string.IsNullOrEmpty(countText.text) ? 1 : int.Parse(countText.text);

        if (count > 1)
        {
            count--;
            countText.text = count >= 2 ? count.ToString() : "";
        }
        else
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
            countText.text = "";
            slotItems[index] = null;
        }
    }
}

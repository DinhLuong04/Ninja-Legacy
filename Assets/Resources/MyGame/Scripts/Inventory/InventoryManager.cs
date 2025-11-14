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
    private PlayerStats playerStats;
    public ItemUseHandler itemUseHandler;
    
    void Awake()
{
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
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
        playerStats = PlayerStats.Instance;
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
            if (itemT)
            {
                Image img = itemT.GetComponent<Image>();
                img.sprite = null;
                img.enabled = false; 
            } 
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
                NotificationManager.Instance.Show($"Bạn đã nhặt được {newItem.itemName} ");
                if(QuickUseManager.Instance != null)
                QuickUseManager.Instance.UpdateButton(newItem.itemType);
                return;
            }
            else if (slotItems[i] == newItem)
            {
                // Nếu item đã tồn tại, tăng count
                int count = string.IsNullOrEmpty(countText.text) ? 1 : int.Parse(countText.text);
                count++;
                countText.text = count >= 2 ? count.ToString() : "";
                Debug.Log($" Increased {newItem.itemName} count to {count}");
                if(QuickUseManager.Instance != null)
                QuickUseManager.Instance.UpdateButton(newItem.itemType);
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
    if(QuickUseManager.Instance != null)
        QuickUseManager.Instance.UpdateButton(item.itemType);
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

    public int CountItem(ItemType type)
    {
        int count = 0;
        for (int i = 0; i < slotItems.Length; i++)
        {
            if (slotItems[i] != null && slotItems[i].itemType == type)
            {
                TextMeshProUGUI countText = slots[i].transform.Find("Count").GetComponent<TextMeshProUGUI>();
                int c = string.IsNullOrEmpty(countText.text) ? 1 : int.Parse(countText.text);
                count += c;
            }
        }
        return count;
    }
public ItemData GetFirstItemOfType(ItemType type)
{
    for (int i = 0; i < slotItems.Length; i++)
    {
        if (slotItems[i] != null && slotItems[i].itemType == type)
            return slotItems[i];
    }
    return null;
}

public void RemoveOneItem(ItemData item)
{
    for (int i = 0; i < slotItems.Length; i++)
    {
        if (slotItems[i] == item)
        {
            DecreaseItemCount(i);
            return;
        }
    }
}
public void ResetForRealGame()
{
    for (int i = 0; i < slotItems.Length; i++)
    {
        slotItems[i] = null;
        Transform itemT = slots[i].transform.Find("Item");
        Transform countT = slots[i].transform.Find("Count");

        if (itemT != null) itemT.GetComponent<Image>().sprite = null;
        if (itemT != null) itemT.GetComponent<Image>().enabled = false;
        if (countT != null) countT.GetComponent<TextMeshProUGUI>().text = "";
    }
}
}
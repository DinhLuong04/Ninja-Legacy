using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickUseManager : MonoBehaviour
{
    public static QuickUseManager Instance;

    [Header("Quick Use Buttons")]
    public Button hpButton;
    public TextMeshProUGUI hpCountText;
    public Button mpButton;
    public TextMeshProUGUI mpCountText;

    private InventoryManager inventory;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        
    }

    private void Start()
    {
        if (hpButton != null)
            hpButton.onClick.AddListener(() => UsePotion(ItemType.Potion));
        if (mpButton != null)
            mpButton.onClick.AddListener(() => UsePotion(ItemType.ManaPotion));
        inventory = InventoryManager.Instance;
        UpdateAllButtons();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
            UsePotion(ItemType.Potion);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            UsePotion(ItemType.ManaPotion);
    }

    public void UsePotion(ItemType type)
    {
        ItemData item = inventory.GetFirstItemOfType(type);
        if (item != null)
        {
            inventory.RemoveOneItem(item);
            inventory.itemUseHandler.UseItem(item);
            UpdateButton(type);
        }
        else
        {
            if (type == ItemType.Potion)
                NotificationManager.Instance.Show("Bạn đã hết bình HP!");
            else if (type == ItemType.ManaPotion)
                NotificationManager.Instance.Show("Bạn đã hết bình MP!");
        }
    }

    public void UpdateButton(ItemType type)
    {
        int count = inventory.CountItem(type);
        if (type == ItemType.Potion)
            hpCountText.text = count > 0 ? count.ToString() : "0";
        else if (type == ItemType.ManaPotion)
            mpCountText.text = count > 0 ? count.ToString() : "0";
    }

    public void UpdateAllButtons()
    {
        UpdateButton(ItemType.Potion);
        UpdateButton(ItemType.ManaPotion);
    }
}

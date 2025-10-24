using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffPanelManager : MonoBehaviour
{
    public static BuffPanelManager Instance;

    [SerializeField] private Transform buffContainer;
    [SerializeField] private GameObject buffSlotPrefab;

    private class ActiveBuff
    {
        public ItemData item;
        public float remainingTime;
        public GameObject uiObject;
        public TextMeshProUGUI timeText;
        public Image icon;
    }

    private Dictionary<ItemData, ActiveBuff> activeBuffs = new();
    private ItemData currentFood; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        List<ItemData> expired = new();

        foreach (var kvp in activeBuffs)
        {
            var buff = kvp.Value;
            buff.remainingTime -= Time.deltaTime;

            if (buff.remainingTime <= 0)
                expired.Add(kvp.Key);
            else
                buff.timeText.text = FormatTime(buff.remainingTime);
        }

        foreach (var b in expired)
            RemoveBuff(b);
    }

    public void AddOrResetBuff(ItemData item)
    {
        if (item.itemType != ItemType.Buff && item.itemType != ItemType.Food)
            return;

        //  Food — chỉ 1 loại
        if (item.itemType == ItemType.Food)
        {
            if (currentFood != null && activeBuffs.ContainsKey(currentFood))
                RemoveBuff(currentFood);
            currentFood = item;
        }

        // Nếu item đã tồn tại → reset time
        if (activeBuffs.ContainsKey(item))
        {
            activeBuffs[item].remainingTime = item.duration;
            activeBuffs[item].timeText.text = FormatTime(item.duration);
            Debug.Log($"⏱ Reset thời gian buff/food {item.itemName}");
            return;
        }

        // Tạo UI mới
        GameObject slot = Instantiate(buffSlotPrefab, buffContainer);
        Image icon = slot.transform.Find("Icon").GetComponent<Image>();
        TextMeshProUGUI text = slot.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();

        icon.sprite = item.icon;
        text.text = FormatTime(item.duration);

        ActiveBuff newBuff = new()
        {
            item = item,
            remainingTime = item.duration,
            uiObject = slot,
            timeText = text,
            icon = icon
        };

        activeBuffs[item] = newBuff;
    }

    public void RemoveBuff(ItemData item)
    {
        if (!activeBuffs.ContainsKey(item)) return;

        Destroy(activeBuffs[item].uiObject);
        activeBuffs.Remove(item);

        if (currentFood == item)
            currentFood = null;
    }

    private string FormatTime(float seconds)
    {
        if (seconds >= 3600)
            return $"{(int)(seconds / 3600)}h";
        else if (seconds >= 60)
            return $"{(int)(seconds / 60)}m";
        else
            return $"{Mathf.CeilToInt(seconds)}s";
    }

    //  Trả về tất cả buff đang hoạt động (cho PlayerStats tính toán)
    public List<ItemData> GetActiveBuffs()
    {
        List<ItemData> list = new();
        foreach (var kvp in activeBuffs)
        {
            if (kvp.Key.itemType == ItemType.Buff)
                list.Add(kvp.Key);
        }
        return list;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseHandler : MonoBehaviour
{
    public static ItemUseHandler Instance;
    private PlayerStats player;

    private Coroutine currentFoodCoroutine;
    private ItemData currentFoodItem;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        player = FindObjectOfType<PlayerStats>();
    }

    public void UseItem(ItemData item)
    {
        if (item == null || player == null) return;

        switch (item.itemType)
        {
            case ItemType.Potion:
                player.Heal(item.healAmount);
                break;

            case ItemType.ManaPotion:
                player.RestoreMana(item.manaAmount);
                break;

            case ItemType.Food:
                StartCoroutine(ApplyFoodEffect(item));
                break;

            case ItemType.Buff:
                StartCoroutine(ApplyBuff(item));
                break;
        }
    }

    //  Chỉ 1 món ăn có hiệu lực
    private IEnumerator ApplyFoodEffect(ItemData food)
    {
        if (currentFoodCoroutine != null)
        {
            StopCoroutine(currentFoodCoroutine);
            BuffPanelManager.Instance.RemoveBuff(currentFoodItem);
        }

        currentFoodItem = food;
        currentFoodCoroutine = StartCoroutine(FoodRoutine(food));
        yield break;
    }

    private IEnumerator FoodRoutine(ItemData food)
    {
        BuffPanelManager.Instance.AddOrResetBuff(food);
        Debug.Log($" Bắt đầu hiệu ứng: {food.itemName}");

        float elapsed = 0;
        while (elapsed < food.duration)
        {
            if (food.healPerSecond > 0)
                player.Heal(Mathf.RoundToInt(food.healPerSecond));
            if (food.manaPerSecond > 0)
                player.RestoreMana(Mathf.RoundToInt(food.manaPerSecond));

            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($" Hết hiệu ứng: {food.itemName}");
        BuffPanelManager.Instance.RemoveBuff(food);
        currentFoodItem = null;
        currentFoodCoroutine = null;
    }

    // Buff — có thể nhiều loại cùng lúc
    private IEnumerator ApplyBuff(ItemData buff)
    {
        BuffPanelManager.Instance.AddOrResetBuff(buff);
        Debug.Log($"Áp dụng buff: {buff.itemName}");

        player.RecalculateFromBuffs(); // Tính lại stat sau khi thêm buff

        yield return new WaitForSeconds(buff.duration);

        Debug.Log($"💊 Hết hiệu lực: {buff.itemName}");
        BuffPanelManager.Instance.RemoveBuff(buff);
        player.RecalculateFromBuffs(); // Tính lại stat sau khi hết buff
    }
}

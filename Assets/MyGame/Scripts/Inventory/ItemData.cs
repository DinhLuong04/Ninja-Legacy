using UnityEngine;

public enum ItemType
{
    Potion,
    ManaPotion,
    Food,
    Buff
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public ItemType itemType;

    [Header("Base Stats")]
    public int price = 10;
    public bool usable = false;

    [Header("Instant Effects")]
    public int healAmount;
    public int manaAmount;

    [Header("Over Time / Buff Effects")]
    public float duration = 0f; // Thời gian tồn tại buff hoặc hồi máu theo thời gian
    public float healPerSecond = 0f;
    public float manaPerSecond = 0f;

    public float damageMultiplier = 1f;
    public float hpMultiplier = 1f;
    public float mpMultiplier = 1f;
}

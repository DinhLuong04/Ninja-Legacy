using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData; 
    public InventoryManager inventory;

   void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        if (inventory == null)
        {
            Debug.LogError("InventoryManager not assigned in ItemPickup!");
            return;
        }
        if (itemData == null)
        {
            Debug.LogError("ItemData not assigned in ItemPickup!");
            return;
        }
        inventory.AddItem(itemData);
        Destroy(gameObject);
    }
}
}

using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData; 
    public AudioClip pickupSound; 
    [Range(0f, 1f)] public float volume = 0.5f; 

    private InventoryManager inventory;

    void Start()
    {
        if (inventory == null)
            inventory = InventoryManager.Instance;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (inventory == null)
            {
                return;
            }
            if (itemData == null)
            {
                return;
            }

            inventory.AddItem(itemData);
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
            Destroy(transform.parent.gameObject);
        }
    }
}

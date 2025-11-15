using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public int goldAmount = 10;
    public AudioClip pickupSound; 
    [Range(0f, 1f)] public float volume = 0.5f;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            PlayerStats ps = PlayerStats.Instance;
            if (ps != null)
            {
                ps.AddGold(goldAmount);
                NotificationManager.Instance.Show($"Bạn đã nhận được {goldAmount} vàng");
            }

            collected = true;

            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);

            Destroy(transform.parent.gameObject);
        }
    }
}

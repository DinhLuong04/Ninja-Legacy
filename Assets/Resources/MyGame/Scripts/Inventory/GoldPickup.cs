using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public int goldAmount = 10;
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
                Debug.Log($"Player nhận {goldAmount} vàng");
            }
            collected = true;
            Destroy(transform.parent.gameObject);
        }
    }
}

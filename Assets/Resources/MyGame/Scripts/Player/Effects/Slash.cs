using UnityEngine;

public class Slash : MonoBehaviour
{
    public int damage;
    public float lifetime = 0.2f; 

    void Awake()
    {
        // Lấy damage từ PlayerStats
        PlayerStats playerStats = PlayerStats.Instance;
        if (playerStats != null)
            damage = Mathf.RoundToInt(playerStats.GetDamage() / 3f);
    }

    void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}

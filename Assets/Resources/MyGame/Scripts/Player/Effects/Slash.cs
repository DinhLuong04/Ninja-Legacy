using UnityEngine;

public class Slash : MonoBehaviour
{
    [SerializeField] public int damage = 15;
    public float lifetime = 0.2f; 

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

using UnityEditor;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    private Rigidbody2D rb;
    
    private int damage ;
    [SerializeField] private float rotateSpeed = 720f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerStats playerStats = PlayerStats.Instance;
        damage =  Mathf.RoundToInt(playerStats.GetDamage() * 0.8f);
    }

    public void Launch(Vector2 direction, float speed, int dmg)
    {
        
        rb.velocity = direction * speed;
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
    if (other.CompareTag("Enemy"))
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}


}

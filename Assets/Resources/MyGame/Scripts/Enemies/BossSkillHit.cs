using UnityEngine;

public class BossSkillHit : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageMultiplier = 1f;
    public float knockbackForce = 6f;
    public bool destroyOnHit = true;
    private int damage;

    private Enemy boss; 

    public void Init(Enemy bossRef)
    {
        boss = bossRef;
        damage = Mathf.RoundToInt(boss.GetDame() * damageMultiplier);
    }

    void Start()
    {
        
        if (boss == null)
        {
            boss = GetComponentInParent<Enemy>();
            if (boss != null)
            {
                damage = Mathf.RoundToInt(boss.GetDame() * damageMultiplier);
            }
            else
            {
                Debug.LogWarning($"[{name}] Không tìm thấy Enemy/Boss cha! (chưa Init)");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var playerStats = PlayerStats.Instance;
        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
            Debug.Log($"[BOSS SKILL] Dame: {damage} | Knockback: {knockbackForce}");
            
            var rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (collision.transform.position - transform.position).normalized;
                if (dir.sqrMagnitude < 0.01f) dir = Vector2.up;
                rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
        }

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
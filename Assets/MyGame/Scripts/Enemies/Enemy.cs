using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 50;
    private int currentHP;
    public int damage = 10;
    public int expReward = 20;

    [Header("Type")]
    public EnemyType enemyType;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    private Transform player;

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        animator.SetBool("isHurt", false);
        animator.SetBool("isDie", false);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        animator.SetBool("isHurt", true);

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            
            Invoke(nameof(ResetHurt), 0.3f);
        }
    }

    void ResetHurt()
    {
        animator.SetBool("isHurt", false);
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("isDie", true);

        rb.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        // Thưởng EXP
        if (player != null)
        {
            PlayerStats ps = player.GetComponent<PlayerStats>();
            if (ps != null)
            {
                ps.GainExp(expReward);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy PlayerStats trên Player!");
            }
        }
        QuestManager.Instance?.EnemyKilled(enemyType);
        Destroy(gameObject, 1f); 
        Destroy(GetComponentInChildren<EnemyHealthBar>()?.gameObject);

    }
    public int GetCurrentHP()
    {
         return currentHP;
    }
       
}

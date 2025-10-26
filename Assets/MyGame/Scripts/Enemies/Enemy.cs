using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 50;
    public int damage = 10;
    public int expReward = 20;

    [Header("Type")]
    public EnemyType enemyType;
    [Header("Item Drop Settings")]
    [Tooltip("Danh sách vật phẩm có thể rơi khi quái chết")]
    public GameObject[] dropPrefabs; 

    [Range(0f, 1f)]
    public float dropChance = 0.5f; // Tỉ lệ rơi 

    [Tooltip("Số lượng vật phẩm tối đa rơi ra (Boss có thể cao hơn)")]
    public int maxDropCount = 1;

    public bool isBoss = false;

    [Header("References")]
    public Animator animator;

    [Header("Respawn")]
    public GameObject enemyPrefab;
    private EnemyRespawner respawner;
    [Header("AI Settings")]
    public float attackRange = 1f;
    protected int currentHP;
    protected Transform player;
    protected bool isDead = false;

    protected float lastAttackTime = 0f;
    public float attackCooldown = 1.5f;
    private Vector3 spawnPosition;
    protected virtual void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spawnPosition = transform.position;
        if (animator == null) animator = GetComponent<Animator>();

        respawner = FindObjectOfType<EnemyRespawner>();
        if (respawner == null) Debug.LogError("Không tìm thấy EnemyRespawner trong scene!");
        if (enemyPrefab == null) Debug.LogError("Enemy prefab chưa gán trong " + gameObject.name);
    }

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        if (animator != null) animator.SetBool("isHurt", true);

        if (currentHP <= 0) Die();
        else Invoke(nameof(ResetHurt), 0.3f);
    }

    void ResetHurt()
    {
        if (animator != null) animator.SetBool("isHurt", false);
    }

    protected virtual void Die()
    {
        isDead = true;
        if (animator != null) animator.SetBool("isDie", true);

        GetComponent<Collider2D>().enabled = false;

        if (player != null)
        {
            PlayerStats ps = player.GetComponent<PlayerStats>();
            if (ps != null) ps.GainExp(expReward);
        }

        QuestManager.Instance?.EnemyKilled(enemyType);

       
        Debug.Log("Enemy prefab in Die: " + enemyPrefab?.name);
        if (respawner != null) respawner.ScheduleRespawn(enemyPrefab, spawnPosition);
        DropItems();
        Destroy(GetComponentInChildren<EnemyHealthBar>()?.gameObject);
        Destroy(gameObject, 1f);
    }

    protected virtual void DropItems()
{
    if (dropPrefabs == null || dropPrefabs.Length == 0) return;

    // Nếu là boss thì rơi nhiều món hơn
    int dropCount = isBoss ? maxDropCount : 1;
    dropCount = Mathf.Min(dropCount, dropPrefabs.Length);

    for (int i = 0; i < dropCount; i++)
    {
        // Chọn ngẫu nhiên 1 vật phẩm từ danh sách
        GameObject dropPrefab = dropPrefabs[UnityEngine.Random.Range(0, dropPrefabs.Length)];
        if (dropPrefab == null) continue;

        // Vị trí rơi hơi lệch xung quanh quái
        Vector3 dropPos = transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0.5f, 0);
        GameObject drop = Instantiate(dropPrefab, dropPos, Quaternion.identity);

        // Bắn nhẹ item ra xung quanh
        Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 forceDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), 1f).normalized;
            rb.AddForce(forceDir * UnityEngine.Random.Range(2f, 3f), ForceMode2D.Impulse);
        }

        Debug.Log($"{gameObject.name} dropped {dropPrefab.name}");
    }
}


    public void DealDamage()
    {
        if (player == null)
        {
            Debug.LogError("Player is null in DealDamage!");
            return;
        }
        float distance = Vector2.Distance(transform.position, player.position);
        Debug.Log("DealDamage called at " + Time.time + ", Distance to player: " + distance);
        if (distance <= attackRange)
        {
            PlayerStats ps = player.GetComponent<PlayerStats>();
            if (ps != null)
            {
                ps.TakeDamage(damage);
                Debug.Log("Damage " + damage + " dealt to player");
            }
            else
            {
                Debug.LogError("PlayerStats not found on " + player.name);
            }
        }
        else
        {
            Debug.LogWarning("Distance (" + distance + ") exceeds " + attackRange + "f, no damage dealt");
        }
    }

    protected void FlipSprite(float dir)
    {
        if (dir > 0.1f) transform.localScale = new Vector3(1, 1, 1);
        else if (dir < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
    }

    protected abstract void HandleAI();

    protected virtual void Update()
    {
        if (isDead || player == null) return;

        HandleAI();
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }
}

using System;
using System.Collections;
using Unity.VisualScripting;
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
    protected PlayerStats ps;
    protected virtual void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        spawnPosition = transform.position;
        if (animator == null) animator = GetComponent<Animator>();
        ps = PlayerStats.Instance;
        respawner = EnemyRespawner.Instance;
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
            if (ps != null) ps.GainExp(expReward);
        }

        QuestManager.Instance?.EnemyKilled(enemyType);

       
        Debug.Log("Enemy prefab in Die: " + enemyPrefab?.name);
        if (respawner != null &&!isBoss) respawner.ScheduleRespawn(enemyPrefab, spawnPosition);
        DropItems();
        Destroy(GetComponentInChildren<EnemyHealthBar>()?.gameObject);
        Destroy(gameObject, 1f);
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.OnEnemyKilled();
        }
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
    if (player == null || ps == null) return;

    Collider2D enemyCol = GetComponent<Collider2D>();
    Collider2D playerCol = player.GetComponent<Collider2D>();

    if (enemyCol == null || playerCol == null) return;

    // Tâm đến tâm
    float centerDistance = Vector2.Distance(enemyCol.bounds.center, playerCol.bounds.center);

    // Bán kính cộng lại (nửa chiều rộng theo trục X)
    float combinedHalfExtents = enemyCol.bounds.extents.x + playerCol.bounds.extents.x;

    // Khoảng cách thực tế giữa 2 bề mặt
    float surfaceDistance = centerDistance - combinedHalfExtents;

    // Debug để kiểm tra
    Debug.Log($"Surface Distance: {surfaceDistance:F2} / AttackRange: {attackRange}");

    if (surfaceDistance <= attackRange)
    {
        ps.TakeDamage(damage);
        Debug.Log("DAMAGE DEALT! (Surface contact)");
    }
}

    protected void FlipSprite(float dir)
{
    Vector3 scale = transform.localScale;

    if (dir > 0.1f)
        scale.x = Mathf.Abs(scale.x); 
    else if (dir < -0.1f)
        scale.x = -Mathf.Abs(scale.x); 

    transform.localScale = scale;
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
    public virtual IEnumerator UseBossSkill(int skillId)
    {
        yield return null;
    }
    public bool IsDead => isDead;

    public int GetDame()
    {
        return damage;
    }

    protected bool CanDetectPlayer()
{
    var stealth = player?.GetComponent<PlayerStealth>();
    if (stealth != null && stealth.isStealthed)
        return false;
    return true;
}
}

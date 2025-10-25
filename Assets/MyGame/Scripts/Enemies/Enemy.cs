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

    protected virtual void Start()
    {
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (animator == null) animator = GetComponent<Animator>();

        respawner = FindObjectOfType<EnemyRespawner>();
        if (respawner == null) Debug.LogError("Không tìm thấy EnemyRespawner trong scene!");
        if( enemyPrefab == null) Debug.LogError("Enemy prefab chưa gán trong " + gameObject.name);
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

        Vector3 spawnPosition = transform.position;
        Debug.Log("Enemy prefab in Die: " + enemyPrefab?.name);
        if (respawner != null) respawner.ScheduleRespawn(enemyPrefab, spawnPosition);

        Destroy(GetComponentInChildren<EnemyHealthBar>()?.gameObject);
        Destroy(gameObject, 1f);
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

    // Abstract method: AI riêng từng loại quái
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

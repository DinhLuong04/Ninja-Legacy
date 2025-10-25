using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Flying AI")]
    public float moveSpeed = 2f;
    public float patrolRange = 3f;
    public float chaseRange = 5f;

    [Tooltip("Thời gian toàn bộ chu kỳ mổ (tính cả lùi về)")]
    public float peckCycleDuration = 1f;

    [Tooltip("Khoảng cách lùi lại sau khi mổ")]
    public float returnDistance = 0.5f;

    [Tooltip("Thời điểm gây damage (giây tính từ đầu chu kỳ)")]
    public float damageTime = 0.3f;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 targetPos;

    private bool isPecking = false;
    private float peckTimer = 0f;
    private bool hasDealtDamage = false;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        targetPos = startPos;

        animator?.SetBool("IsAttacking", false);
    }

    protected override void HandleAI()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (isPecking)
        {
            HandlePeckCycle();
            return;
        }

        // Nếu trong phạm vi đuổi
        if (distToPlayer <= chaseRange && distToPlayer > attackRange)
        {
            MoveTowards(player.position);
            animator?.SetBool("IsAttacking", false);
        }
        // Nếu trong phạm vi tấn công
        else if (distToPlayer <= attackRange)
        {
            if (rb != null) rb.velocity = Vector2.zero;

            if (Time.time > lastAttackTime + attackCooldown)
            {
                StartPeckCycle();
                lastAttackTime = Time.time;
            }
        }
        // Nếu ngoài phạm vi -> tuần tra
        else
        {
            Patrol();
            animator?.SetBool("IsAttacking", false);
        }
    }

    void Patrol()
    {
        // Khi đến gần vị trí tuần tra thì chọn vị trí mới
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            float randomX = Random.Range(-patrolRange, patrolRange);
            float randomY = Random.Range(-patrolRange, patrolRange);
            targetPos = startPos + new Vector2(randomX, randomY);
        }

        MoveTowards(targetPos);
    }

    void MoveTowards(Vector2 target)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        if (rb != null)
            rb.velocity = dir * moveSpeed;

        FlipSprite(dir.x);
    }

    void StartPeckCycle()
    {
        isPecking = true;
        hasDealtDamage = false;
        peckTimer = 0f;

        animator?.SetBool("IsAttacking", true);
    }

    void HandlePeckCycle()
    {
        peckTimer += Time.deltaTime;

        //  chuẩn bị & gây damage 
        if (peckTimer < damageTime)
        {
            if (rb != null) rb.velocity = Vector2.zero;
        }

        // 
        else if (!hasDealtDamage)
        {
            DealDamage();
            hasDealtDamage = true;
        }

        //  lùi lại ---
        if (peckTimer >= damageTime && peckTimer < peckCycleDuration)
        {
            Vector2 dirBack = ((Vector2)transform.position - (Vector2)player.position).normalized;
            Vector2 returnPos = startPos + dirBack * returnDistance;
            MoveTowards(returnPos);

            animator?.SetBool("IsAttacking", false);
        }

        //  kết thúc ---
        if (peckTimer >= peckCycleDuration)
        {
            isPecking = false;
            if (rb != null) rb.velocity = Vector2.zero;
            animator?.SetBool("IsAttacking", false);
        }
    }
}

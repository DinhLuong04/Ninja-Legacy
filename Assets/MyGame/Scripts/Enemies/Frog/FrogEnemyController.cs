using UnityEngine;

public class EnemyGround : Enemy
{
    [Header("AI Settings")]
    public float moveSpeed = 2f;
    public float patrolRange = 3f;
    public float chaseRange = 5f;
    public float minChaseDistance = 0.5f;
    public float maxHeightDifference = 0.5f;

    [Header("Ground Detection")]
    public BoxCollider2D frontCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.6f;
    public float wallCheckDistance = 0.3f;
    public float minYPosition = -10f;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private bool movingRight = true;
    private bool isPecking = false;
    private float peckTimer = 0f;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;

        if (animator != null)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsAttacking", false);
        }
    }

    protected override void HandleAI()
    {
        float distToPlayerX = Mathf.Abs(player.position.x - transform.position.x);
        float heightDifference = Mathf.Abs(player.position.y - transform.position.y);

        if (isDead || player == null) return;

        if (isPecking)
        {
            HandlePeckCycle();
            return;
        }

        // Chase player
        if (distToPlayerX <= chaseRange && IsPlayerInPatrolRange() &&
            distToPlayerX > attackRange && heightDifference <= maxHeightDifference)
        {
            ChasePlayer(distToPlayerX);
        }
        // Attack player
        else if (distToPlayerX <= attackRange && heightDifference <= maxHeightDifference)
        {
            if (rb != null) rb.velocity = Vector2.zero;
            if (animator != null) animator.SetBool("IsJumping", false);

            if (Time.time > lastAttackTime + attackCooldown)
            {
                if (animator != null) animator.SetBool("IsAttacking", true);
                lastAttackTime = Time.time;
                StartPeckCycle();
            }
        }
        // Patrol
        else
        {
            Patrol();
            if (animator != null)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsAttacking", false);
            }
        }

        // Reset IsAttacking sau cooldown
        if (Time.time > lastAttackTime + attackCooldown && animator != null && animator.GetBool("IsAttacking"))
            animator.SetBool("IsAttacking", false);

        // Giới hạn Y
        if (transform.position.y < minYPosition && rb != null)
            rb.velocity = new Vector2(rb.velocity.x, 0f);
    }

    void Patrol()
    {
        if (!IsGroundAhead())
        {
            movingRight = !movingRight;
            return;
        }

        if (animator != null)
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsAttacking", false);
        }

        float currentX = transform.position.x;
        if (rb != null)
        {
            rb.velocity = movingRight ? new Vector2(moveSpeed, rb.velocity.y) : new Vector2(-moveSpeed, rb.velocity.y);
        }

        if (currentX >= startPos.x + patrolRange) movingRight = false;
        if (currentX <= startPos.x - patrolRange) movingRight = true;

        FlipSprite(rb != null ? rb.velocity.x : 0);
    }

    void ChasePlayer(float distToPlayerX)
    {
        if (!IsGroundAhead())
        {
            Patrol();
            return;
        }

        if (animator != null)
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsAttacking", false);
        }

        float dir = player.position.x - transform.position.x;
        if (rb != null)
        {
            rb.velocity = distToPlayerX > minChaseDistance ? new Vector2(Mathf.Sign(dir) * moveSpeed, rb.velocity.y) : Vector2.zero;
        }

        float newX = Mathf.Clamp(transform.position.x, startPos.x - patrolRange, startPos.x + patrolRange);
        transform.position = new Vector2(newX, transform.position.y);

        FlipSprite(dir);
    }

    bool IsGroundAhead()
    {
        if (frontCheck == null) return true;

        Vector2 origin = frontCheck.bounds.center;
        Vector2 dir = movingRight ? Vector2.right : Vector2.left;

        RaycastHit2D groundHit = Physics2D.Raycast(origin + dir * 0.2f, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D wallHit = Physics2D.Raycast(origin, dir, wallCheckDistance, groundLayer);

        return groundHit && !wallHit;
    }

    bool IsPlayerInPatrolRange()
    {
        float playerDistX = Mathf.Abs(player.position.x - startPos.x);
        return playerDistX <= patrolRange;
    }

    void StartPeckCycle()
    {
        isPecking = true;
        peckTimer = 0f;
        if (animator != null) animator.SetBool("IsAttacking", true);
    }

    void HandlePeckCycle()
    {
        peckTimer += Time.deltaTime;

        if (peckTimer < 0.35f)
        {
            if (rb != null) rb.velocity = Vector2.zero;
            if (animator != null) animator.SetBool("IsAttacking", true);
            Debug.Log("Pecking at " + peckTimer + "s, Distance: " + Vector2.Distance(transform.position, player?.position ?? Vector2.zero));
        }
        else
        {
            isPecking = false;
            if (rb != null) rb.velocity = Vector2.zero;
            if (animator != null) animator.SetBool("IsAttacking", false);
            Debug.Log("Peck cycle ended at " + peckTimer + "s");
        }
    }
}
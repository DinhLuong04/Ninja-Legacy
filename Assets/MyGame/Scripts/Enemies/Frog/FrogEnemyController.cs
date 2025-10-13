using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float moveSpeed = 2f;
    public float patrolRange = 3f;
    public float chaseRange = 5f; // Khoảng cách chase theo trục X
    public float attackRange = 1f;
    public float attackCooldown = 1.5f;
    public float minChaseDistance = 0.5f; // Khoảng cách tối thiểu khi chase
    public float maxHeightDifference = 0.5f; // Ngưỡng chênh lệch chiều cao tối đa (trục Y)

    [Header("Ground Detection")]
    [SerializeField] private BoxCollider2D frontCheck; 
    [SerializeField] private LayerMask groundLayer;   // Layer chứa "ground", "one-way", và chướng ngại vật
    [SerializeField] private float minYPosition = -10f; // Ngưỡng tối thiểu trục Y để tránh rơi vực

    [Header("Obstacle Detection (Raycasts)")]
    [SerializeField] private float groundCheckDistance = 0.6f; // Khoảng cách raycast xuống
    [SerializeField] private float wallCheckDistance = 0.3f;   // Khoảng cách raycast ngang

    private Vector2 startPos;
    private bool movingRight = true;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;

    private float lastAttackTime = 0f;

    void Start()
    {
        startPos = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsAttacking", false);

        if (frontCheck == null)
        {
            Debug.LogError("FrontCheck BoxCollider2D không được gán trong Inspector!");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayerX = Mathf.Abs(player.position.x - transform.position.x);
        float heightDifference = Mathf.Abs(player.position.y - transform.position.y);

        // Chase player
        if (distToPlayerX <= chaseRange && IsPlayerInPatrolRange() && 
            distToPlayerX > attackRange && heightDifference <= maxHeightDifference)
        {
            ChasePlayer(distToPlayerX);
        }
        // Attack player
        else if (distToPlayerX <= attackRange && heightDifference <= maxHeightDifference)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsJumping", false);

            if (Time.time > lastAttackTime + attackCooldown)
            {
                animator.SetBool("IsAttacking", true);
                lastAttackTime = Time.time;
            }
        }
        // Patrol
        else
        {
            Patrol();
        }

        // Reset animation nếu không tấn công
        if (Time.time > lastAttackTime + attackCooldown && animator.GetBool("IsAttacking"))
        {
            animator.SetBool("IsAttacking", false);
        }

        // Giới hạn trục Y để tránh rơi quá sâu
        if (transform.position.y < minYPosition)
        {
            transform.position = new Vector2(transform.position.x, minYPosition);
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }

    // Check vực & tường
    bool IsGroundAhead()
    {
        Vector2 origin = frontCheck.bounds.center;
        Vector2 dir = movingRight ? Vector2.right : Vector2.left;

        // Raycast xuống (check vực)
        RaycastHit2D groundHit = Physics2D.Raycast(origin + dir * 0.2f, Vector2.down, groundCheckDistance, groundLayer);
        if (!groundHit) return false; // không có ground phía trước

        // Raycast ngang (check tường)
        RaycastHit2D wallHit = Physics2D.Raycast(origin, dir, wallCheckDistance, groundLayer);
        if (wallHit) return false; // có vật cản trước mặt

        return true;
    }

    void Patrol()
    {
        if (!IsGroundAhead())
        {
            movingRight = !movingRight; // quay đầu khi gặp vực hoặc vật cản
            return;
        }

        animator.SetBool("IsJumping", true);
        animator.SetBool("IsAttacking", false);

        float currentX = transform.position.x;
        if (movingRight)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            if (currentX >= startPos.x + patrolRange) movingRight = false;
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            if (currentX <= startPos.x - patrolRange) movingRight = true;
        }

        FlipSprite(rb.velocity.x);
    }

    void ChasePlayer(float distToPlayerX)
    {
        // Nếu gặp vực hoặc tường thì bỏ chase
        if (!IsGroundAhead())
        {
            Patrol();
            return;
        }

        animator.SetBool("IsJumping", true);
        animator.SetBool("IsAttacking", false);

        float dir = player.position.x - transform.position.x;

        // Chỉ di chuyển nếu chưa đủ gần (minChaseDistance)
        if (distToPlayerX > minChaseDistance)
        {
            rb.velocity = new Vector2(Mathf.Sign(dir) * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        // Giới hạn quái không ra khỏi vùng patrol
        float newX = Mathf.Clamp(transform.position.x, startPos.x - patrolRange, startPos.x + patrolRange);
        transform.position = new Vector2(newX, transform.position.y);

        FlipSprite(dir);
    }

    bool IsPlayerInPatrolRange()
    {
        float playerDistX = Mathf.Abs(player.position.x - startPos.x);
        return playerDistX <= patrolRange;
    }

    void FlipSprite(float dir)
    {
        if (dir > 0.1f && rb.velocity.x != 0) transform.localScale = new Vector3(1, 1, 1);
        else if (dir < -0.1f && rb.velocity.x != 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    // Gọi từ animation event
    public void DealDamage()
    {
        if (player == null) return;
        float distToPlayerX = Mathf.Abs(player.position.x - transform.position.x);
        float heightDifference = Mathf.Abs(player.position.y - transform.position.y);
        if (distToPlayerX <= attackRange + 0.3f && heightDifference <= maxHeightDifference)
        {
            PlayerStats ps = player.GetComponent<PlayerStats>();
            if (ps != null) ps.TakeDamage(10);
        }
    }

    // Debug để vẽ vùng kiểm tra
    void OnDrawGizmos()
    {
        if (frontCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(frontCheck.bounds.center, frontCheck.size);
        }

        if (frontCheck != null)
        {
            Vector2 origin = frontCheck.bounds.center;
            Vector2 dir = movingRight ? Vector2.right : Vector2.left;

            // Vẽ raycast xuống
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin + dir * 0.2f, origin + dir * 0.2f + Vector2.down * groundCheckDistance);

            // Vẽ raycast ngang
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(origin, origin + (Vector2)dir * wallCheckDistance);
        }
    }
}

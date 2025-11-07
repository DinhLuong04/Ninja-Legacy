using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Collider2D playerCollider;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    private float horizontalInput;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask oneWayLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private bool isGrounded;

    // Animator hash
    private int isRunningHash = Animator.StringToHash("isRunning");
    private int isJumpingHash = Animator.StringToHash("isJumping");
    private int isFallingHash = Animator.StringToHash("isFalling");

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        // Input di chuyển
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Flip sprite
        if (horizontalInput > 0) transform.localScale = new Vector2(1, transform.localScale.y);
        else if (horizontalInput < 0) transform.localScale = new Vector2(-1, transform.localScale.y);

        // Di chuyển ngang
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // Nhảy
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool(isJumpingHash, true);
        }

        // Animation chạy/idle
        bool isRunning = Mathf.Abs(horizontalInput) > 0 && isGrounded;
        animator.SetBool(isRunningHash, isRunning);
    }

    void FixedUpdate()
    {
        // Check ground cả Ground + OneWay
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer | oneWayLayer);

        // Animation nhảy/rơi
        if (rb.velocity.y > 0.1f && !isGrounded)
        {
            animator.SetBool(isJumpingHash, true);
            animator.SetBool(isFallingHash, false);
        }
        else if (rb.velocity.y < -0.1f && !isGrounded)
        {
            animator.SetBool(isJumpingHash, false);
            animator.SetBool(isFallingHash, true);
        }
        else if (isGrounded)
        {
            animator.SetBool(isJumpingHash, false);
            animator.SetBool(isFallingHash, false);
        }
        animator.SetBool("isGrounded", isGrounded);

        //  Xử lý đi ngang xuyên qua OneWay
        Collider2D[] oneWayHits = Physics2D.OverlapCircleAll(transform.position, 0.2f, oneWayLayer);
        foreach (Collider2D hit in oneWayHits)
        {
            if (hit != null)
            {
                if (Mathf.Abs(rb.velocity.y) < 0.05f) // đi ngang
                    Physics2D.IgnoreCollision(playerCollider, hit, true);
                else
                    Physics2D.IgnoreCollision(playerCollider, hit, false);
            }
        }
    }

    // Debug ground check
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

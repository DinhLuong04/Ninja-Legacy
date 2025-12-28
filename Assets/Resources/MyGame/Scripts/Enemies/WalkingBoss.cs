// WalkingBoss.cs - Boss CHỈ ĐI TRÊN ĐẤT, KHÔNG NHẢY
using UnityEngine;
using System.Collections;

public class WalkingBoss : Boss
{
    [Header("Walking Boss Settings")]
    public float detectWallDistance = 0.4f;
    public float detectGroundDistance = 0.4f;
    public float fallCheckDistance = 2f; // Kiểm tra vực sâu

    private BossSkillController skillController;

    protected override void Start()
    {
        base.Start();
        isBoss = true;
        skillController = GetComponent<BossSkillController>();
        skillController?.Init(this);
    }

    #region --- PATROL (Đi qua đi lại) ---
    protected override void Patrol()
    {
        if (!isGrounded) return;

        float dir = facingRight ? 1 : -1;
        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);

        // ĐỔi HƯỚNG khi gặp TƯỜNG hoặc VỰC
        if (IsWallAhead() || !IsGroundAhead())
        {
            Flip();
        }
    }
    #endregion

    #region --- CHASE (Đuổi theo ngang) ---
    protected override void ChasePlayer()
    {
        if (player == null || !isGrounded) return;

        Vector2 bossPos = transform.position;
        Vector2 playerPos = player.position;
        float horizontalDiff = playerPos.x - bossPos.x;

        //  HƯỚNG NHÌN
        if (horizontalDiff > 0.1f && !facingRight) Flip();
        if (horizontalDiff < -0.1f && facingRight) Flip();

        //  DI CHUYỂN NGANG
        if (Mathf.Abs(horizontalDiff) > safeXDistance)
        {
            float moveDir = Mathf.Sign(horizontalDiff);
            rb.velocity = new Vector2(moveDir * chaseSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // ĐỔI HƯỚNG khi gặp TƯỜNG hoặc VỰC (không nhảy)
        if (IsWallAhead() || !IsGroundAhead())
        {
            Flip();
        }
    }
    #endregion

    #region --- GROUND DETECTION ---
    protected override bool CheckGrounded()
    {
        if (groundCheck == null) return false;

        Vector2 size = new Vector2(0.8f, detectGroundDistance);
        bool onGround = Physics2D.OverlapBox(groundCheck.position, size, 0f, groundLayer);
        bool onOneWay = Physics2D.OverlapBox(groundCheck.position, size, 0f, oneWayLayer);
        return onGround || onOneWay;
    }

    private bool IsWallAhead()
    {
        if (wallCheck == null) return false;

        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        Vector2 size = new Vector2(detectWallDistance, 1.2f);
        return Physics2D.OverlapBox(wallCheck.position, size, 0f, groundLayer);
    }

    private bool IsGroundAhead()
    {
        if (groundCheck == null) return true;

        Vector2 forward = facingRight ? Vector2.right : Vector2.left;
        Vector2 checkPos = (Vector2)groundCheck.position + forward * 0.6f;
        Vector2 size = new Vector2(0.5f, detectGroundDistance + 0.2f);

        bool hitGround = Physics2D.OverlapBox(checkPos, size, 0f, groundLayer);
        bool hitOneWay = Physics2D.OverlapBox(checkPos, size, 0f, oneWayLayer);
        return hitGround || hitOneWay;
    }
    #endregion

        #region --- SKILLS (KIẾM BAY NGANG) ---
  public override IEnumerator UseBossSkill(int skillId)
{
    Debug.Log("[WalkingBoss] UseBossSkill START");
    
    rb.velocity = Vector2.zero;
    animator?.SetBool("IsAttacking", true);

    // Spawn kiếm
    Vector2 firePos = (Vector2)transform.position + Vector2.right * (facingRight ? 0.5f : -0.5f);
    SpawnSwordBeam(firePos);

    // Chờ animation
    yield return new WaitForSeconds(1.0f);

    //  UPDATE TIMER TRƯỚC KHI KẾT THÚC
    lastSkillTime = Time.time;
    
    animator?.SetBool("IsAttacking", false);
    
    Debug.Log("[WalkingBoss] UseBossSkill FINISHED");
}

    #endregion

    private void SpawnSwordBeam(Vector2 pos)
    {
        if (skillController == null || skillController.skillPrefabs.Length == 0) return;

        GameObject swordPrefab = skillController.skillPrefabs[0];
        if (swordPrefab == null) return;

        // Instantiate kiếm
        var sword = Instantiate(swordPrefab, pos, Quaternion.identity);
        sword.SetActive(true);

        // HƯỚNG KIẾM (theo facing)
        float direction = facingRight ? 1f : -1f;
        Vector2 velocity = new Vector2(direction * 20f, 0f); 
        sword.transform.localScale = new Vector3(direction, 1f, 1f);
        var rbSword = sword.GetComponent<Rigidbody2D>();
        if (rbSword != null)
        {
            rbSword.velocity = velocity;
            rbSword.gravityScale = 0f; 
        }

        var hit = sword.GetComponent<BossSkillHit>();
        if (hit != null)
        {
            hit.Init(this);
        }
        Destroy(sword, 2.5f);
    }

   #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // === GROUND CHECK ===
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Vector2 groundSize = new Vector2(0.8f, detectGroundDistance);
            Gizmos.DrawWireCube(groundCheck.position, groundSize);
        }

        // === WALL CHECK ===
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Vector2 dir = facingRight ? Vector2.right : Vector2.left;
            Vector2 wallSize = new Vector2(detectWallDistance, 1.2f);
            Gizmos.DrawWireCube(wallCheck.position, wallSize);
        }

        // === GROUND AHEAD CHECK ===
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 forward = facingRight ? Vector2.right : Vector2.left;
            Vector2 aheadPos = (Vector2)groundCheck.position + forward * 0.6f;
            Vector2 aheadSize = new Vector2(0.5f, detectGroundDistance + 0.2f);
            Gizmos.DrawWireCube(aheadPos, aheadSize);
        }

        // === CHASE & ATTACK RANGE ===
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxChaseRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif 
}
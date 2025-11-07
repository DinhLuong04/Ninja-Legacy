// GroundBoss.cs
using UnityEngine;
using System.Collections;

public class GroundBoss : Boss
{
    [Header("Ground Boss Settings")]
    public float jumpForce = 8f;
    public float verticalFollowThreshold = 0.8f;
    public float detectWallDistance = 0.4f;
    public float detectGroundDistance = 0.4f;
    public float stepHeightMax = 1.5f;
    public float jumpCooldown = 0.6f;

    private BossSkillController skillController;

    protected override void Start()
    {
        base.Start();
        isBoss = true;

        skillController = GetComponent<BossSkillController>();
        skillController?.Init(this);
    }

    protected override void Patrol()
    {
        if (!isGrounded) return;

        float dir = facingRight ? 1 : -1;
        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);

        if (IsWallAhead() && !CanJumpToStep() || !IsGroundAhead())
        {
            Flip();
        }
    }

    protected override void ChasePlayer()
    {
        if (player == null) return;

        Vector2 bossPos = transform.position;
        Vector2 playerPos = player.position;
        float horizontalDiff = playerPos.x - bossPos.x;
        float verticalDiff = playerPos.y - bossPos.y;

        // Hướng nhìn
        if (horizontalDiff > 0.1f && !facingRight) Flip();
        if (horizontalDiff < -0.1f && facingRight) Flip();

        // Di chuyển ngang
        if (Mathf.Abs(horizontalDiff) > safeXDistance)
        {
            float moveDir = Mathf.Sign(horizontalDiff);
            float speed = isGrounded ? chaseSpeed : rb.velocity.x;
            rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // Nhảy / rơi / leo bậc
        if (verticalDiff > verticalFollowThreshold && isGrounded)
        {
            TryJumpToHigherLevel();
        }
        else if (verticalDiff < -verticalFollowThreshold && isGrounded)
        {
            TryDropToLowerLevel();
        }
        else if (isGrounded && (IsWallAhead() || !IsGroundAhead()))
        {
            if (CanJumpToStep())
            {
                TryJumpOverObstacle();
            }
            else
            {
                Flip();
            }
        }
    }

    #region --- JUMP & DETECTION ---

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

    private bool CanJumpToStep()
    {
        if (!IsWallAhead()) return false;
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        Vector2 wallPos = (Vector2)wallCheck.position + dir * detectWallDistance;
        Vector2 topCheckPos = wallPos + Vector2.up * stepHeightMax;
        bool hasSpace = !Physics2D.OverlapBox(topCheckPos, new Vector2(0.4f, 0.1f), 0f, groundLayer);
        Vector2 groundCheckPos = topCheckPos + dir * 0.4f;
        bool hasGround = Physics2D.OverlapBox(groundCheckPos, new Vector2(0.6f, 0.1f), 0f, groundLayer | oneWayLayer);
        return hasSpace && hasGround;
    }

    private void TryJumpOverObstacle()
    {
        if (Time.time < lastJumpTime + jumpCooldown) return;
        lastJumpTime = Time.time;
        float dir = facingRight ? 1f : -1f;
        rb.velocity = new Vector2(dir * chaseSpeed * 0.7f, jumpForce);
        animator?.SetBool("IsJumping", true);
        Invoke(nameof(EndJump), 0.4f);
    }

    private void TryJumpToHigherLevel()
    {
        if (Time.time < lastJumpTime + jumpCooldown) return;
        lastJumpTime = Time.time;
        float dir = facingRight ? 1f : -1f;
        rb.velocity = new Vector2(dir * 2f, jumpForce * 1.1f);
        animator?.SetBool("IsJumping", true);
        Invoke(nameof(EndJump), 0.5f);
    }

    private void TryDropToLowerLevel()
    {
        if (Time.time < lastJumpTime + jumpCooldown) return;
        if (player.position.y >= transform.position.y - verticalFollowThreshold) return;

        Collider2D oneWayBelow = Physics2D.OverlapBox(
            groundCheck.position, new Vector2(0.8f, 0.1f), 0f, oneWayLayer);

        if (oneWayBelow != null)
        {
            StartCoroutine(TemporarilyIgnoreOneWay(gameObject.layer, LayerMask.NameToLayer("One-way"), 0.5f));
            rb.velocity = new Vector2(rb.velocity.x, -jumpForce * 0.6f);
            lastJumpTime = Time.time;
        }
    }

    private void EndJump()
    {
        animator?.SetBool("IsJumping", false);
    }

    private IEnumerator TemporarilyIgnoreOneWay(int bossLayer, int oneWayLayerIdx, float duration)
    {
        Physics2D.IgnoreLayerCollision(bossLayer, oneWayLayerIdx, true);
        yield return new WaitForSeconds(duration);
        Physics2D.IgnoreLayerCollision(bossLayer, oneWayLayerIdx, false);
    }

    #endregion

    #region --- SKILL ---

    public override IEnumerator UseBossSkill(int skillId)
    {
        rb.velocity = Vector2.zero;
        animator?.SetBool("IsAttacking", false);

        switch (skillId)
        {
            case 0: // Liên Hoàn Chưởng
                Vector2 firePos = transform.position;
                for (int i = 0; i < 8; i++)
                {
                    SpawnProjectile(skillController.skillPrefabs[0], firePos, 14f, i * 45f);
                    yield return new WaitForSeconds(0.06f);
                }
                break;

            case 1: // Vòi Rồng dưới chân player
                {
                    Vector2 spawnPos = playerGroundCheck.position;
                    var tornado = Instantiate(skillController.skillPrefabs[1], spawnPos, Quaternion.identity, transform);
                    tornado.SetActive(true);

                    var hit = tornado.GetComponent<BossSkillHit>();
                    if (hit != null) hit.Init(this);

                    Destroy(tornado, 2.5f);
                    yield return new WaitForSeconds(0.4f);
                }
                break;
        }

        yield return new WaitForSeconds(0.6f);
    }

    private void SpawnProjectile(GameObject prefab, Vector2 pos, float speed, float angle)
    {
        var obj = Instantiate(prefab, pos, Quaternion.Euler(0, 0, angle));
        obj.SetActive(true);

        var rbProj = obj.GetComponent<Rigidbody2D>();
        if (rbProj && speed > 0)
        {
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            rbProj.velocity = dir * speed;
        }

        var hit = obj.GetComponent<BossSkillHit>();
        if (hit != null) hit.Init(this);

        Destroy(obj, 3f);
    }

    #endregion
}
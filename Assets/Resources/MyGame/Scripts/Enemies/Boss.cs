// Boss.cs
using UnityEngine;
using System.Collections;

public abstract class Boss : Enemy
{
    [Header("Boss Common Settings")]
    public float moveSpeed = 3f;
    public float chaseSpeed = 5f;
    public float maxChaseRange = 10f;
    public float safeXDistance = 1.5f;

    [Header("Skill Settings")]
    public float skillCooldown = 6f;

    [Header("References")]
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public LayerMask oneWayLayer;
    public Transform playerGroundCheck;

    protected Rigidbody2D rb;
    protected bool isGrounded;
    protected bool facingRight = true;
    protected float lastJumpTime = -10f;
    protected float lastSkillTime = -10f;
    private float lastTryCastTime = 0f;

    public enum BossState { Idle, Patrol, Chase, Attack, Casting }
    public BossState currentState = BossState.Idle;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected override void HandleAI()
    {
        if (isDead || player == null || currentState == BossState.Casting) return;
        if (!CanDetectPlayer())
        {
            Patrol();
            return;
        }
        UpdateGrounded();
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // Xác định trạng thái
        if (distToPlayer <= attackRange)
            currentState = BossState.Attack;
        else if (distToPlayer <= maxChaseRange)
            currentState = BossState.Chase;
        else
            currentState = BossState.Patrol;

        // Reset animation
        if (currentState != BossState.Attack)
            animator?.SetBool("IsAttacking", false);

        // Thực thi hành vi
        switch (currentState)
        {
            case BossState.Patrol: Patrol(); break;
            case BossState.Chase: ChasePlayer(); break;
            case BossState.Attack: AttackPlayer(); break;
        }

        TryStartCasting();
    }

    protected virtual void UpdateGrounded()
    {
        isGrounded = CheckGrounded();
    }

    protected virtual void Patrol()
    {
        // Override nếu cần
    }

    protected virtual void ChasePlayer()
    {
        // Override nếu cần
    }

    protected virtual void AttackPlayer()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator?.SetBool("IsAttacking", true);

        if (Time.time > lastAttackTime + attackCooldown)
        {
            DealDamage();
            lastAttackTime = Time.time;
        }
    }

 protected virtual void TryStartCasting()
{
    if (Time.time < lastTryCastTime + 1f) return;
    lastTryCastTime = Time.time;

    if (currentState != BossState.Chase && currentState != BossState.Attack) return;
    if (Time.time < lastSkillTime + skillCooldown) return;
    
    currentState = BossState.Casting; // ← DI CHUYỂN LÊN TRƯỚC
    lastSkillTime = Time.time;        // ← UPDATE NGAY!
    
    Debug.Log("[TryStartCasting] → CASTING! Trigger CastSkill");
    animator?.SetTrigger("CastSkill");
}

    public void OnCastSkillEvent()
    {
        if (isDead) return;
        StartCoroutine(DoCastSkillOnce());
    }

    private IEnumerator DoCastSkillOnce()
{
    var skillCtrl = GetComponent<BossSkillController>();
    if (skillCtrl == null) yield break;

    int skillId = skillCtrl.GetAvailableSkillId();
    if (skillId == -1) yield break;

    Debug.Log("[DoCastSkillOnce] START - SkillId: " + skillId);

    lastSkillTime = Time.time;
    skillCtrl.MarkSkillUsed(skillId);

    yield return UseBossSkill(skillId);

    Debug.Log("[DoCastSkillOnce] SKILL FINISHED - Reset to Chase");

    yield return new WaitForSeconds(0.4f);
    currentState = BossState.Chase; // ← ĐẢM BẢO VỀ CHASE
}

    // === ABSTRACT & VIRTUAL METHODS ===
    protected abstract bool CheckGrounded();
    protected virtual void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public override IEnumerator UseBossSkill(int skillId)
    {
        yield return null; // Override ở con
    }
}
using UnityEngine;
using System.Collections;

public class FlyingBoss : FlyingEnemy
{
    private BossSkillController skillController;
    private bool isUsingSkill = false;

    [Header("Flying Boss Settings")]
    public float stopDistance = 4f;          // Dừng ở cách player bao xa
    public float skillCooldown = 3f;         // Mỗi bao lâu thì bắn 1 lần

    private float lastSkillTime = 0f;

    protected override void Start()
    {
        base.Start();
        isBoss = true;

        skillController = GetComponent<BossSkillController>();
        if (skillController == null)
            skillController = gameObject.AddComponent<BossSkillController>();

        skillController.Init(this);
    }

    protected override void HandleAI()
    {
        if (isPecking || isUsingSkill) return;
          if (!CanDetectPlayer())
        {
            Patrol();
            return;
        }
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // Nếu ngoài tầm chase -> tuần tra
        if (distToPlayer > chaseRange)
        {
            Patrol();
            animator?.SetBool("IsAttacking", false);
            return;
        }

        // Nếu trong tầm chase nhưng chưa đủ gần -> bay lại gần
        if (distToPlayer > stopDistance)
        {
            MoveTowards(player.position);
            animator?.SetBool("IsAttacking", false);
        }
        else // Nếu đủ gần -> dừng lại và bắn skill
        {
            if (rb != null) rb.velocity = Vector2.zero;

            // Hướng nhìn
            FlipSprite(player.position.x - transform.position.x);

            // Dùng skill định kỳ
            if (Time.time > lastSkillTime + skillCooldown)
            {
                int randomSkill = 0;
                // Random.Range(0, skillController.skillPrefabs.Length);
                StartCoroutine(UseBossSkill(randomSkill));
                lastSkillTime = Time.time;
            }
        }
    }

    public override IEnumerator UseBossSkill(int skillId)
    {
        isUsingSkill = true;
        animator?.SetBool("IsAttacking", true);

        rb.velocity = Vector2.zero;

        switch (skillId)
        {
            case 0: // 🔥 Khạc lửa thẳng về hướng player
                Vector2 dir = (player.position - transform.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                SpawnProjectile(skillController.skillPrefabs[0], transform.position, 10f, angle);
                break;

            case 1: // 🌧️ Mưa lửa rơi từ trên đầu player
                for (int i = 0; i < 5; i++)
                {
                    Vector2 pos = player.position + new Vector3(Random.Range(-3f, 3f), 6f);
                    var fire = Instantiate(skillController.skillPrefabs[1], pos, Quaternion.identity);
                    fire.SetActive(true);
                    yield return new WaitForSeconds(0.2f);
                }
                break;

            case 2: // 🌪️ Xoáy gió quanh player
                Vector2 center = player.position;
                for (int i = 0; i < 6; i++)
                {
                    SpawnProjectile(skillController.skillPrefabs[2], center, 6f, i * 60f);
                    yield return new WaitForSeconds(0.1f);
                }
                break;
        }

        yield return new WaitForSeconds(0.5f);

        animator?.SetBool("IsAttacking", false);
        isUsingSkill = false;
    }

    private void SpawnProjectile(GameObject prefab, Vector2 pos, float speed, float angle)
    {
        if (prefab == null) return;

        var obj = Instantiate(prefab, pos, Quaternion.Euler(0, 0, angle));
        obj.SetActive(true);

        var rbProj = obj.GetComponent<Rigidbody2D>();
        if (rbProj)
        {
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            rbProj.velocity = dir * speed;
        }
        var hit = obj.GetComponent<BossSkillHit>();
        if (hit != null)
            hit.Init(this);

    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRange); // 🟢 Vùng tuần tra

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chaseRange);  // 🔵 Vùng đuổi theo

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stopDistance); // 🟣 Vùng dừng & bắn skill

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);  // 🔴 Vùng tấn công trực tiếp (nếu có)

        // Vẽ hướng nhìn / hướng bắn
        if (Application.isPlaying && player != null)
        {
            Gizmos.color = Color.yellow;
            Vector2 dir = (player.position - transform.position).normalized;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)dir * stopDistance);
        }
    }
#endif

}

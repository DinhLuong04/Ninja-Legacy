using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private WeaponManager weaponManager;

    private bool isAttacking = false;
    private bool isInputLocked = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isInputLocked)
        {
            WeaponData weapon = weaponManager.GetCurrentWeaponData();
            if (weapon == null) return;

            if (weapon.projectilePrefab == null)
            {
                AttackMelee();
            }
            else
            {
                AttackRanged();
            }
        }
    }

    void AttackMelee()
    {
        isAttacking = true;
        isInputLocked = true;
        playerAnimator.SetBool("isAttacking", true);
      
    }

    void AttackRanged()
    {
        isAttacking = true;
        isInputLocked = true;
        playerAnimator.SetBool("isThrowing", true);
     
    }

    private System.Collections.IEnumerator LockInputForAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
        isInputLocked = false;
        isAttacking = false;
        playerAnimator.SetBool("isAttacking", false);
        playerAnimator.SetBool("isThrowing", false);
    }

    public void EnableSlashCollider()
    {
        if (slashEffectPrefab == null || attackPoint == null) return;

        for (int i = 0; i < 3; i++)
        {
            float offsetY = (i - 1) * 0.5f;
            Vector3 pos = attackPoint.position + new Vector3(0f, offsetY, 0f);

            GameObject slash = Instantiate(slashEffectPrefab, pos, Quaternion.identity, attackPoint);

            if (i == 1)
            {
                slash.transform.localScale = new Vector3(1.5f, 1f, 1f);
                Destroy(slash, 0.7f);
            }
            else
            {
                slash.transform.localScale = new Vector3(1f, 1f, 1f);
                Destroy(slash, 0.5f);
            }
        }
    }

    public void DisableSlashCollider()
    {
        playerAnimator.SetBool("isAttacking", false);
        isAttacking = false;
        isInputLocked = false;
    }

    public void ThrowShuriken()
    {
        WeaponData weapon = weaponManager.GetCurrentWeaponData();
        if (weapon == null || weapon.projectilePrefab == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - attackPoint.position).normalized;

        GameObject proj = Instantiate(weapon.projectilePrefab, attackPoint.position, Quaternion.identity);
        Shuriken s = proj.GetComponent<Shuriken>();
        if (s != null) s.Launch(dir, weapon.projectileSpeed, weapon.damage);
    }

    public void EndThrow()
    {
        playerAnimator.SetBool("isThrowing", false);
        isAttacking = false;
        isInputLocked = false;
    }
}
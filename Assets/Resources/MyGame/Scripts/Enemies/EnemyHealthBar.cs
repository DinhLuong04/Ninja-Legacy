using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Enemy enemy;        
    public Image healthFill;   

    void Update()
    {
        if (enemy != null && healthFill != null)
        {
            healthFill.fillAmount = (float)enemy.GetCurrentHP() / enemy.maxHP;
        }

        // Đặt trên đầu quái
        transform.position = enemy.transform.position + Vector3.up * 1f;
    }
}

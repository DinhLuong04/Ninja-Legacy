using System.Collections;
using UnityEngine;

public class EnemyRespawner : MonoBehaviour
{
    public float respawnTime = 5f;

    
    public void ScheduleRespawn(GameObject enemyPrefab, Vector3 spawnPosition)
    {
        Debug.Log("Scheduling respawn with prefab: " + enemyPrefab?.name);
        StartCoroutine(RespawnAfterDelay(enemyPrefab, spawnPosition));
    }

    private IEnumerator RespawnAfterDelay(GameObject enemyPrefab, Vector3 spawnPosition)
{
    yield return new WaitForSeconds(respawnTime);
    Debug.Log("Received prefab: " + enemyPrefab?.name);
    if (enemyPrefab != null)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        newEnemy.name = enemyPrefab.name; 

        // Gán lại enemyPrefab cho script Enemy của đối tượng mới
        Enemy enemyScript = newEnemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.enemyPrefab = enemyPrefab; 
        }
    }
    else
    {
        Debug.LogError("Enemy prefab chưa gán khi respawn!");
    }
}
}

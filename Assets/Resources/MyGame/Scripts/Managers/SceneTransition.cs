// Assets/Scripts/SceneTransition.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("=== CÀI ĐẶT CHUYỂN MAP ===")]
    public string targetScene = "Level2";           // Tên scene đích
    public string spawnKey = "FromLevel1";          // Key để nhận diện spawn point
    public Vector2 spawnPosition = Vector2.zero;    // Vị trí player sẽ spawn

    [Header("=== TỰ ĐỘNG ===")]
    public bool autoTransition = true;              // Tự động khi vào vùng
    public float transitionDelay = 0.3f;            // Delay nhỏ tránh lỗi

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isTransitioning) return;

        if (autoTransition)
        {
            isTransitioning = true;
            Invoke(nameof(DoTransition), transitionDelay);
        }
    }

    private void DoTransition()
    {
        PlayerSpawnManager.SetNextSpawn(targetScene, spawnKey, spawnPosition);
        SceneManager.LoadScene(targetScene);
    }

    // Hiển thị trong Scene view
    private void OnDrawGizmosSelected()
    {
        if (string.IsNullOrEmpty(targetScene)) return;

        Gizmos.color = new Color(0, 1, 1, 0.8f);
        Gizmos.DrawWireCube(spawnPosition, Vector3.one * 1.8f);
        Gizmos.DrawLine(transform.position, spawnPosition);

#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.Label(spawnPosition + Vector2.up * 2f,
            $"→ {targetScene}\n[{spawnKey}]");
#endif
    }
}
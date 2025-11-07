using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    public Transform parent;

    void Start()
    {
        parent = transform.parent;
        if (parent == null)
        {
            Debug.LogWarning($"[KeepUpright] {name} has no parent!");
        }
    }

    void LateUpdate()
    {
        if (parent == null) return;

        if (parent.localScale.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);  // giữ hướng âm
            transform.localScale = scale;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);  // luôn giữ hướng dương
            transform.localScale = scale;

            // Nếu bạn dùng Canvas World Space thì có thể thêm dòng này để reset rotation
            // transform.rotation = Quaternion.identity;
        }
    }
}

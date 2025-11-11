using UnityEngine;
using UnityEngine.UI;

public class CloudPingPongUI : MonoBehaviour
{
    public float speed = 50f;         
    public float moveDistance = 500f; 
    private RectTransform rectTransform;
    private Vector2 startPos;
    private int direction = 1;        

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // di chuyển cloud
        rectTransform.anchoredPosition += Vector2.right * speed * Time.deltaTime * direction;

        // nếu đi quá khoảng moveDistance thì đổi hướng
        if (rectTransform.anchoredPosition.x >= startPos.x + moveDistance)
            direction = -1;
        else if (rectTransform.anchoredPosition.x <= startPos.x - moveDistance)
            direction = 1;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class BackgroundFloatUI : MonoBehaviour
{
    public float speed = 50f;          
    public Vector2 direction = Vector2.down; 
    private RectTransform rectTransform;
    private float width;
    private float height;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
    }

    void Update()
    {
        // di chuyển theo hướng
        rectTransform.anchoredPosition += direction.normalized * speed * Time.deltaTime;

        // reset vị trí nếu trôi ra ngoài (loop)
        if (direction == Vector2.down && rectTransform.anchoredPosition.y <= -height)
        {
            rectTransform.anchoredPosition += new Vector2(0, height * 2);
        }
        else if (direction == Vector2.up && rectTransform.anchoredPosition.y >= height)
        {
            rectTransform.anchoredPosition += new Vector2(0, -height * 2);
        }
        else if (direction == Vector2.right && rectTransform.anchoredPosition.x >= width)
        {
            rectTransform.anchoredPosition += new Vector2(-width * 2, 0);
        }
        else if (direction == Vector2.left && rectTransform.anchoredPosition.x <= -width)
        {
            rectTransform.anchoredPosition += new Vector2(width * 2, 0);
        }
    }
}

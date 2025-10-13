using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform player;
    public float parallaxEffect = 0.1f; // Tỷ lệ di chuyển (0.1 nghĩa là chậm hơn 10%)
    public Transform cameraTransform; // Gán Main Camera hoặc Virtual Camera

    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(delta.x * parallaxEffect, delta.y * parallaxEffect, 0);
        lastCameraPosition = cameraTransform.position;
    }
}
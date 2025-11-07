using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudPingPong : MonoBehaviour
{
    [SerializeField] private float moveDistance = 3f;   // khoảng cách bay qua lại
    [SerializeField] private float minSpeed = 0.5f;     
    [SerializeField] private float maxSpeed = 2f;       
    [SerializeField] private bool randomDirection = true; 
    [SerializeField] private bool randomStartOffset = true; 

    private Vector3 startPos;
    private float speed;
    private int direction;
    private float startOffset;

    void Start()
    {
        startPos = transform.position;

        
        speed = Random.Range(minSpeed, maxSpeed);

        // Random hướng
        direction = randomDirection && Random.value > 0.5f ? -1 : 1;

        // Random lệch thời gian ban đầu
        startOffset = randomStartOffset ? Random.Range(0f, 10f) : 0f;
    }

    void Update()
    {
        float offset = Mathf.PingPong((Time.time + startOffset) * speed, moveDistance);
        transform.position = startPos + new Vector3(offset * direction, 0, 0);
    }

}

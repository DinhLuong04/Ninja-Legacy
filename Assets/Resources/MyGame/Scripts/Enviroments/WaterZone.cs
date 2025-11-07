using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterZone : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            
            bool isMoving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0;

            if (isMoving)
            {
                // Giữ player ở mặt nước
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.gravityScale = 0;
            }
            else
            {
                // Cho rơi xuống
                rb.gravityScale = 2.5f; 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.gravityScale = 2.5f; 
        }
    }
}

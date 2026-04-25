using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            collision.transform.position = CheckpointManager.instance.respawnPoint;
        }
    }
}
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float travelSpeed = 7f;

    private Rigidbody2D projectileBody;

    private void Awake()
    {
        projectileBody = GetComponent<Rigidbody2D>();

        if (projectileBody != null)
        {
            projectileBody.bodyType = RigidbodyType2D.Kinematic;
            projectileBody.gravityScale = 0f;
            projectileBody.linearVelocity = transform.right * travelSpeed;
        }

        Collider2D projectileCollider = GetComponent<Collider2D>();
        if (projectileCollider != null)
        {
            projectileCollider.isTrigger = true;
        }
    }

    private void Update()
    {
        if (projectileBody == null)
        {
            return;
        }

        projectileBody.linearVelocity = transform.right * travelSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            // TODO: award points to player
        }

        if (collision.CompareTag("Box")) {
            Destroy(gameObject);
        }

        if (collision.GetComponent<TilemapCollider2D>() != null)
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;

    public Transform pointA;
    public Transform pointB;

    private Transform currentPoint;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;

    public float detectionRange = 3f;

    // control de daño
    private float damageCooldown = 1f;
    private float lastDamageTime = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        currentPoint = pointB;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        if (anim != null)
            anim.SetBool("isMoving", true);
    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        DetectPlayer();
    }

    void Move()
    {
        if (pointA == null || pointB == null) return;

        // direccion hacia el objetivo
        Vector2 direction = (currentPoint.position - transform.position).normalized;

        // movimiento con fisica (unity 6 usa linearVelocity)
        rb.linearVelocity = new Vector2(direction.x * speed, 0);

        // cuando llega al punto cambia destino
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.1f)
        {
            if (currentPoint == pointA)
                currentPoint = pointB;
            else
                currentPoint = pointA;

            Flip();
        }
    }

    void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (anim != null)
            anim.SetBool("isAttacking", distance < detectionRange);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                GameControl.Instance.SpendLives();
                lastDamageTime = Time.time;
            }
        }
    }
}
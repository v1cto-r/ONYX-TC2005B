using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float detectionRange = 2f;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private Transform player;
    private Animator anim;

    // daño
    private float damageCooldown = 0.5f;
    private float lastDamageTime;

    // ataque
    private bool isAttackingNow = false;
    private float attackDuration = 0.4f;
    private float attackTimer = 0f;

    private float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    void FixedUpdate()
    {
        Move();
        CheckGround();
    }

    void Update()
    {
        DetectPlayer();
    }

    void Move()
    {
        if (isAttackingNow)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float moveDir = (movingRight ? 1 : -1);
        rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y);

        if (anim != null)
            anim.SetBool("isMoving", true);
    }

    void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);

        if (hit.collider == null)
        {
            Flip();
        }
    }

    void DetectPlayer()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // iniciar ataque
        if (dist < detectionRange &&
            Time.time >= lastAttackTime + attackCooldown &&
            !isAttackingNow)
        {
            isAttackingNow = true;
            attackTimer = attackDuration;
            lastAttackTime = Time.time;
        }

        // controlar duración del ataque
        if (isAttackingNow)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0)
            {
                isAttackingNow = false;
            }
        }

        if (anim != null)
        {
            anim.SetBool("isAttacking", isAttackingNow);
            anim.SetBool("isMoving", !isAttackingNow);
        }
    }

    void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // SOLO hace daño cuando está atacando
            if (isAttackingNow && Time.time >= lastDamageTime + damageCooldown)
            {
                GameControl.Instance.SpendLives();
                lastDamageTime = Time.time;
            }
        }
    }
}
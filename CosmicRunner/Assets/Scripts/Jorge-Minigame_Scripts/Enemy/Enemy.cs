using UnityEngine;

public class Enemy : MonoBehaviour
{
    // velocidad de movimiento
    public float speed = 2f;

    // deteccion de suelo para no caerse
    public Transform groundCheck;
    public LayerMask groundLayer;

    // distancia para detectar al jugador
    public float detectionRange = 2f;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private Transform player;
    private Animator anim;

    // control de daño
    private float damageCooldown = 0.5f;
    private float lastDamageTime;

    // control de ataque
    private bool isAttackingNow = false;
    private float attackDuration = 0.4f;
    private float attackTimer = 0f;

    private float attackCooldown = 1.5f;
    private float lastAttackTime = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // busca al jugador en la escena
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
        // si esta atacando no se mueve
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
        // usa raycast para detectar si hay suelo
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

        // inicia ataque si el jugador esta cerca
        if (dist < detectionRange &&
            Time.time >= lastAttackTime + attackCooldown &&
            !isAttackingNow)
        {
            isAttackingNow = true;
            attackTimer = attackDuration;
            lastAttackTime = Time.time;
        }

        // controla la duracion del ataque
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
        // cambia de direccion
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // hace daño al jugador si esta atacando
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isAttackingNow && Time.time >= lastDamageTime + damageCooldown)
            {
                GameControl.Instance.SpendLives();
                lastDamageTime = Time.time;
            }
        }
    }
}
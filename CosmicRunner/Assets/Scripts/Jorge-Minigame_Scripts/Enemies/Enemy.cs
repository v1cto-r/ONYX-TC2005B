using UnityEngine;

namespace JorgeGame
{
public class Enemy : MonoBehaviour
{
    // velocidad de movimiento
    public float speed = 2f;

    // deteccion de suelo para no caerse
    public Transform groundCheck;
    public LayerMask groundLayer;

    // distancia para detectar al jugador
    public float detectionRange = 2f;

    // distancia real para poder atacar
    public float attackRange = 1f;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private Transform player;
    private Animator anim;

    // control de ataque
    private bool isAttackingNow = false;
    private float attackDuration = 0.8f;
    private float attackTimer = 0f;

    private float attackCooldown = 1f;
    private float lastAttackTime = -999f;

    // indica si ya golpeo durante este ataque (para no pegar varias veces)
    private bool hasHit = false;

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

    // mueve al enemigo en la direccion actual mientras no ataca
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

    // detecta si hay suelo delante para evitar caerse
    void CheckGround()
    {
        // usa raycast para detectar si hay suelo
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);

        if (hit.collider == null)
        {
            Flip();
        }
    }

    // controla deteccion, ataque con cooldown y dano unico por ataque
    void DetectPlayer()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // inicia ataque si el jugador esta dentro del rango
        if (dist < attackRange &&
            Time.time >= lastAttackTime + attackCooldown &&
            !isAttackingNow)
        {
            isAttackingNow = true;
            attackTimer = attackDuration;
            lastAttackTime = Time.time;

            // reinicia el golpe para este ataque
            hasHit = false;
        }

        // controla la duracion del ataque
        if (isAttackingNow)
        {
            attackTimer -= Time.deltaTime;

            // hace daño UNA sola vez en medio del ataque
            // esto evita que el daño sea constante o random
            if (!hasHit && attackTimer <= attackDuration * 0.2f)
            {
                // solo hace daño si el jugador sigue dentro del rango
                if (dist < attackRange)
                {
                    GameControl.Instance.SpendLives();

                    // desparentar para evitar bugs con plataformas
                    player.SetParent(null);

                    // regresar al ultimo checkpoint
                    player.position = SpawnPoint.instance.respawnPoint + Vector3.up * 1f;
                }

                // marca que ya golpeo para no repetir el daño
                hasHit = true;
            }

            // termina el ataque
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

    // invierte direccion de movimiento y orientacion visual
    void Flip()
    {
        // cambia de direccion
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
}
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // acciones de movimiento y salto
    private InputAction moveAction;
    private InputAction jumpAction;
    
    // velocidad y fuerza de salto
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D rb;

    // input del jugador
    private Vector2 moveInput;

    // controla cuando se aplica el salto (para hacerlo en fixedupdate)
    private bool jumpQueue;    

    // indica si el jugador esta tocando el suelo
    private bool isGrounded = false;

    private SpriteRenderer spriteRenderer;
    private Animator animatorController;

    void Start()
    {
        // obtiene componentes principales del jugador
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animatorController = GetComponent<Animator>();
        
        // obtiene las acciones del input system
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        // si el juego esta pausado no procesa input
        if (Time.timeScale == 0f)
        {
            jumpQueue = false;
            return;
        }

        // lee el input de movimiento
        moveInput = moveAction.ReadValue<Vector2>();

        // voltea el sprite segun la direccion horizontal
        if (moveInput.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
        
        // si presiona salto y esta en el suelo, guarda la orden
        if (jumpAction.triggered && isGrounded)
        {
            jumpQueue = true;
        }

        // actualiza las animaciones segun el estado
        UpdatePlayerAnimation();
    }

    void FixedUpdate()
    {
        // aplica movimiento horizontal usando fisicas
        Vector2 moveVelocity = moveInput * moveSpeed;
        rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);

        // ejecuta el salto en fisicas para mayor precision
        if (jumpQueue && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }

        jumpQueue = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // detecta cuando toca el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            // se hace hijo de la plataforma para moverse junto a ella
            StartCoroutine(SetParentNextFrame(collision.transform));
        }
    }

    IEnumerator SetParentNextFrame(Transform newParent)
    {
        // espera un frame para evitar errores de fisicas
        yield return null;

        transform.SetParent(newParent);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // cuando deja de tocar el suelo se separa de la plataforma
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(RemoveParentNextFrame());
            }
        }
    }

    IEnumerator RemoveParentNextFrame()
    {
        yield return null;

        if (gameObject.activeInHierarchy)
        {
            transform.SetParent(null);
        }
    }

    // estados posibles de animacion
    public enum PlayerAnimation
    {
        Idle, Run, Jump
    }

    void UpdatePlayerAnimation()
    {
        // decide que animacion usar segun el estado del jugador
        if (!isGrounded)
        {
            UpdatePlayerAnimation(PlayerAnimation.Jump);
        }
        else if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            UpdatePlayerAnimation(PlayerAnimation.Run);
        }
        else
        {
            UpdatePlayerAnimation(PlayerAnimation.Idle);
        }
    }

    void UpdatePlayerAnimation(PlayerAnimation nameAnimation)
    {
        // cambia los parametros del animator
        switch(nameAnimation)
        {
            case PlayerAnimation.Idle:
                animatorController.SetBool("isMoving", false);
                animatorController.SetBool("isJumping", false);
                break;
            case PlayerAnimation.Run:
                animatorController.SetBool("isMoving", true);
                animatorController.SetBool("isJumping", false);
                break;
            case PlayerAnimation.Jump:
                animatorController.SetBool("isMoving", false);
                animatorController.SetBool("isJumping", true);
                break;
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // Referencias a las aciones de entrada del jugador
    private InputAction moveAction;
    private InputAction jumpAction;
    
    // Velocidad de movimiento del jugador
    [SerializeField] private float moveSpeed = 5f;
   
    // Fuerza de salto del jugador
    [SerializeField] private float jumpForce = 10f;

    // Referencia al Rigidbody2D del jugador
    private Rigidbody2D rb;

    // Variable para almacenar el valor de movimiento del jugador
    private Vector2 moveInput;

    // Variable para controlar el llamado de salto del jugador
    private bool jumpQueue;    

    // Escala inicial del jugador para controlar la direccion del sprite
    private Vector3 startScale;

    // Variable para verificar si el jugador esta en el suelo
    private bool isGrounded = false;

    // Referencia al SpriteRenderer del jugador
    private SpriteRenderer spriteRenderer;

    // Control de animador
    private Animator animatorController;

    // Control de sonidos

    
    // Start es llamado antes del primer frame update
    void Start()
    {
        // Obtener la referencia al Rigidbody2D del jugador
        rb = GetComponent<Rigidbody2D>();

        // Leer la escala del jugador por defecto 
        startScale = transform.localScale;
        
        // Habilitar las acciones de entrada
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        // Obtener la referencia al SpriteRenderer del jugador
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Obtener la referencia al Animator del jugador
        animatorController = GetComponent<Animator>();

        // Obtener la referencia al SFXManager
       
    }

    // Update es llamado una vez por frame
    void Update()
    {
        // Verificar si el juego esta pausado o si el input de juego esta bloqueado para evitar errores
        bool blocked = Time.timeScale == 0f;
        if (blocked)
        {
            jumpQueue = false;
            return;
        }

        // Obtener el valor de movimiento del jugador
        moveInput = moveAction.ReadValue<Vector2>();

        // Direcionar el "sprite" del jugador segun la direccion del movimiento
        if (moveInput.x > 0.01f) // Derecha
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < -0.01f) // Izquierda
        {
            spriteRenderer.flipX = true;
        }
        
        // Verificar si se ha presionado el botón de salto
        if (jumpAction.triggered && isGrounded)
        {
            jumpQueue = true;
        }

        // Actualizar la animacion del jugador segun su estado
        UpdatePlayerAnimation();
    }

    void FixedUpdate()
    {
        // Calcular la velocidad de movimiento del jugador
        Vector2 moveVelocity = moveInput * moveSpeed;
        
        // Aplicar la velocidad de movimiento al Rigidbody2D del jugador
        rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocity.y);

        // Verificar si el jugador ha solicitado un salto y esta en el suelo
        if (jumpQueue && isGrounded)
        {
            // Aplicar una fuerza de salto al Rigidbody2D del jugador
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;

            // Reproducir el sonido de salto
  
        }

        jumpQueue = false;
    }

    // Metodo para detectar colisiones con el suelo
void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Ground"))
    {
        isGrounded = true;

        // esperar un frame antes de hacer parent
        StartCoroutine(SetParentNextFrame(collision.transform));
    }
}

IEnumerator SetParentNextFrame(Transform newParent)
{
    yield return null; // espera 1 frame
    transform.SetParent(newParent);
}

void OnCollisionExit2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Ground"))
    {
        StartCoroutine(RemoveParentNextFrame());
    }
}

IEnumerator RemoveParentNextFrame()
{
    yield return null;
    transform.SetParent(null);
}

    // Enumeracion para los estados de animacion del jugador
    public enum PlayerAnimation
    {
        Idle, Run, Jump
    }

    // Metodo para actualizar la animacion del jugador segun su estado
    void UpdatePlayerAnimation()
    {
        // Determinar la animacion del jugador segun su estado (saltando, corriendo o idle)
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

    // Metodo para actualizar los parametros del animador segun la animacion del jugador
    void UpdatePlayerAnimation(PlayerAnimation nameAnimation)
    {
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
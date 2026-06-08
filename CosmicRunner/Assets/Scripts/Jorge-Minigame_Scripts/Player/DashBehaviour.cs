using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JorgeGame
{
public class DashBehaviour : MonoBehaviour
{
    // accion de dash del input system
    private InputAction dashAction;
    private InputAction moveAction;
    
    // referencia al rigidbody del jugador
    private Rigidbody2D rb;

    // configuracion del dash
    [SerializeField] private float dashSpeed = 6f;
    [SerializeField] private float dashDuration = 0.12f;
    
    // direccion del dash
    private Vector2 dashDirection;
    
    // control del estado del dash
    private bool isDashing = false;
    private bool canDash = true;
    
    // efecto visual del dash
    private TrailRenderer trailRenderer;
    
    // guarda la gravedad original
    private float originalGravityScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        
        // obtiene la accion de dash y la accion de movimiento
        dashAction = InputSystem.actions.FindAction("Sprint");
        moveAction = InputSystem.actions.FindAction("Move");

        if (dashAction == null)
        {
            Debug.LogError("Sprint action was not found in Input System Actions.", this);
        }

        if (moveAction == null)
        {
            Debug.LogError("Move action was not found in Input System Actions.", this);
        }
        
        // guarda la gravedad original
        originalGravityScale = rb.gravityScale;
    }

    private void OnEnable()
    {
        if (dashAction == null)
        {
            dashAction = InputSystem.actions.FindAction("Sprint");
        }

        if (moveAction == null)
        {
            moveAction = InputSystem.actions.FindAction("Move");
        }

        dashAction?.Enable();
        moveAction?.Enable();
    }

    private void OnDisable()
    {
        dashAction?.Disable();
        moveAction?.Disable();
    }

    void Update()
    {
        // evita usar dash cuando el juego esta pausado
        if (Time.timeScale == 0) return;

        // activa el dash si se presiona y esta disponible
        if (dashAction != null && dashAction.WasPressedThisFrame() && canDash)
        {
            isDashing = true;
            canDash = false;
            trailRenderer.emitting = true;

            // obtiene direccion de movimiento
            Vector2 moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

            // dash hacia arriba con ligera direccion lateral
            dashDirection = new Vector2(moveInput.x * 0.2f, 1f).normalized;

            rb.gravityScale = 0f;

            StartCoroutine(StopDash());
        }

        // aplica la velocidad del dash
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            return;
        }
    }

    // corrutina que termina el dash y restaura la gravedad
    private IEnumerator StopDash()
    {
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        trailRenderer.emitting = false;

        // restaura la gravedad
        rb.gravityScale = originalGravityScale;
    }

    // reactiva el dash cuando vuelve a tocar el suelo
    void OnCollisionEnter2D(Collision2D collision)
    {
        // permite volver a usar dash al tocar el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            canDash = true;
        }
    }
}
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace Nicte.Minigame{
// Controla el movimiento vertical del jugador
public class PlayerMovement : MonoBehaviour
{
    // Rigidbody y limites de movimiento en pantalla
    public Rigidbody2D rig;
    float moveSpeed = 6f;
    public float minY = -3.70f;    
    public float maxY = 2.54f;  
    // Entrada vertical actual y estado de habilitacion
    private float yInput;
    public bool inputEnabled = true;

    // Input actions para movimiento, se asignan desde PlayerControl
    private InputAction moveAction;
    


    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Lee las teclas de direccion cada frame
    void Update()
    {
        yInput = 0f;

        if (!inputEnabled)
        {
            return;
        }

        if (moveAction != null)
        {
            Vector2 inputVector = moveAction.ReadValue<Vector2>();
            yInput = inputVector.y;
        }
        else
        {
            // Fallback a teclado si no se asigno InputAction
            if (Keyboard.current.upArrowKey.isPressed)
            {
                yInput = 1f;
            }
            else if (Keyboard.current.downArrowKey.isPressed)
            {
                yInput = -1f;
            }
        }
    }

    // Aplica el movimiento fisico en FixedUpdate
    void FixedUpdate()
    {
        float currentY = rig.position.y;

        if (!inputEnabled)
        {
            rig.linearVelocity = Vector2.zero;
            return;
        }

        if ((yInput > 0 && currentY<maxY) ||
            (yInput < 0 && currentY > minY))
        {
            rig.linearVelocity = new Vector2(0f, yInput * moveSpeed);
        }
        else
        {
            rig.linearVelocity = new Vector2(0f, 0f);
        }
    }

    // Habilita o bloquea el control del jugador
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        if (!enabled)
        {
            rig.linearVelocity = Vector2.zero;
        }
    }
}
}
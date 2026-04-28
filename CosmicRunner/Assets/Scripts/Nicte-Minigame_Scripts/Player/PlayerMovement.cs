using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rig;
    public float moveSpeed = 4f;
    public float minY = -3.70f;    
    public float maxY = 2.54f;  
    private float yInput;
    public bool inputEnabled = true;

    void Update()
    {
        yInput = 0f;

        if (Keyboard.current.upArrowKey.isPressed)
        {
            yInput = 1f;
        }
        else if (Keyboard.current.downArrowKey.isPressed)
        {
            yInput = -1f;
        }
    }

    void FixedUpdate()
    {
        float currentY = rig.position.y;

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

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        if (!enabled)
            rig.linearVelocity = Vector2.zero;
    }
}
using UnityEngine;

public class PlayerMenuAnimations : MonoBehaviour
{
    // Componentes de animacion del jugador
    private Animator animator;
    
    private void Awake()
    {
        // Buscamos el Animator del jugador para cambiar animaciones
        animator = GetComponent<Animator>();

        PlayIdleAnimation();
    }

    // Metodo para activar la animacion de seleccion
    public void PlayIdleAnimation()
    {
        animator.SetTrigger("idle_down");
    }
}

using UnityEngine;

namespace Nicte.Minigame{
// Coordina el estado general y la reaccion del enemigo
public class EnemyController : MonoBehaviour
{
    // Referencias a los sistemas de juego, ataque y salud
    GameController gameController;
    EnemyMovement enemyMovement;
    EnemyAttack attack;
    EnemyHealth health;
    
    // Estados y umbrales de comportamiento
    public string currentState = "Normal";
    public int aggressiveThreshold = 6;
    public int enragedThreshold = 2;
    public int currentHealth = 12;

    // Prepara las referencias y guarda la vida inicial
    void Awake()
    {
        gameController = FindObjectOfType<GameController>();

        PlayerPrefs.SetInt("EnemyHealth", currentHealth);

        enemyMovement = GetComponent<EnemyMovement>();
        attack= GetComponent<EnemyAttack>();
        health= GetComponent<EnemyHealth>();
    }

    // Coloca al enemigo en estado normal al iniciar
    void Start()
    {
        ChangeState("Normal");
    }

    // Notifica a ataque y movimiento cuando cambia el estado
    public void ChangeState(string newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        
        if (attack != null)
        {
             attack.StateChanged(newState);
        }

        if (enemyMovement != null)
        { 
            enemyMovement.StateChanged(newState);
        }
    }

    // Decide el estado segun la salud restante
    public void EvaluateStateFromHealth(float healthRatio)
    {
        if (healthRatio <= enragedThreshold)
        {
            ChangeState("Enraged");
            health.Heal();
            Debug.Log("Enraged");
        }
        else if (healthRatio <= aggressiveThreshold)
        {
            ChangeState("Aggressive");
            health.Heal();
            Debug.Log("Aggressive");
        }
        else
        {
            ChangeState("Normal");
            Debug.Log("Normal");
        }
    }
}
}
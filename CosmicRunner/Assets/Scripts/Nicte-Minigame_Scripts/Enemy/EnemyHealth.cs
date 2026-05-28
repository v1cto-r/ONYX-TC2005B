using UnityEngine;

namespace Nicte.Minigame{
// Gestiona la salud del enemigo y su victoria o derrota
public class EnemyHealth : MonoBehaviour
{
    // Interfaz de vida y controlador del enemigo
    HealthBarUI healthBarUI;
    EnemyController enemyController;
    // Vida maxima y estado de vida actual
    public int maxHealth = 12;
    public bool isAlive = true;
    private int currentHealth;

    // Inicializa la salud y referencias visibles
    void Start()
    {
        currentHealth = maxHealth;
        enemyController = GetComponent<EnemyController>();
        PlayerPrefs.SetInt("EnemyHealth", currentHealth);
        healthBarUI = FindObjectOfType<HealthBarUI>();
    }
    // Recibe daño y actualiza la barra de vida
    public void TakeDamage(int damage)
    {
        if (!isAlive) return;
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (healthBarUI != null)
        {
            healthBarUI.updateEnemyHealth(currentHealth);
            Debug.Log("Enemy Health: " + currentHealth);
        }

        if (enemyController != null)
        {
            // Pasar la salud actual directamente al controlador (evita depender de PlayerPrefs)
            enemyController.EvaluateStateFromHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            isAlive = false;
            Die();
        }
        
    }

    // Finaliza la partida cuando el enemigo muere
    void Die()
    {
        if (isAlive) return;

        isAlive = false;

        if (GameController.instancia != null){
            GameController.instancia.ChangeState("Victory");
        }
    }

    // Regenera parte de la vida segun una probabilidad aleatoria
    public void Heal()
    {
        int randomValue = Random.Range(0, 10);
            if (randomValue < 3)
            {
                currentHealth += 2;
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                if (healthBarUI != null)
                {
                    healthBarUI.regenerateEnemyHealth(currentHealth, 0);
                }
            }
    }
}
}
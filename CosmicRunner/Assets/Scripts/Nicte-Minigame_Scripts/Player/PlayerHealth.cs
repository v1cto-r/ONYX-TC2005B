using UnityEngine;
using Nicte.Minigame;

namespace Nicte.Minigame{
// Gestiona la vida, escudo y muerte del jugador
public class PlayerHealth : MonoBehaviour
{
    // Referencias a control y UI de vida
    PlayerController playerController;
    HealthBarUI healthBarUI;
    // Efecto visual del escudo y datos de salud
    public GameObject shieldEffect;
    public int maxHealth = 12;
    public float shieldTimerMax = 5f;
    public bool isAlive = true;
    public bool shieldActive = false;
    public int currentHealth;

    // Inicializa la vida y guarda el valor persistente
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        healthBarUI = FindObjectOfType<HealthBarUI>();
        currentHealth = maxHealth;
        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
    }

    // Reduce el temporizador del escudo mientras esta activo
    void Update()
    {
        if (shieldActive)
        {
            shieldTimerMax -= Time.deltaTime;
            if (shieldTimerMax <= 0f)
            {
                shieldActive = false;
                shieldEffect.SetActive(false);
                shieldTimerMax = 5f;
            }
        }
    }

    // Resta vida si el jugador no esta protegido
    public void TakeDamage(int damage)
    {
        if (!isAlive || damage <= 0|| shieldActive) return;
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth =0;
        }

        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        Debug.Log("Player health after taking damage: " + currentHealth);
        if (healthBarUI != null)
        {
            healthBarUI.updatePlayerHealth(currentHealth);
        }
 
        if (currentHealth <= 0)
        {
            Debug.Log("Player health is zero or less, player is dead.");
            Die();
        }   
    }

    // Desactiva el jugador y cambia el estado del juego a derrota
    void Die()
    {
        Debug.Log("Llegue a la funcion DIE");
        if (!isAlive) return;

        isAlive = false;

        if (playerController != null)
            playerController.DisableInput();

        if (GameController.instancia != null)
            GameController.instancia.ChangeState("Defeat");
    }

    // Activa el escudo temporal del jugador
    public void ActivateShield()
    {
        Debug.Log("Shield activated");
        shieldActive = true;
        if (shieldEffect != null)
        {
            shieldEffect.SetActive(true);
        }
        shieldTimerMax = 5f;
    }

    // Recupera vida hasta el maximo permitido
    public void Heal()
    {
        currentHealth += 2;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (healthBarUI != null)
        {
            healthBarUI.regeneratePlayerHealth(currentHealth, 0);
        }
    }
}
}

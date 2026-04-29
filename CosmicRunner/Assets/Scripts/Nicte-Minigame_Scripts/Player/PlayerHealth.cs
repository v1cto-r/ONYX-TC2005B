using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    PlayerController playerController;
    HealthBarUI healthBarUI;
    public GameObject shieldEffect;
    public int maxHealth = 12;
    public float shielTimerMax = 5f;
    public bool isAlive = true;
    public bool shieldActive = false;
    public int currentHealth;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        healthBarUI = FindObjectOfType<HealthBarUI>();
        currentHealth = maxHealth;
        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
    }

    void Update()
    {
        if (shieldActive)
        {
            shielTimerMax -= Time.deltaTime;
            if (shielTimerMax <= 0f)
            {
                shieldActive = false;
                shieldEffect.SetActive(false);
                shielTimerMax = 5f;
            }
        }
    }

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

        if (playerController != null)
        {
            playerController.currentHealth = currentHealth;
        }   
        if (currentHealth <= 0)
        {
            Debug.Log("Player health is zero or less, player is dead.");
            Die();
        }   
    }

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

    public void ActivateShield()
    {
        Debug.Log("Shield activated");
        shieldActive = true;
        if (shieldEffect != null)
        {
            shieldEffect.SetActive(true);
        }
        shielTimerMax = 5f;
    }

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

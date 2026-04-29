using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    PlayerController playerController;
    HealthBarUI healthBarUI;
    public int maxHealth = 12;
    public float shielTimerMax = 3f;
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
                shielTimerMax = 3f;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive || damage <= 0) return;
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth =0;
        }

        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        if (healthBarUI != null)
        {
            Debug.Log("Updating player health: " + currentHealth + " - " + damage);
            healthBarUI.updatePlayerHealth(currentHealth, damage);
        }

        if (playerController != null)
        {
            playerController.currentHealth = currentHealth;
        }   
        if (currentHealth <= 0)
        {
            isAlive = false;
            Die();
        }   
    }

    

    void Die()
    {
        if (!isAlive) return;

        isAlive = false;

        if (playerController != null)
            playerController.DisableInput();

        if (GameController.instancia != null)
            GameController.instancia.ChangeState("Defeat");
    }

}

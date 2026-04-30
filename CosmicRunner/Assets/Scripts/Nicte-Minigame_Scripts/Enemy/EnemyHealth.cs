using UnityEngine;

namespace Nicte.Minigame{
public class EnemyHealth : MonoBehaviour
{
    HealthBarUI healthBarUI;
    EnemyController enemyController;
    public int maxHealth = 12;
    public bool isAlive = true;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        enemyController = GetComponent<EnemyController>();
        PlayerPrefs.SetInt("EnemyHealth", currentHealth);
        healthBarUI = FindObjectOfType<HealthBarUI>();
    }
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
        }

        if (enemyController != null)
        {
            enemyController.EvaluateStateFromHealth(PlayerPrefs.GetInt("EnemyHealth"));
        }

        if (currentHealth <= 0)
        {
            isAlive = false;
            Die();
        }
        
    }

    void Die()
    {
        if (isAlive) return;

        isAlive = false;

        if (GameController.instancia != null){
            GameController.instancia.ChangeState("Victory");
        }
    }

    public void Heal()
    {
        int randomValue = Random.Range(0, 10);
            if (randomValue < 5)
            {
                currentHealth += 3;
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
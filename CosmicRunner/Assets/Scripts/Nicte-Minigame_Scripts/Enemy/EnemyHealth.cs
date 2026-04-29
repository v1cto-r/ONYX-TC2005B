using UnityEngine;

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
        PlayerPrefs.SetInt("EnemyHealth", currentHealth);
        healthBarUI.updateEnemyHealth(currentHealth, damage);

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
}

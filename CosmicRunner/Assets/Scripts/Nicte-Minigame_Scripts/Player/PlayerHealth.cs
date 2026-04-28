using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    PlayerController playerController;
    HealthBarUI healthBarUI;
    public int maxHealth = 12;
    public float shielTimerMax = 3f;
    public bool isAlive = true;
    public bool shieldActive = false;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
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
        if (shieldActive || !isAlive) return;
        healthBarUI.updatePlayerHealth(PlayerPrefs.GetInt("PlayerHealth"), damage);
        if (PlayerPrefs.GetInt("PlayerHealth") <= 0)
        {
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

using UnityEngine;

public class EnemyController : MonoBehaviour
{
    GameController gameController;
    EnemyMovement enemyMovement;
    EnemyAttack attack;
    EnemyHealth health;
    EnemyPowerUp powerUp;
    public string currentState = "Normal";
    public int aggressiveThreshold = 6;
    public int enragedThreshold = 2;
    public int currentHealth = 12;

    void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        PlayerPrefs.SetInt("EnemyHealth", currentHealth);
        enemyMovement = GetComponent<EnemyMovement>();
        attack= GetComponent<EnemyAttack>();
        health= GetComponent<EnemyHealth>();
        powerUp = GetComponent<EnemyPowerUp>();
    }

    void Start()
    {
        ChangeState("Normal");
    }

    public void ChangeState(string newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        if (attack != null) attack.StateChanged(newState);
        if (enemyMovement != null) enemyMovement.StateChanged(newState);
    }

    public void EvaluateStateFromHealth(float healthRatio)
    {
        if (healthRatio <= enragedThreshold)
        {
            ChangeState("Enraged");
            Debug.Log("Enraged");
        }
        else if (healthRatio <= aggressiveThreshold)
        {
            ChangeState("Aggressive");
            Debug.Log("Aggressive");
        }
        else
        {
            ChangeState("Normal");
            Debug.Log("Normal");
        }
    }
}

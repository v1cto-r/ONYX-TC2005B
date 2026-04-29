using UnityEngine;
using System.Collections;

public class EnemyPowerUp : MonoBehaviour
{
    EnemyHealth health;
    public bool enableRegeneration = true;
    public float regenAmountNormal= 5f;
    public float regenAmountAggressive = 2f;
    public float regenTickInterval=3f;
    string currentState = "Normal";
    Coroutine regenCoroutine;

    void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }

    public void StateChanged(string state)
    {
        currentState = state;

        // Detener la regeneración anterior
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        if (state == "Normal" && enableRegeneration)
        {
            regenCoroutine = StartCoroutine(Regenerate(regenAmountNormal));
        }
        else if (state == "Aggressive" && enableRegeneration)
        {
            regenCoroutine = StartCoroutine(Regenerate(regenAmountAggressive));
        }
    }

    IEnumerator Regenerate(float amount)
    {
        while (health.isAlive)
        {
            yield return new WaitForSeconds(regenTickInterval);
            //health.Heal(Mathf.RoundToInt(amount));
        }
    }
}

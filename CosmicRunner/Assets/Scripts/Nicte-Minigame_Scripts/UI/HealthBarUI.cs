using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image[] playerLivesImage;
    public Image[] enemyLivesImage;

    public void updatePlayerHealth(int currentHealth, int damage)
    {
        if(currentHealth>=0 && currentHealth < playerLivesImage.Length)
        {
            for (int i = 0; i < playerLivesImage.Length; i++)
            {
                if (i < currentHealth)
                {
                    playerLivesImage[i].enabled = true;
                }
                else
                {
                    playerLivesImage[i].enabled = false;
                }
            }
        }
    }

    public void regeneratePlayerHealth(int currentHealth, int regenAmount)
    {
        Debug.Log("Regenerating player health: " + currentHealth + " + " + regenAmount);
        int newHealth = currentHealth + regenAmount;
        if(newHealth > playerLivesImage.Length)
        {
            newHealth = playerLivesImage.Length;
        }
        if (newHealth < 0)
        {
            newHealth = 0;
        }
        for (int i = 0; i < playerLivesImage.Length; i++)
        {
            playerLivesImage[i].enabled = (i < newHealth);
        }
    }

    public void regenerateEnemyHealth(int currentHealth, int regenAmount)
    {
        Debug.Log("Regenerating enemy health: " + currentHealth + " + " + regenAmount);
        int newHealth = currentHealth + regenAmount;
        if(newHealth > enemyLivesImage.Length)
        {
            newHealth = enemyLivesImage.Length;
        }
        if (newHealth < 0)
        {
            newHealth = 0;
        }
        for (int i = 0; i < enemyLivesImage.Length; i++)
        {
            enemyLivesImage[i].enabled = (i < newHealth);
        }
    }

    public void updateEnemyHealth(int currentHealth, int damage)
    {
        Debug.Log("Updating enemy health: " + currentHealth + " - " + damage);
        if(currentHealth>=0 && currentHealth < enemyLivesImage.Length)
        {
            for (int i = 0; i < enemyLivesImage.Length; i++)
            {
                if (i < currentHealth)
                {
                    enemyLivesImage[i].enabled = true;
                }
                else
                {
                    enemyLivesImage[i].enabled = false;
                }
            }
        }
    }


    
}

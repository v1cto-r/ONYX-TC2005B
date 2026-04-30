using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image[] playerLivesImage;
    public Image[] enemyLivesImage;

    public void updatePlayerHealth(int currentHealth)
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
        int newHealth = currentHealth + regenAmount;
        PlayerPrefs.SetInt("PlayerHealth", newHealth);

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
        int newHealth = currentHealth + regenAmount;
        PlayerPrefs.SetInt("EnemyHealth", newHealth);

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

    public void updateEnemyHealth(int currentHealth)
    {
        if(currentHealth>enemyLivesImage.Length)
        {
            currentHealth = enemyLivesImage.Length;
        }

        if(currentHealth<0)
        {
            currentHealth = 0;
        }

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
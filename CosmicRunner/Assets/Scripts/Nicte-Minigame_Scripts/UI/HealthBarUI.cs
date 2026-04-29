using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Image[] playerLivesImage;
    public Image[] enemyLivesImage;
    int playerLives = 12;
    int enemyLives = 12;

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

    public void updateEnemyHealth(int currentHealth, int damage)
    {
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

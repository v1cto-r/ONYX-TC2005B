using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SpecialAttackUI : MonoBehaviour
{
    public Image[] SpecialAttackImages;
    public int currentCharge = 0;
    float timer = 0f;


     void Awake()
    {
        resetAttack();
    }

    void Update()
    {
        if(currentCharge < SpecialAttackImages.Length)
        {
            timer+= Time.deltaTime;
            if (timer >= 3f)
            {
                UpdateCharge(currentCharge+1);
                timer = 0f;
            }
        }
    }
    public void resetAttack()
    {
        currentCharge = 0;
        for (int i = 0; i < SpecialAttackImages.Length; i++)
            {
                SpecialAttackImages[i].enabled = false;
            }
    }

    public void UpdateCharge(int charge)
    {
        currentCharge = charge;
        updateSpecialBar(currentCharge);
    }

    public void updateSpecialBar(int charge)
    {
        if(charge>=0 && charge <= SpecialAttackImages.Length)
        {
            for (int i = 0; i < SpecialAttackImages.Length; i++)
            {
                if (i < charge)
                {
                    SpecialAttackImages[i].enabled = true;
                }
                else
                {
                    SpecialAttackImages[i].enabled = false;
                }
            }
        }

    }

    public bool CanUseSpecialAttack()
    {
        return currentCharge >= SpecialAttackImages.Length;
    }
}

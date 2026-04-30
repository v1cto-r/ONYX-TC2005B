using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace Nicte.Minigame{
public class PlayerController : MonoBehaviour
{
   GameController gameController;
   PlayerMovement playermovement;
   SpecialAttackUI specialAttackUI;
   SpriteRenderer spriteRenderer;
   string colorCooldown="#FFA3A3";
   //public int currentHealth = 12;
   bool inputEnabled = true;
   bool isDead = false;
   float attackWindow = 1f;
   float lastAttackTime = -Mathf.Infinity;
   int attackCount = 0;
   int maxAttacks = 5;
   float cooldownTime = 2f;
   bool isOnCooldown = false;

   void Awake()
   {
       playermovement = GetComponent<PlayerMovement>();
       specialAttackUI = FindObjectOfType<SpecialAttackUI>();
       spriteRenderer = GetComponent<SpriteRenderer>();
       gameController = FindObjectOfType<GameController>();
   }
   
   void Update()
   {
       if (!isDead && gameController != null)
            inputEnabled = (gameController.currentState == "Playing");

        if (playermovement != null)
        {
            playermovement.SetInputEnabled(inputEnabled && !isDead);
        }

        if (!inputEnabled) {
            return;
        }

        if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            PlayerCombat combat = GetComponent<PlayerCombat>();
            if (combat != null && !isOnCooldown)
            {
                float currentTime = Time.time;
                if (currentTime-lastAttackTime <= attackWindow)
                {
                    attackCount ++;
                }
                else
                {
                    attackCount = 1;
                }

                lastAttackTime = currentTime;
                
                combat.Shoot();
                if (attackCount >= maxAttacks)
                {
                    StartCoroutine(AttackCooldown());
                }
            }
        }

        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PlayerCombat combat = GetComponent<PlayerCombat>();
            if (combat != null)
            {
                if (specialAttackUI.CanUseSpecialAttack()==true)
                {
                    combat.SpecialAttack();
                    specialAttackUI.resetAttack();
                }
            }
        }
   }

   public void DisableInput()
    {
        isDead = true;
        inputEnabled = false;
        if (playermovement != null)
            playermovement.SetInputEnabled(false);
    }

    IEnumerator AttackCooldown()
   {
       isOnCooldown = true;
       if (ColorUtility.TryParseHtmlString(colorCooldown, out Color cooldownColor))
       {
        spriteRenderer.color = cooldownColor;
    } 

       yield return new WaitForSeconds(cooldownTime);

       attackCount = 0;
       isOnCooldown = false;
       spriteRenderer.color = Color.white;
   }
}
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
   GameController gameController;
   PlayerMovement playermovement;
   PlayerCombat combat;
   public int currentHealth = 12;
   bool inputEnabled = true;
   bool isDead = false;
   void Awake()
   {
       playermovement = GetComponent<PlayerMovement>();
       combat=GetComponent<PlayerCombat>();
       PlayerPrefs.SetInt("PlayerHealth", currentHealth);
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

        if(Keyboard.current.zKey.wasPressedThisFrame)
        {
            PlayerCombat combat = GetComponent<PlayerCombat>();
            if (combat != null)
            {
                combat.shoot();
            }
        }

        if(Keyboard.current.xKey.wasPressedThisFrame)
        {
            PlayerCombat combat = GetComponent<PlayerCombat>();
            if (combat != null)
            {
                //combat.specialShoot();
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

}

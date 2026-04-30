using UnityEngine;

namespace Nicte.Minigame{
public class PowerUp : MonoBehaviour
{
    float powerUpSpeed = 8f;
    float deadZoneX = 10f;
    string[] powerUpTypes = {"Shield", "Heal", "SpecialRecharge"};

     void Update()
    {
        transform.position+= Vector3.right*Time.deltaTime*powerUpSpeed;
        if (transform.position.x >= deadZoneX)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            SFXManager.Instance.PowerUpSound();
            int randomIndex = Random.Range(0, powerUpTypes.Length);
            string selectedPowerUp = powerUpTypes[randomIndex];
            switch (selectedPowerUp)
            {
                case "Shield":
                    collision.gameObject.GetComponent<PlayerHealth>().ActivateShield();
                    break;
                case "Heal":
                    collision.gameObject.GetComponent<PlayerHealth>().Heal();
                    break;
                case "SpecialRecharge":
                    collision.gameObject.GetComponent<PlayerCombat>().RechargeSpecial();
                    break;
            }
        }
    }
}
}
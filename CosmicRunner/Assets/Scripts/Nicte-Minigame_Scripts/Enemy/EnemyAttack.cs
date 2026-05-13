using UnityEngine;

namespace Nicte.Minigame{
// Maneja los disparos del enemigo segun su estado
public class EnemyAttack : MonoBehaviour
{
    // Prefabs de balas normales y especiales
    public GameObject[] bullets;
    public GameObject specialBullet;
    public Transform firePoint;
    // Tiempos de disparo por estado y municion especial
    private float normalCooldown = 1.5f;
    private float aggressiveCooldown = 1f;
    private float enragedCooldown= 0.6f;
    public int   specialBulletCount = 8;
    string currentState = "Normal";
    float shootTimer=0.3f;
    float currentCooldown;
    bool canAttack = true;
    // Arranca con la cadencia de disparo normal
    void Start()
    {
        currentCooldown = normalCooldown;
    }

    // Dispara de forma continua mientras el ataque siga habilitado
    void Update()
    {
        if (!canAttack) return;

        shootTimer-= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = currentCooldown;
        }

    }
    // Elige una bala aleatoria del arreglo disponible
    GameObject GetBullet()
    {
        if (bullets == null || bullets.Length == 0) return null;
        int index = Random.Range(0, bullets.Length);
        return bullets[index];
    }

    // Instancia la bala correspondiente al estado actual
    void Shoot()
    {
        if(currentState == "Enraged" && specialBulletCount > 0)
        {
            Instantiate(specialBullet, firePoint.position, Quaternion.identity);
            specialBulletCount--;
            return;
        }else {
        GameObject selectedBullet = GetBullet();
        if (selectedBullet == null) return;
        Instantiate(selectedBullet, firePoint.position, Quaternion.identity);
        }
    }

    // Ajusta la cadencia y municion segun el nuevo estado
    public void StateChanged(string state)
    {
        currentState = state;

        if (state == "Normal")         
        {
             currentCooldown = normalCooldown; 
        } else if (state == "Aggressive") 
        { 
            currentCooldown = aggressiveCooldown; 
            }
            else if (state == "Enraged") 
            { 
                currentCooldown = enragedCooldown; 
                }
    }
}
}
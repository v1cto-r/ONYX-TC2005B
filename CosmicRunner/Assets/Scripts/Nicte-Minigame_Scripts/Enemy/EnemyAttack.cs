using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject[] bullets;
    public GameObject specialBullet;
    public Transform firePoint;
    private float normalCooldown = 1.5f;
    private float aggressiveCooldown = 1f;
    private float enragedCooldown= 0.6f;
    public int   specialBulletCount = 8;
    string currentState = "Normal";
    float shootTimer=0.3f;
    float currentCooldown;
    bool canAttack = true;
    void Start()
    {
        currentCooldown = normalCooldown;
    }

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
    GameObject GetBullet()
    {
        if (bullets == null || bullets.Length == 0) return null;
        int index = Random.Range(0, bullets.Length);
        return bullets[index];
    }

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

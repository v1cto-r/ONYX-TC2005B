using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    public GameObject specialBullet;
    //PlayerFatigueSystem fatigueSystem;
    public float specialBulletSpeed = 15f;
    public float specialChargeRechargeTime = 5f;
    public int specialChargeMax = 3;
    public float comboResetTime = 2f;
    public int currentCombo = 0;
    public int currentSpecialCharges = 3;
    public float specialRechargeTimer = 0f;
    float shootTimer = 0f;
    float comboTimer = 0f;
    public float shootCooldown = 0.2f;
    public float baseCooldown;

    void Start()
    {
        baseCooldown = shootCooldown;
        //fatigueSystem = GetComponent<PlayerFatigueSystem>();
        currentSpecialCharges = specialChargeMax;
        specialRechargeTimer = 0f;
    }

    void Update()
    {
        if (shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
        }

       
        if (currentCombo > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f){
                ResetCombo();
            }
        }
    }

    public void shoot()
    {
        if (shootTimer > 0f || bullet == null || firePoint == null) return;
        GameObject bulletShoot = Instantiate(bullet, firePoint.position, Quaternion.identity);

        shootTimer = shootCooldown;

        currentCombo++;
        comboTimer = comboResetTime;

    }

    public void ResetCombo()
    {
        currentCombo = 0;
        comboTimer = 0f;
    }

}

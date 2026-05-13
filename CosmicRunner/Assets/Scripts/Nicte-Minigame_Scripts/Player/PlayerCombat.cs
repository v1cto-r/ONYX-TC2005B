using UnityEngine;

namespace Nicte.Minigame{
// Maneja disparos normales, especiales y combo del jugador
public class PlayerCombat : MonoBehaviour
{
    // Prefabs de bala y punto de disparo
    public GameObject bullet;
    public Transform firePoint;
    public GameObject specialBullet;
    // Sistema de combo y carga especial
    public int specialChargeMax = 3;
    public float comboResetTime = 2f;
    public int currentCombo = 0;
    public int currentSpecialCharges = 3;
    // Temporizadores internos para disparo y combo
    float shootTimer = 0f;
    float comboTimer = 0f;
    private float shootCooldown = 0.1f;


    // Arranca con la carga especial completa
    void Start()
    {
        currentSpecialCharges = specialChargeMax;
    }

    // Mantiene los temporizadores del disparo y combo
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

    // Instancia una bala normal si el disparo esta disponible
    public void Shoot()
    {
        if (shootTimer > 0f || bullet == null || firePoint == null) return;
        GameObject bulletShoot = Instantiate(bullet, firePoint.position, Quaternion.identity);

        shootTimer = shootCooldown;

        currentCombo++;
        comboTimer = comboResetTime;

    }

    // Dispara una bala especial consumiendo su carga
    public void SpecialAttack()
    {
        if (currentSpecialCharges <= 0 || specialBullet == null || firePoint == null) return;
        GameObject bullet = Instantiate(specialBullet, firePoint.position, Quaternion.identity);
    }

    // Limpia el combo cuando expira su tiempo limite
    public void ResetCombo()
    {
        currentCombo = 0;
        comboTimer = 0f;
    }

    // Restaura la carga especial al maximo y actualiza la UI
    public void RechargeSpecial()
    {
        currentSpecialCharges = specialChargeMax;
        SpecialAttackUI specialUI = FindObjectOfType<SpecialAttackUI>();
        if (specialUI != null)
        {
            specialUI.UpdateCharge(currentSpecialCharges);
        }
    }
}
}
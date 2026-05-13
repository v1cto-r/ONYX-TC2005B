using UnityEngine;

namespace Nicte.Minigame{
// Proyectil especial del jugador con daño variable
public class PlayerSpecialBullet : MonoBehaviour
{

    // Referencia a la UI de combo para sumar impacto
    ComboUI comboUI;
    // Vida util y velocidad de la bala especial
    public float lifetime = 4f;
    public float bulletSpeed = 10f;

    // Prepara la bala especial y reproduce sonido de disparo
    void Start()
    {
        Destroy(gameObject, lifetime);
        comboUI = FindObjectOfType<ComboUI>();
        SFXManager.Instance.BulletSound();
    }

    // Daño aleatorio al impactar a un enemigo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            SFXManager.Instance.DamageSound();
            
            int damage = Random.Range(1,3);
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
            comboUI.UpdateCombo(comboUI.currentCombo + 1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Desplaza la bala especial hacia la izquierda
    void Update()
    {
        transform.position+= Vector3.left*Time.deltaTime*bulletSpeed;
    }
}
}
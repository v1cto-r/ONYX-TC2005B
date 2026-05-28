using UnityEngine;

namespace Nicte.Minigame{
// Proyectil normal del jugador
public class PlayerBullet : MonoBehaviour
{

    // Referencias a combo y creditos al acertar
    ComboUI comboUI;
    GeneralUI generalUI;
    // Vida util y velocidad del disparo
    public float lifetime = 3f;
    public float bulletSpeed = -10f;

    // Busca las interfaces necesarias y reproduce el sonido
    void Start()
    {
        Destroy(gameObject, lifetime);
        comboUI = FindObjectOfType<ComboUI>();
        generalUI = FindObjectOfType<GeneralUI>();
        SFXManager.Instance.BulletSound();
    }

    // Aplica daño al enemigo o destruye la bala al chocar con otra cosa
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            SFXManager.Instance.DamageSound();
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(1);
            comboUI.UpdateCombo(comboUI.currentCombo + 1);
            generalUI.UpdateCredits(100);
        }else{
            Destroy(gameObject);
        }
    }

    // Mueve la bala hacia la izquierda cada frame
    void Update()
    {
        transform.position+= Vector3.left*Time.deltaTime*bulletSpeed;
    }
}
}

using UnityEngine;

public class PlayerBullet : MonoBehaviour
{

    ComboUI comboUI;
    GeneralUI generalUI;
    public float lifetime = 3f;
    public float bulletSpeed = -10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
        comboUI = FindObjectOfType<ComboUI>();
        generalUI = FindObjectOfType<GeneralUI>();
        SFXManager.Instance.BulletSound();
    }

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

    void Update()
    {
        transform.position+= Vector3.left*Time.deltaTime*bulletSpeed;
    }
}

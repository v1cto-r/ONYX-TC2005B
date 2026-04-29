using UnityEngine;

public class PlayerSpecialBullet : MonoBehaviour
{

    ComboUI comboUI;
    public float lifetime = 4f;
    public float bulletSpeed = 15f;

    void Start()
    {
        Destroy(gameObject, lifetime);
        comboUI = FindObjectOfType<ComboUI>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            int damage = Random.Range(1,3);
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
            comboUI.UpdateCombo(comboUI.currentCombo + 1);
        }else{
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.position+= Vector3.left*Time.deltaTime*bulletSpeed;
    }
}

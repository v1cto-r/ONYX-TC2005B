using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float lifetime = 3f;
    public float bulletSpeed = -10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(1);
        }
    }

    void Update()
    {
        transform.position+= Vector3.left*Time.deltaTime*bulletSpeed;
    }
}

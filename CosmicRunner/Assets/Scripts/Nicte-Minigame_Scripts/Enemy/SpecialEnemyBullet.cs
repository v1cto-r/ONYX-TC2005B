using UnityEngine;

public class SpecialEnemyBullet : MonoBehaviour
{
    public float lifetime = 3f;
    public float bulletSpeed = 10f;

    private int damage = 2;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.position+= Vector3.right*Time.deltaTime*bulletSpeed;
    }

}

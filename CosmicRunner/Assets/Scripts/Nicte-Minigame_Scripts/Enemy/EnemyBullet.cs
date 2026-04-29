using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float lifetime = 3f;
    public float bulletSpeed = 8f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.position+= Vector3.right*Time.deltaTime*bulletSpeed;
    }

}

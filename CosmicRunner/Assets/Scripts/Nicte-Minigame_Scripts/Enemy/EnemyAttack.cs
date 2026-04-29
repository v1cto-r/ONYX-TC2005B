using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject[] bullets;
    public Transform firePoint;
    public float normalCooldown = 2f;
    public float aggressiveCooldown = 1f;
    public float enragedCooldown= 0.4f;
    public int   specialBulletCount = 8;
    string currentState = "Normal";
    float shootTimer    = 0f;
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
        GameObject selectedBullet = GetBullet();
        if (selectedBullet == null) return;
        Instantiate(selectedBullet, firePoint.position, Quaternion.identity);

    }

    void SpecialAttack()
    {
        if (currentState == "Enraged")
        {
            float angleStep = 360f / specialBulletCount;
            for (int i = 0; i < specialBulletCount; i++)
            {
                float angle = i * angleStep;
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),Mathf.Sin(angle * Mathf.Deg2Rad));
                //SpawnBullet(bullet, transform.position, dir, 8);
            }
        }
        
    }
    void SpawnBullet(GameObject prefab, Vector3 pos, Vector2 dir, float speed)
    {
        GameObject bullet = Instantiate(prefab, pos, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = dir * speed;

    }

    public void StateChanged(string state)
    {
        currentState = state;

        if (state == "Normal")         currentCooldown = normalCooldown;
        else if (state == "Aggressive") currentCooldown = aggressiveCooldown;
        else if (state == "Enraged")    currentCooldown = enragedCooldown;
    }

    public void SetAttackEnabled(bool enabled)
    {
        canAttack = enabled;
    }
}

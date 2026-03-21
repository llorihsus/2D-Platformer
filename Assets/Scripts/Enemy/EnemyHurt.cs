using UnityEngine;

public class EnemyHurt : MonoBehaviour
{
    [SerializeField] int damageAmount = 1;

    EnemyPatrol enemyPatrol;

    void Start()
    {
        enemyPatrol = GetComponent<EnemyPatrol>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (enemyPatrol == null || !enemyPatrol.IsAlive) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damageAmount);
        }
    }
}
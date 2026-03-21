using UnityEngine;

public class EnemyHeadHitBox : MonoBehaviour
{
    [Header("Stomp")]
    [SerializeField] int stompDamage = 1;
    [SerializeField] float bounceForce = 12f;

    EnemyPatrol enemyPatrol;

    void Start()
    {
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (enemyPatrol == null || !enemyPatrol.IsAlive) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        // Only allow stomp when player is falling or moving downward
        if (playerRb.linearVelocity.y <= 0f)
        {
            enemyPatrol.TakeDamage(stompDamage);

            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);

            GameSession.Instance?.AddScore(100);
            CameraShake.Instance?.Shake(1f, 0.15f);
        }
    }
}
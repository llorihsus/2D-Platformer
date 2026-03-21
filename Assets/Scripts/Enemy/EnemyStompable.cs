using UnityEngine;

public class EnemyStompable : MonoBehaviour
{
    [Header("Stomp Settings")]
    [SerializeField] float stompBounceForce = 12f;
    [SerializeField] float stompThreshold = 0.3f;
    // how far above the enemy the player must be to count as a stomp

    EnemyPatrol enemyPatrol;

    void Start()
    {
        enemyPatrol = GetComponent<EnemyPatrol>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

        if (playerRb == null) return;

        ContactPoint2D contact = collision.GetContact(0);

        bool playerIsFalling = playerRb.linearVelocity.y <= 0f;
        bool hitFromAbove = contact.normal.y < -0.5f;
        bool playerAboveEnemy = collision.transform.position.y > transform.position.y + stompThreshold;

        if (playerIsFalling && hitFromAbove && playerAboveEnemy)
        {
            // Kill enemy
            enemyPatrol?.Die();

            // Bounce player upward
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, stompBounceForce);

            CameraShake.Instance?.Shake(1f, 0.15f);
        }
        else
        {
            // Player got hurt instead
            playerHealth?.TakeDamage(1);
        }
    }
}

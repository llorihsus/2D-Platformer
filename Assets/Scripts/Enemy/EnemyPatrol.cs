using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] Transform leftEdge;
    [SerializeField] Transform rightEdge;
    [SerializeField] float edgeBuffer = 0.1f;

    [Header("Ground Detection")]
    [SerializeField] Transform groundDetect;
    [SerializeField] float detectRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("Health")]
    [SerializeField] int maxHealth = 1;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;

    bool movingRight = true;
    bool isAlive = true;
    int currentHealth;

    public bool IsAlive => isAlive;
    public int CurrentHealth => currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!isAlive) return;
        CheckTurnConditions();
    }

    void FixedUpdate()
    {
        if (!isAlive) return;
        Patrol();
    }

    void Patrol()
    {
        float direction = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        spriteRenderer.flipX = !movingRight;
    }

    void CheckTurnConditions()
    {
        // Turn around at patrol boundaries
        if (leftEdge != null && !movingRight && transform.position.x <= leftEdge.position.x + edgeBuffer)
        {
            TurnAround();
            return;
        }

        if (rightEdge != null && movingRight && transform.position.x >= rightEdge.position.x - edgeBuffer)
        {
            TurnAround();
            return;
        }

        // Turn around if no ground ahead
        if (groundDetect != null)
        {
            bool groundAhead = Physics2D.OverlapCircle(groundDetect.position, detectRadius, groundLayer);
            if (!groundAhead)
            {
                TurnAround();
            }
        }
    }

    void TurnAround()
    {
        movingRight = !movingRight;
    }

    public void TakeDamage(int amount)
    {
        if (!isAlive) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    public void Die()
    {
        if (!isAlive) return;

        isAlive = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        if (animator != null)
            animator.SetTrigger("die");

        Destroy(gameObject, 0.8f);
    }

    void OnDrawGizmosSelected()
    {
        if (groundDetect != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundDetect.position, detectRadius);
        }

        if (leftEdge != null && rightEdge != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(leftEdge.position, rightEdge.position);
        }
    }
}
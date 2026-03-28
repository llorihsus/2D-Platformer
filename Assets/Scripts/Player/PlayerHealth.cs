using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 3;
    [SerializeField] float invincibilityDuration = 1.5f;

    [Header("UI")]
    [SerializeField] Image[] heartImages;
    [SerializeField] Sprite fullHeart;

    [Header("Effects")]
    [SerializeField] GameObject deathParticles;

    int currentHealth;
    bool isInvincible;
    float invincibilityTimer;

    PlayerMovement movement;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = fullHeart;
        }

        UpdateHeartUI();
    }

    void Update()
    {
        HandleInvincibility();
    }

    void HandleInvincibility()
    {
        if (!isInvincible) return;

        invincibilityTimer -= Time.deltaTime;
        spriteRenderer.enabled = Mathf.Sin(invincibilityTimer * 20f) > 0;

        if (invincibilityTimer <= 0)
        {
            isInvincible = false;
            spriteRenderer.enabled = true;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateHeartUI();
        CameraShake.Instance?.Shake(2f, 0.3f);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    void Die()
    {
        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        movement.OnDeath();
        Invoke(nameof(GameOver), 1f);
    }

    void GameOver()
    {
        FindFirstObjectByType<GameSession>()?.ProcessPlayerDeath();
    }

    void UpdateHeartUI()
    {
        GameSession session = FindFirstObjectByType<GameSession>();
        int livesRemaining = session != null ? session.GetLives() : maxHealth;

        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < livesRemaining;
        }
    }
}
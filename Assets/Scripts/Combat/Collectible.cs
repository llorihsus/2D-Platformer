using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] int pointValue = 100;
    [SerializeField] GameObject collectEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameSession.Instance?.AddScore(pointValue);
            AudioManager.Instance?.PlayCollect();
            if (collectEffect != null)
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

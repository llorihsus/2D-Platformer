using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 999;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }
}
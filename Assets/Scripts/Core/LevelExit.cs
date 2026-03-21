using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float exitDelay = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LoadNextLevel());
        }
    }

    System.Collections.IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(exitDelay);
        GameSession.Instance?.LoadNextLevel();
    }
}

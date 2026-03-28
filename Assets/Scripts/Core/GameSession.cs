using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    [SerializeField] int startingLives = 3;
    [SerializeField] int lives = 3;
    int score = 0;

    void Awake()
    {
        // Singleton pattern — one GameSession persists across scenes
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        lives = startingLives;
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public int GetScore() => score;

    public int GetLives() => lives;

    public void ProcessPlayerDeath()
    {
        lives--;

        if (lives > 0)
            ReloadCurrentScene();
        else
            LoadGameOver();
    }

    void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void LoadGameOver()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("GameOver");
    }

    public void LoadNextLevel()
    {
        lives = startingLives;
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene("Win");
    }
}
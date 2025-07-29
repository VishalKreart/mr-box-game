using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public Text gameplayScoreText; // Assign in inspector
    public Text gameOverScoreText; // Assign in inspector (on GameOverPanel)
    private int score = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateGameplayScore();
    }

    public void ShowGameOverScore()
    {
        if (gameOverScoreText != null)
            gameOverScoreText.text = "Score: " + score;
    }

    private void UpdateGameplayScore()
    {
        if (gameplayScoreText != null)
            gameplayScoreText.text = "Score: " + score;
    }

    public void ResetScore()
    {
        score = 0;
        UpdateGameplayScore();
    }

    public int GetScore()
    {
        return score;
    }
} 
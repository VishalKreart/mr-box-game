using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public string date;
    public GameMode gameMode;

    public LeaderboardEntry(string name, int score, GameMode mode)
    {
        this.playerName = name;
        this.score = score;
        this.date = System.DateTime.Now.ToString("MM/dd/yyyy");
        this.gameMode = mode;
    }
}

[System.Serializable]
public class LeaderboardWrapper
{
    public LeaderboardEntry[] entries;
}

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public CameraStackFollow cameraStackFollow; // Assign in inspector
    public BackgroundColorManager backgroundColorManager; // Assign in inspector

    [Header("Refs")]
    [SerializeField] public ContinueUIManager continueUI;

    private bool isGameOver = false;

    public void GameOver()
    {
        Debug.Log("Game Over!");


        // Stop background color changes
        if (backgroundColorManager != null)
        {
            backgroundColorManager.OnGameEnd();
        }

        // Check if we're in Time Attack mode
        TimeAttackManager timeAttackManager = FindObjectOfType<TimeAttackManager>();
        if (timeAttackManager != null && timeAttackManager.IsTimeAttackMode())
        {
            // Let TimeAttackManager handle the game over for Time Attack mode
            timeAttackManager.OnTowerFell();
        }
        else
        {
            // Normal game over for Classic mode
            Time.timeScale = 0f; // Pause the game
            gameOverPanel.SetActive(true);
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.ShowGameOverScore();
                SaveHighScore();
            }
            if (cameraStackFollow != null) cameraStackFollow.ZoomOutOnGameOver();
        }
        // Show interstitial every 3rd game over (if ads not removed)
        MonetizationManager.Instance.OnGameOver();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        // Start fresh background color
        if (backgroundColorManager != null)
        {
            backgroundColorManager.OnGameStart();
        }

        // Reset Time Attack text if needed
        TimeAttackManager timeAttackManager = FindObjectOfType<TimeAttackManager>();
        if (timeAttackManager != null)
        {
            timeAttackManager.ResetGameOverText();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Load the main menu scene
    }

    private void SaveHighScore()
    {
        if (ScoreManager.Instance != null)
        {
            int currentScore = ScoreManager.Instance.GetScore();

            // Check which mode we're in and save to appropriate high score
            int selectedMode = PlayerPrefs.GetInt("SelectedGameMode", 0);
            GameMode gameMode = (GameMode)selectedMode;

            // Save to legacy high score system (for backward compatibility)
            string highScoreKey = "";
            if (gameMode == GameMode.Classic)
            {
                highScoreKey = "HighScore_Classic";
            }
            else if (gameMode == GameMode.TimeAttack)
            {
                highScoreKey = "HighScore_TimeAttack";
            }

            int highScore = PlayerPrefs.GetInt(highScoreKey, 0);

            if (currentScore > highScore)
            {
                PlayerPrefs.SetInt(highScoreKey, currentScore);
                PlayerPrefs.Save();
                Debug.Log("New " + gameMode + " High Score: " + currentScore);
                // Submit score to PlayFab online leaderboard
                if (PlayFabManager.Instance != null)
                {
                    PlayFabManager.Instance.SubmitScore(currentScore, gameMode);
                }
                else
                {
                    Debug.LogWarning("PlayFabManager not found. Score not submitted to online leaderboard.");
                }
            }

            // Save to local leaderboard system using PlayerPrefs for cross-scene communication
            SaveScoreToLeaderboard(currentScore, gameMode);



            // Check if it's a new high score and show celebration
            if (IsNewHighScore(currentScore, gameMode))
            {
                ShowNewHighScoreCelebration();
            }
        }
    }

    private void ShowNewHighScoreCelebration()
    {
        // You can add visual/audio feedback here
        Debug.Log("🎉 NEW HIGH SCORE! 🎉");

        // Optional: Show a popup or animation
        // You can add UI elements for this later
    }

    private void SaveScoreToLeaderboard(int score, GameMode gameMode)
    {
        // Create a temporary score entry
        string playerName = PlayerPrefs.GetString("PlayerDisplayName", "Player 1");
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score, gameMode);

        // Get existing leaderboard data
        string leaderboardKey = gameMode == GameMode.Classic ? "ClassicLeaderboard" : "TimeAttackLeaderboard";
        string existingData = PlayerPrefs.GetString(leaderboardKey, "");

        List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();

        if (!string.IsNullOrEmpty(existingData))
        {
            var wrapper = JsonUtility.FromJson<LeaderboardWrapper>(existingData);
            if (wrapper != null && wrapper.entries != null)
            {
                leaderboard = wrapper.entries.ToList();
            }
        }

        // Add new score
        leaderboard.Add(newEntry);

        // Sort and keep only top 10
        leaderboard = leaderboard.OrderByDescending(x => x.score).Take(10).ToList();

        // Save back to PlayerPrefs
        var newWrapper = new LeaderboardWrapper { entries = leaderboard.ToArray() };
        string newData = JsonUtility.ToJson(newWrapper);
        PlayerPrefs.SetString(leaderboardKey, newData);
        PlayerPrefs.Save();
    }

    private bool IsNewHighScore(int score, GameMode gameMode)
    {
        string leaderboardKey = gameMode == GameMode.Classic ? "ClassicLeaderboard" : "TimeAttackLeaderboard";
        string existingData = PlayerPrefs.GetString(leaderboardKey, "");

        if (string.IsNullOrEmpty(existingData))
        {
            return true; // First score is always a high score
        }

        var wrapper = JsonUtility.FromJson<LeaderboardWrapper>(existingData);
        if (wrapper != null && wrapper.entries != null && wrapper.entries.Length > 0)
        {
            return score > wrapper.entries[0].score;
        }

        return true;
    }

    // Call this when the player loses balance (before finalizing Game Over)
    public void OnPlayerFailed()
    {
        if (isGameOver) return;

        // If a rewarded ad is available, show continue popup; else go straight to Game Over
        if (MonetizationManager.Instance.CanContinue())
        {
            ShowContinueOptions();
        }
        else
        {
            GameOver();
        }
    }

    // Old method name kept for compatibility with older code
    public void ShowContinueOptions()
    {
        if (continueUI != null)
        {
            continueUI.ShowContinuePanel(); // calls new popup under the hood
        }
        else
        {
            GameOver();
        }
    }

    // Called by Continue flow after rewarded completes
    public void ResumeGame()
    {
        // Your logic to resume gameplay with intact tower:
        // e.g., unpause physics, reset a "failed" flag, restore input, etc.
        isGameOver = false;
        Debug.Log("[GameManager] ResumeGame()");
    }
    // Legacy compatibility (if something calls this exact method)
    public void ShowContinuePanel()
    {
        ShowContinueOptions();
    }
}
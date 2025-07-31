using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public CameraStackFollow cameraStackFollow; // Assign in inspector

    public void GameOver()
    {
        Debug.Log("Game Over!");
        
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
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        
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
            }
        }
    }
}

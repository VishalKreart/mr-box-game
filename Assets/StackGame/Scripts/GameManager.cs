using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public CameraStackFollow cameraStackFollow; // Assign in inspector

    public void GameOver()
    {
        Debug.Log("Game Over!");
        Time.timeScale = 0f; // Pause the game
        gameOverPanel.SetActive(true);
        if (ScoreManager.Instance != null) 
        {
            ScoreManager.Instance.ShowGameOverScore();
            SaveHighScore();
        }
        if (cameraStackFollow != null) cameraStackFollow.ZoomOutOnGameOver();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
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
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            
            if (currentScore > highScore)
            {
                PlayerPrefs.SetInt("HighScore", currentScore);
                PlayerPrefs.Save();
                Debug.Log("New High Score: " + currentScore);
            }
        }
    }
}

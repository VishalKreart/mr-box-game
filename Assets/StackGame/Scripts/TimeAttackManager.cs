using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TimeAttackManager : MonoBehaviour
{
    [Header("Time Attack Settings")]
    public float timeLimit = 25f; // Even shorter - 25 seconds base time
    public float currentTime;
    public bool isTimeAttackMode = false;
    
    [Header("Time Limit Options")]
    public float easyTimeLimit = 20f;    // Easy mode: 20 seconds
    public float normalTimeLimit = 25f;  // Normal mode: 25 seconds  
    public float hardTimeLimit = 35f;    // Hard mode: 35 seconds
    
    [Header("Difficulty Options")]
    public bool useProgressiveTime = false; // Disabled - no time extensions
    public bool survivalRequired = true; // Tower must survive for score to count
    
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    //public GameObject timeAttackUI;
    public TextMeshProUGUI failedText; // FAILED text for Time Attack mode
    public GameObject gameOverPanel; // Reference to game over panel
    
    [Header("Game Events")]
    public UnityEngine.Events.UnityEvent onTimeUp;
    
    private BoxSpawner boxSpawner;
    private GameManager gameManager;
    private bool gameEnded = false;
    
    void Start()
    {
        // Check for multiple instances
        TimeAttackManager[] managers = FindObjectsOfType<TimeAttackManager>();
        if (managers.Length > 1)
        {
            Debug.LogWarning("Multiple TimeAttackManager instances found: " + managers.Length);
        }
        
        // Check if Time Attack mode is selected
        int selectedMode = PlayerPrefs.GetInt("SelectedGameMode", 0);
        isTimeAttackMode = (GameMode)selectedMode == GameMode.TimeAttack;
        
        Debug.Log("Selected game mode: " + (GameMode)selectedMode + ", isTimeAttackMode: " + isTimeAttackMode);
        
        if (isTimeAttackMode)
        {
            // Use the time limit set in inspector (no longer forced to 25f)
            Debug.Log("Time Attack Mode detected. Using time limit from inspector: " + timeLimit + " seconds");
            InitializeTimeAttackMode();
        }
        else
        {
            // Hide Time Attack UI for Classic mode
            if (timerText != null)
                timerText.gameObject.SetActive(false);
        }
    }
    
    void InitializeTimeAttackMode()
    {
        Debug.Log("Initializing Time Attack Mode with " + timeLimit + " seconds");
        
        // Show Time Attack UI
        if (timerText != null)
            timerText.gameObject.SetActive(true);
            
        // Initialize timer with the time limit from inspector
        currentTime = timeLimit;
        Debug.Log("Starting Time Attack with " + currentTime + " seconds");
        UpdateTimerDisplay();
        
        // Get references
        boxSpawner = FindObjectOfType<BoxSpawner>();
        gameManager = FindObjectOfType<GameManager>();
        
        // Start timer
        StartCoroutine(TimeAttackCoroutine());
    }
    
    void Update()
    {
        if (isTimeAttackMode && !gameEnded)
        {
            // Update timer display every frame for smooth countdown
            UpdateTimerDisplay();
        }
    }
    
    IEnumerator TimeAttackCoroutine()
    {
        while (currentTime > 0 && !gameEnded)
        {
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
            
            // Update timer display
            UpdateTimerDisplay();
            
            // Check if time is up
            if (currentTime <= 0)
            {
                EndTimeAttack();
                break;
            }
        }
    }
    
    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            // Change color when time is running low
            if (currentTime <= 10f)
            {
                timerText.color = Color.red;
            }
            else if (currentTime <= 30f)
            {
                timerText.color = Color.yellow;
            }
            else
            {
                timerText.color = Color.white;
            }
        }
    }
    
    void EndTimeAttack()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        Debug.Log("Time Attack Mode - Time's up!");
        
        // Stop box spawning
        if (boxSpawner != null)
        {
            boxSpawner.StopSpawning();
        }
        
        // Calculate final score - only if tower survived
        int finalScore = 0;
        bool towerSurvived = CheckTowerSurvival();
        
        if (towerSurvived && survivalRequired)
        {
            finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetScore() : 0;
            Debug.Log("Time Attack SUCCESS! Tower survived. Final Score: " + finalScore);
            ShowSuccessUI(finalScore);
        }
        else
        {
            Debug.Log("Time Attack FAILED! Tower fell or survival not required. Score: 0");
            ShowFailedUI();
        }
        
        // Trigger game over
        if (onTimeUp != null)
        {
            onTimeUp.Invoke();
        }
        
        // End game through GameManager
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }
    
    void ShowSuccessUI(int score)
    {
        // Save high score for Time Attack mode
        SaveTimeAttackHighScore(score);
        
        // Save to new leaderboard system
        SaveTimeAttackScoreToLeaderboard(score);
        
        // Show normal game over with score
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Find and modify the game over text for success
            TextMeshProUGUI gameOverText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (gameOverText != null)
            {
                gameOverText.text = "Time Attack Complete!";
                gameOverText.color = Color.green;
            }
        }
        
        // Use ScoreManager to update the score display
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ShowGameOverScore();
        }
        
        // Hide failed text if it was showing
        if (failedText != null)
        {
            failedText.gameObject.SetActive(false);
        }
        // Show interstitial every 3rd game over (if ads not removed)
        //MonetizationManager.Instance.OnGameOver();
    }
    
    void SaveTimeAttackHighScore(int score)
    {
        int highScore = PlayerPrefs.GetInt("HighScore_TimeAttack", 0);
        
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore_TimeAttack", score);
            PlayerPrefs.Save();
            Debug.Log("New Time Attack High Score: " + score);

            if (PlayFabManager.Instance != null)
            {
                PlayFabManager.Instance.SubmitScore(score, GameMode.TimeAttack);
            }
            else
            {
                Debug.LogWarning("PlayFabManager not found. Score not submitted to online leaderboard.");
            }
        }
    }
    
    void SaveTimeAttackScoreToLeaderboard(int score)
    {
        // Create a new entry
        string playerName = PlayerPrefs.GetString("PlayerDisplayName", "Player 1");
        LeaderboardEntry newEntry = new LeaderboardEntry(playerName, score, GameMode.TimeAttack);

        // Get existing Time Attack leaderboard data
        string existingData = PlayerPrefs.GetString("TimeAttackLeaderboard", "");
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
        PlayerPrefs.SetString("TimeAttackLeaderboard", newData);
        PlayerPrefs.Save();
    }
    
    void ShowFailedUI()
    {
        // Pause the game
        Time.timeScale = 0f;
        
        // Show the game over panel (for buttons)
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Find and modify the game over text
            TextMeshProUGUI gameOverText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (gameOverText != null)
            {
                gameOverText.text = "FAILED!\nTower Fell";
                gameOverText.color = Color.red;
            }
            
        }
        
        // Hide the ScoreManager's game over score text
        if (ScoreManager.Instance != null && ScoreManager.Instance.gameOverScoreText != null)
        {
            ScoreManager.Instance.gameOverScoreText.gameObject.SetActive(false);
        }
        
        // Hide the separate failed text (we're using the game over panel instead)
        if (failedText != null)
        {
            failedText.gameObject.SetActive(false);
        }
        
        // Zoom out camera if available
        CameraStackFollow cameraStackFollow = FindObjectOfType<CameraStackFollow>();
        if (cameraStackFollow != null) 
        {
            cameraStackFollow.ZoomOutOnGameOver();
        }

        // Show interstitial every 3rd game over (if ads not removed)
        //MonetizationManager.Instance.OnGameOver();
    }
    
    // Check if the tower is still standing
    bool CheckTowerSurvival()
    {
        // Find all boxes in the scene
        GameObject[] allBoxes = GameObject.FindGameObjectsWithTag("Box"); // Make sure boxes have "Box" tag
        if (allBoxes.Length == 0) return false;
        
        // Check if any boxes are still in the scene (not destroyed)
        foreach (GameObject box in allBoxes)
        {
            if (box != null && box.activeInHierarchy)
            {
                // Check if box is still in a reasonable position (not fallen off screen)
                if (box.transform.position.y > -10f) // Adjust threshold as needed
                {
                    return true; // At least one box is still in play
                }
            }
        }
        
        return false; // All boxes have fallen or been destroyed
    }
    
    // Called when a box is successfully landed (for tracking purposes)
    public void OnBoxLanded()
    {
        // No time extension - just track successful landings
        Debug.Log("Box landed successfully - score increased");
    }
    
    public void PauseTimer()
    {
        if (isTimeAttackMode)
        {
            StopAllCoroutines();
        }
    }
    
    public void ResumeTimer()
    {
        if (isTimeAttackMode && !gameEnded)
        {
            StartCoroutine(TimeAttackCoroutine());
        }
    }
    
    public float GetRemainingTime()
    {
        return currentTime;
    }
    
    public bool IsTimeAttackMode()
    {
        return isTimeAttackMode;
    }
    
    // Called when tower falls during gameplay (from GameManager or FallDetector)
    public void OnTowerFell()
    {
        if (isTimeAttackMode && !gameEnded)
        {
            Debug.Log("Tower fell during Time Attack mode - FAILED!");
            ShowFailedUI();
            gameEnded = true;
            
            // Stop the timer
            StopAllCoroutines();
        }
    }
    
    // Reset game over text to default (called when restarting)
    public void ResetGameOverText()
    {
        if (gameOverPanel != null)
        {
            // Reset main game over text
            TextMeshProUGUI gameOverText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (gameOverText != null)
            {
                gameOverText.text = "Game Over";
                gameOverText.color = Color.white;
            }
        }
        
        // Restore ScoreManager's game over score text
        if (ScoreManager.Instance != null && ScoreManager.Instance.gameOverScoreText != null)
        {
            ScoreManager.Instance.gameOverScoreText.gameObject.SetActive(true);
        }
    }
} 
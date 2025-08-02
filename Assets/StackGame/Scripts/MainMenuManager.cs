using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public enum GameMode
{
    Classic,
    TimeAttack
}

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button playButton;
    public Button leaderboardButton;
    public Button settingsButton;
    public Button quitButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI highScoreText;
    
    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Button closeSettingsButton;
    public Button resetTutorialButton;
    
    [Header("Mode Selection Panel")]
    public GameObject modeSelectPanel;
    public Button classicModeButton;
    public Button timeAttackModeButton;
    public Button backToMenuButton;
    
    [Header("Animation")]
    public Animator titleAnimator; // Optional: for title animation
    
    private string originalResetButtonText = "Reset Tutorial";
    
    void Start()
    {
        SetupButtons();
        UpdateHighScore();
        ShowMainMenu();
    }
    
    void SetupButtons()
    {
        // Main menu buttons
        if (playButton != null)
            playButton.onClick.AddListener(OpenModeSelection);
            
        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(OpenLeaderboard);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
            
        // Settings panel buttons
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(CloseSettings);
            
        if (resetTutorialButton != null)
            resetTutorialButton.onClick.AddListener(ResetTutorial);
            
        // Mode selection buttons
        if (classicModeButton != null)
            classicModeButton.onClick.AddListener(() => StartGame(GameMode.Classic));
            
        if (timeAttackModeButton != null)
            timeAttackModeButton.onClick.AddListener(() => StartGame(GameMode.TimeAttack));
            
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(ShowMainMenu);
    }
    
    void UpdateHighScore()
    {
        if (highScoreText != null)
        {
            int classicHighScore = PlayerPrefs.GetInt("HighScore_Classic", 0);
            int timeAttackHighScore = PlayerPrefs.GetInt("HighScore_TimeAttack", 0);
            
            highScoreText.text = "Classic: " + classicHighScore + "\nTime Attack: " + timeAttackHighScore;
        }
    }
    
    public void OpenModeSelection()
    {
        if (modeSelectPanel != null)
        {
            modeSelectPanel.SetActive(true);
        }
    }
    
    public void OpenLeaderboard()
    {
        SimpleLeaderboardManager[] allManagers = FindObjectsOfType<SimpleLeaderboardManager>(true);
        SimpleLeaderboardManager leaderboardManager = allManagers.Length > 0 ? allManagers[0] : null;
        
        if (leaderboardManager != null)
        {
            leaderboardManager.ShowLeaderboard();
        }
    }
    
    public void StartGame(GameMode mode)
    {
        Debug.Log("Starting game in " + mode + " mode...");
        // Store the selected game mode
        PlayerPrefs.SetInt("SelectedGameMode", (int)mode);
        PlayerPrefs.Save();
        // Load the gameplay scene
        SceneManager.LoadScene("MainScene");
    }
    
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    
    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialComplete", 0);
        PlayerPrefs.Save();
        Debug.Log("Tutorial reset! It will show again when you start the game.");
        
        // Show feedback to user
        if (resetTutorialButton != null)
        {
            TextMeshProUGUI buttonText = resetTutorialButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                originalResetButtonText = buttonText.text;
                buttonText.text = "Tutorial Reset!";
                Invoke("RestoreResetButtonText", 2f);
            }
        }
    }
    
    public void RestoreResetButtonText()
    {
        if (resetTutorialButton != null)
        {
            TextMeshProUGUI buttonText = resetTutorialButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = originalResetButtonText;
            }
        }
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void ShowMainMenu()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        if (modeSelectPanel != null)
            modeSelectPanel.SetActive(false);
    }
} 
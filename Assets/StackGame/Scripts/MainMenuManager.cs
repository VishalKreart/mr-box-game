using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI highScoreText;
    
    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Button closeSettingsButton;
    public Button resetTutorialButton;
    
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
            playButton.onClick.AddListener(StartGame);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
            
        // Settings panel buttons
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(CloseSettings);
            
        if (resetTutorialButton != null)
            resetTutorialButton.onClick.AddListener(ResetTutorial);
    }
    
    void UpdateHighScore()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "High Score: " + highScore;
        }
    }
    
    public void StartGame()
    {
        Debug.Log("Starting game...");
        // Load the gameplay scene
        SceneManager.LoadScene("MainScene"); // Make sure your gameplay scene is named "MainScene"
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
    }
} 
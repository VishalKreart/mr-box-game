using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Firebase.Analytics;

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
    public Button removeAdsButton;
    public Button quitButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI highScoreText;
    
    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public Button closeSettingsButton;
    public Button rateUsSettingsButton;
    public Button moreGamesSettingsButton;
    public Button PrivacyPolicySettingsButton;
    public Button changeNameSettingsButton;
    public Button resetTutorialButton;
    public GameObject restorePurchasesButton;


    [Header("Mode Selection Panel")]
    public GameObject modeSelectPanel;
    public Button classicModeButton;
    public Button timeAttackModeButton;
    public Button backToMenuButton;

    [Header("Exit Game popup")]
    public GameObject exitPopUp;
    public Button exitYesButton;
    public Button exitNoButton;
    [Header("Rate US popup")]
    public GameObject rateUsPopUp;
    public Button rateNowButton;
    public Button rateLaterButton;

    private const string APP_LAUNCH_COUNT_KEY = "AppLaunchCount";
    private const string HAS_RATED_KEY = "HasRated";


    private string originalResetButtonText = "Reset Tutorial";
    
    [Header("First Launch Panel")]
    public GameObject firstLaunchPanel;
    public TMP_InputField firstLaunchNameInput;
    public Button confirmNameButton;
    //public TextMeshProUGUI welcomeText;

    private static bool hasTrackedLaunch = false;
    // Replace the TrackAppLaunch call in Start() with this:
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeOnLoad()
    {
        if (!hasTrackedLaunch)
        {
            // Find the MainMenuManager instance in the scene
            MainMenuManager instance = FindObjectOfType<MainMenuManager>();
            if (instance != null)
            {
                instance.TrackAppLaunch();
                hasTrackedLaunch = true;
            }
        }
    }


    void Start()
    {
        // TrackAppLaunch();
        SetupButtons();
        UpdateHighScore();
        CheckFirstLaunch();
    }

    private void TrackAppLaunch()
    {
        // Increment launch counter
        int launchCount = PlayerPrefs.GetInt(APP_LAUNCH_COUNT_KEY, 0) + 1;
        PlayerPrefs.SetInt(APP_LAUNCH_COUNT_KEY, launchCount);
        PlayerPrefs.Save();
        // Show rate popup every 3rd launch if not already rated
        if (launchCount % 3 == 0 && PlayerPrefs.GetInt(HAS_RATED_KEY, 0) == 0)
        {
            ShowRateUsPopup();
        }
    }
    private void ShowExitConfirmation()
    {
        if (exitPopUp != null)
        {
            exitPopUp.SetActive(true);
        }
    }
    private void HideExitConfirmation()
    {
        if (exitPopUp != null)
        {
            exitPopUp.SetActive(false);
        }
    }
    private void ShowRateUsPopup()
    {
        AnalyticsManager.Instance.LogEvent("rate_popup");
        if (rateUsPopUp != null)
        {
            rateUsPopUp.SetActive(true);
        }
    }
    private void HideRateUsPopup()
    {
        AnalyticsManager.Instance.LogEvent("rate_later");
        if (rateUsPopUp != null)
        {
            rateUsPopUp.SetActive(false);
        }
    }

    void SetupButtons()
    {
        // Main menu buttons
        if (playButton != null)
            playButton.onClick.AddListener(OpenModeSelection);
            
        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(OpenLeaderboard);


        if (removeAdsButton != null && MonetizationManager.Instance != null)
        {
            removeAdsButton.gameObject.SetActive(!MonetizationManager.Instance.IsAdsRemoved());
            removeAdsButton.onClick.AddListener(BuyRemoveAds);
            MonetizationManager.OnAdsRemoved += HandleAdsRemoved;
        }
            

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            
        //if (quitButton != null)
        //    quitButton.onClick.AddListener(QuitGame);
            
        // Settings panel buttons
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(CloseSettings);
            
        if (resetTutorialButton != null)
            resetTutorialButton.onClick.AddListener(ResetTutorial);

        if (changeNameSettingsButton != null)
            changeNameSettingsButton.onClick.AddListener(openChangeNamePopup);

        if (rateUsSettingsButton != null)
            rateUsSettingsButton.onClick.AddListener(rateUs);
        if (moreGamesSettingsButton != null)
            moreGamesSettingsButton.onClick.AddListener(moreGames);
        if (PrivacyPolicySettingsButton != null)
            PrivacyPolicySettingsButton.onClick.AddListener(privacyPolicy);

        // Mode selection buttons
        if (classicModeButton != null)
            classicModeButton.onClick.AddListener(() => StartGame(GameMode.Classic));
            
        if (timeAttackModeButton != null)
            timeAttackModeButton.onClick.AddListener(() => StartGame(GameMode.TimeAttack));
            
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(ShowMainMenu);
            
        if (confirmNameButton != null)
            confirmNameButton.onClick.AddListener(SaveFirstLaunchName);


        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(ShowExitConfirmation);
        }
        // Add these button setups in the SetupButtons method
        if (exitYesButton != null)
            exitYesButton.onClick.AddListener(QuitGame);

        if (exitNoButton != null)
            exitNoButton.onClick.AddListener(HideExitConfirmation);

        if (rateNowButton != null)
        {
            rateNowButton.onClick.AddListener(() => {
                rateUs(); // Call your existing rateUs function
                
            });
        }

        if (rateLaterButton != null)
            rateLaterButton.onClick.AddListener(HideRateUsPopup);

#if UNITY_IOS
        restorePurchasesButton.SetActive(true);
#else
        restorePurchasesButton.SetActive(false);
#endif


    }

    public void OnRestorePurchasesClicked()
    {
        MonetizationManager.Instance.RestorePurchases();
    }

    private void OnDestroy()
    {
        MonetizationManager.OnAdsRemoved -= HandleAdsRemoved;
    }

    private void HandleAdsRemoved(bool removed)
    {
        removeAdsButton.gameObject.SetActive(!removed);
    }

    void UpdateHighScore()
    {
        if (highScoreText != null)
        {
            int classicHighScore = PlayerPrefs.GetInt("HighScore_Classic", 0);
            int timeAttackHighScore = PlayerPrefs.GetInt("HighScore_TimeAttack", 0);
            Debug.Log("Classic: " + classicHighScore + " Time Attack: " + timeAttackHighScore);
            
            highScoreText.text = "Classic: " + classicHighScore + "\nTime Attack: " + timeAttackHighScore;
        }
    }

    private void rateUs()
    {
        AnalyticsManager.Instance.LogEvent("rate_now");
        PlayerPrefs.SetInt(HAS_RATED_KEY, 1);
        PlayerPrefs.Save();
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + Application.identifier);
#elif UNITY_IOS
    Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_APP_ID?action=write-review");
#else
    Application.OpenURL("https://your-website.com");
#endif
        HideRateUsPopup();
    }
    public void moreGames()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/developer?id=Watermelon+Lab+IND");
#endif
#if UNITY_IOS
        Application.OpenURL("https://apps.apple.com/us/developer/vishal-sanap/id1041732969");
#endif
    }
    private void privacyPolicy()
    {

        Application.OpenURL("https://www.watermelonlab.in/privacypolicy.html");
    }

    public void OpenModeSelection()
    {
        if (modeSelectPanel != null)
        {
            modeSelectPanel.SetActive(true);
        }
    }

    public void BuyRemoveAds()
    {
        if(MonetizationManager.Instance!= null)
        MonetizationManager.Instance.PurchaseRemoveAds();
    }


    public void OpenLeaderboard()
    {
        // Find and show the PlayFab leaderboard, including inactive ones
        PlayFabLeaderboardUI playFabLeaderboard = FindObjectOfType<PlayFabLeaderboardUI>(true);
        if (playFabLeaderboard != null)
        {
            playFabLeaderboard.ShowLeaderboard(); // Defaults to Classic mode
        }
        else
        {
            Debug.LogWarning("PlayFabLeaderboardUI not found in the scene. Make sure it is present and configured.");
        }
    }
    
    //public void OpenShop()
    //{
    //    // Find and show the shop UI
    //    ShopUIManager shopUI = FindObjectOfType<ShopUIManager>();
    //    if (shopUI != null)
    //    {
    //        shopUI.OpenShop();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("ShopUIManager not found in the scene. Make sure it is present and configured.");
    //    }
    //}
    
    public void StartGame(GameMode mode)
    {

        AnalyticsManager.Instance.LogEvent("game_start",new Parameter("mode", mode ==0 ? "classic":"time_attack"));
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

    public void openChangeNamePopup()
    {
        if (firstLaunchPanel!=null)
        {
            firstLaunchPanel.SetActive(true);
            if (firstLaunchNameInput != null)
            {
                firstLaunchNameInput.text = PlayerPrefs.GetString("PlayerDisplayName", "");
            }
        }
    }

    public void socialMediaBtnClick(int i)
    {
        switch (i)
        {
            case 1://discord
                Application.OpenURL("https://discord.gg/mB9SfaKebH");
                break;
            case 2://yt
                Application.OpenURL("https://www.youtube.com/@watermelonlabsindia");
                break;
            case 3://fb
                Application.OpenURL("https://www.facebook.com/watermelonlab");
                break;
            case 4://telegram
                Application.OpenURL("https://t.me/+GAJBj92BCG80ZTJl");
                break;
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
    
    //public void ShowMainMenu()
    //{
    //    if (settingsPanel != null)
    //        settingsPanel.SetActive(false);
    //    if (modeSelectPanel != null)
    //        modeSelectPanel.SetActive(false);
    //}
    
   
    
    public void SaveFirstLaunchName()
    {
        if (firstLaunchNameInput != null)
        {
            string newName = firstLaunchNameInput.text.Trim();
            if (!string.IsNullOrEmpty(newName))
            {
                PlayerPrefs.SetString("PlayerDisplayName", newName);
                PlayerPrefs.Save();
                
                // Update PlayFab display name
                PlayFabManager.Instance?.UpdateDisplayName(newName);
                
                // Hide first launch panel and show main menu
                if (firstLaunchPanel != null)
                    firstLaunchPanel.SetActive(false);
                if(!settingsPanel.activeSelf)
                ShowMainMenu();
            }
        }
    }
    
    public void ShowMainMenu()
    {
        if (firstLaunchPanel != null)
            firstLaunchPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        if (modeSelectPanel != null)
            modeSelectPanel.SetActive(false);
    }
    
    void CheckFirstLaunch()
    {
        if (!PlayerPrefs.HasKey("PlayerDisplayName"))
        {
            if (firstLaunchPanel != null)
            {
                firstLaunchPanel.SetActive(true);
                
                if (firstLaunchNameInput != null)
                    firstLaunchNameInput.text = "";
            }
        }
        else
        {
            ShowMainMenu();
        }
    }
}
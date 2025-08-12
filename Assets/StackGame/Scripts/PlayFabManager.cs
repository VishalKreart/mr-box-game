using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance { get; private set; }
    
    [Header("PlayFab Settings")]
    [Tooltip("Your PlayFab Title ID")]
    public string titleId = "";
    
    [Header("Leaderboard Settings")]
    public string classicLeaderboardStatistic = "classic_score";
    public string timeAttackLeaderboardStatistic = "time_attack_score";
    
    [Header("Debug")]
    public bool debugMode = true;
    
    private string playFabId;
    private bool isLoggedIn = false;
    
    // Property to access PlayFabId from other scripts
    public string PlayFabId { get { return playFabId; } }
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Set PlayFab Title ID
        if (!string.IsNullOrEmpty(titleId))
        {
            PlayFabSettings.staticSettings.TitleId = titleId;
        }
        else if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            Debug.LogError("PlayFab Title ID not set! Please set it in the PlayFabManager component or PlayFabSettings.");
        }
    }
    
    void Start()
    {
        // Automatically login with device ID
        LoginWithDeviceId();
    }
    
    #region Authentication
    
    public void LoginWithDeviceId()
    {
        LogDebug("Logging in with Device ID...");
        
        // Get a unique device ID or create one if it doesn't exist
        string deviceId = GetDeviceId();
        
        var request = new LoginWithCustomIDRequest
        {
            CustomId = deviceId,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }
    
    private void OnLoginSuccess(LoginResult result)
    {
        isLoggedIn = true;
        playFabId = result.PlayFabId;
        LogDebug("PlayFab Login Successful - PlayFabId: " + playFabId);
        // Check if this is a new player
        bool isNewPlayer = result.NewlyCreated;
        if (isNewPlayer)
        {
            // Set display name from PlayerPrefs if available, else fallback
            string savedName = PlayerPrefs.GetString("PlayerDisplayName", "");
            if (!string.IsNullOrEmpty(savedName))
            {
                UpdateDisplayName(savedName);
            }
            else
            {
                string randomName = "Player_" + UnityEngine.Random.Range(1000, 9999);
                UpdateDisplayName(randomName);
                PlayerPrefs.SetString("PlayerDisplayName", randomName);
                PlayerPrefs.Save();
            }
        }
    }
    
    private void OnLoginFailure(PlayFabError error)
    {
        isLoggedIn = false;
        LogError("PlayFab Login Failed: " + error.GenerateErrorReport());
    }
    
    public void UpdateDisplayName(string displayName)
    {
        if (!isLoggedIn)
        {
            LogError("Cannot update display name: Not logged in.");
            return;
        }
        
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };
        
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, 
            result => LogDebug("Display name updated to: " + result.DisplayName),
            error => LogError("Display name update failed: " + error.GenerateErrorReport()));
    }
    
    private string GetDeviceId()
    {
        string deviceId = PlayerPrefs.GetString("PlayFabDeviceId", "");
        
        if (string.IsNullOrEmpty(deviceId))
        {
            // Generate a new device ID
            deviceId = SystemInfo.deviceUniqueIdentifier;
            if (string.IsNullOrEmpty(deviceId) || deviceId == SystemInfo.unsupportedIdentifier)
            {
                // Fallback to a random GUID if device ID is not available
                deviceId = Guid.NewGuid().ToString();
            }
            
            // Save the device ID
            PlayerPrefs.SetString("PlayFabDeviceId", deviceId);
            PlayerPrefs.Save();
        }
        
        return deviceId;
    }
    
    #endregion
    
    #region Leaderboard
    
    public void SubmitScore(int score, GameMode gameMode)
    {
        if (!isLoggedIn)
        {
            LogError("Cannot submit score: Not logged in.");
            return;
        }
        
        string statisticName = gameMode == GameMode.Classic ? 
            classicLeaderboardStatistic : timeAttackLeaderboardStatistic;
        
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = statisticName,
                    Value = score
                }
            }
        };
        
        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => LogDebug("Successfully submitted score to " + statisticName + ": " + score),
            error => LogError("Score submission failed: " + error.GenerateErrorReport()));
    }
    
    public void GetLeaderboard(GameMode gameMode, Action<List<PlayerLeaderboardEntry>> onSuccess, Action<string> onFailure, int startPosition = 0, int maxResults = 10)
    {
        if (!isLoggedIn)
        {
            onFailure?.Invoke("Not logged in.");
            return;
        }
        
        string statisticName = gameMode == GameMode.Classic ? 
            classicLeaderboardStatistic : timeAttackLeaderboardStatistic;
        
        var request = new GetLeaderboardRequest
        {
            StatisticName = statisticName,
            StartPosition = startPosition,
            MaxResultsCount = maxResults,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true
            }
        };
        
        PlayFabClientAPI.GetLeaderboard(request,
            result => {
                LogDebug("Leaderboard retrieved successfully.");
                onSuccess?.Invoke(result.Leaderboard);
            },
            error => {
                string errorMessage = "Leaderboard retrieval failed: " + error.GenerateErrorReport();
                LogError(errorMessage);
                onFailure?.Invoke(errorMessage);
            });
    }
    
    public void GetLeaderboardAroundPlayer(GameMode gameMode, Action<List<PlayerLeaderboardEntry>> onSuccess, Action<string> onFailure, int maxResults = 10)
    {
        if (!isLoggedIn)
        {
            onFailure?.Invoke("Not logged in.");
            return;
        }
        
        string statisticName = gameMode == GameMode.Classic ? 
            classicLeaderboardStatistic : timeAttackLeaderboardStatistic;
        
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = statisticName,
            MaxResultsCount = maxResults,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true
            }
        };
        
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request,
            result => {
                LogDebug("Leaderboard around player retrieved successfully.");
                onSuccess?.Invoke(result.Leaderboard);
            },
            error => {
                string errorMessage = "Leaderboard around player retrieval failed: " + error.GenerateErrorReport();
                LogError(errorMessage);
                onFailure?.Invoke(errorMessage);
            });
    }
    
    #endregion
    
    #region Utility
    
    private void LogDebug(string message)
    {
        if (debugMode)
        {
            Debug.Log("[PlayFabManager] " + message);
        }
    }
    
    private void LogError(string message)
    {
        Debug.LogError("[PlayFabManager] " + message);
    }
    
    #endregion
}
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class PlayFabLeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leaderboardPanel;
    public Transform contentParent;
    public GameObject entryPrefab;
    public TextMeshProUGUI titleText;
    public Button classicButton;
    public Button timeAttackButton;
    public Button closeButton;
    public GameObject loadingIndicator;
    public TextMeshProUGUI statusText;
    
    [Header("Player Position")]
    public GameObject playerPositionContainer; // Container for the fixed player position entry
    public GameObject playerEntryPrefab; // Can be the same as entryPrefab or a different one
    
    [Header("UI Settings")]
    public Color highlightColor = new Color(1f, 0.92f, 0.016f, 1f); // Yellow
    public Color normalColor = Color.white;
    
    [Header("Trophy Sprites")]
    public Sprite goldTrophy;
    public Sprite silverTrophy;
    public Sprite bronzeTrophy;
    
    private GameMode currentMode = GameMode.Classic;
    private bool isRefreshing = false;
    
    void Start()
    {
        // Add listeners to buttons
        if (classicButton != null)
            classicButton.onClick.AddListener(() => SwitchLeaderboard(GameMode.Classic));
            
        if (timeAttackButton != null)
            timeAttackButton.onClick.AddListener(() => SwitchLeaderboard(GameMode.TimeAttack));
            
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseLeaderboard);
            
        // Hide the panel initially
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
            
        // Set initial status
        if (statusText != null)
            statusText.gameObject.SetActive(false);
            
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);
            
        // Make sure player position container is active
        if (playerPositionContainer != null)
            playerPositionContainer.SetActive(false);
    }
    
    public void ShowLeaderboard(GameMode mode = GameMode.Classic)
    {
        currentMode = mode;
        
        // Show the panel
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);
            
        // Show player position container
        if (playerPositionContainer != null)
            playerPositionContainer.SetActive(true);
            
        // Update UI
        UpdateButtonHighlights();
        UpdateTitleText();
        
        // Load leaderboard data
        RefreshLeaderboard();
    }
    
    public void CloseLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
            
        // Hide player position container
        if (playerPositionContainer != null)
            playerPositionContainer.SetActive(false);
    }
    
    public void SwitchLeaderboard(GameMode mode)
    {
        if (currentMode == mode || isRefreshing)
            return;
            
        currentMode = mode;
        
        // Update UI
        UpdateButtonHighlights();
        UpdateTitleText();
        
        // Refresh leaderboard data
        RefreshLeaderboard();
    }
    
    public void RefreshLeaderboard()
    {
        if (isRefreshing)
            return;
            
        StartCoroutine(RefreshLeaderboardCoroutine());
    }
    
    private IEnumerator RefreshLeaderboardCoroutine()
    {
        isRefreshing = true;
        
        // Show loading indicator
        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);
            
        if (statusText != null)
        {
            statusText.text = "Loading...";
            statusText.gameObject.SetActive(true);
        }
        
        // Clear existing entries
        ClearLeaderboardEntries();
        
        // Check if PlayFabManager is available
        if (PlayFabManager.Instance == null)
        {
            if (statusText != null)
            {
                statusText.text = "Error: PlayFab Manager not found!";
                statusText.gameObject.SetActive(true);
            }
            
            if (loadingIndicator != null)
                loadingIndicator.SetActive(false);
                
            isRefreshing = false;
            yield break;
        }
        
        // Get top leaderboard data
        PlayFabManager.Instance.GetLeaderboard(
            currentMode,
            OnLeaderboardSuccess,
            OnLeaderboardFailure
        );
        
        // Wait for a short time to ensure loading indicator is visible
        yield return new WaitForSeconds(0.5f);
        
        // Get player's position on the leaderboard
        PlayFabManager.Instance.GetLeaderboardAroundPlayer(
            currentMode,
            OnPlayerPositionSuccess,
            OnLeaderboardFailure,
            1 // Only need the player's entry
        );
        
        yield return new WaitForSeconds(0.5f);
        
        isRefreshing = false;
    }
    
    private void OnLeaderboardSuccess(List<PlayerLeaderboardEntry> leaderboard)
    {
        // Hide loading indicator only if we're not waiting for player position
        if (loadingIndicator != null && playerPositionContainer == null)
            loadingIndicator.SetActive(false);
            
        if (statusText != null)
            statusText.gameObject.SetActive(false);
            
        // Check if we have entries
        if (leaderboard == null || leaderboard.Count == 0)
        {
            if (statusText != null)
            {
                statusText.text = "No leaderboard entries found.";
                statusText.gameObject.SetActive(true);
            }
            return;
        }
        
        // Create entries for each leaderboard entry
        foreach (var entry in leaderboard)
        {
            CreateLeaderboardEntry(entry);
        }
    }
    
    private void OnPlayerPositionSuccess(List<PlayerLeaderboardEntry> leaderboard)
    {
        // Hide loading indicator
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);
        
        // Clear any existing player position entry
        if (playerPositionContainer != null)
        {
            foreach (Transform child in playerPositionContainer.transform)
            {
                Destroy(child.gameObject);
            }
            
            // Check if we have the player's entry
            if (leaderboard != null && leaderboard.Count > 0)
            {
                // Find the player's entry
                PlayerLeaderboardEntry playerEntry = null;
                foreach (var entry in leaderboard)
                {
                    if (PlayFabManager.Instance != null && entry.PlayFabId == PlayFabManager.Instance.PlayFabId)
                    {
                        playerEntry = entry;
                        break;
                    }
                }
                
                // If we found the player's entry, create it in the fixed container
                if (playerEntry != null)
                {
                    CreatePlayerFixedEntry(playerEntry);
                }
            }
        }
    }
    
    private void OnLeaderboardFailure(string errorMessage)
    {
        // Hide loading indicator
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);
            
        // Show error message
        if (statusText != null)
        {
            statusText.text = "Failed to load leaderboard. Please try again later.";
            statusText.gameObject.SetActive(true);
        }
        
        Debug.LogError("Leaderboard Error: " + errorMessage);
    }
    
    private void CreateLeaderboardEntry(PlayerLeaderboardEntry entry)
    {
        if (entryPrefab == null || contentParent == null)
            return;
            
        // Instantiate the entry prefab
        GameObject entryGO = Instantiate(entryPrefab, contentParent);
        
        // Get references to the TextMeshProUGUI components
        TextMeshProUGUI[] texts = entryGO.GetComponentsInChildren<TextMeshProUGUI>();
        Image trophyIcon = entryGO.transform.Find("TrophyIcon").GetComponent<Image>(); // Assuming you have an Image component for the trophy

        if (texts.Length >= 3 && trophyIcon != null)
        {
            // Default state
            trophyIcon.gameObject.SetActive(false);
            texts[0].gameObject.SetActive(true);

            // Rank & Trophies
            int rank = entry.Position + 1;
            texts[0].text = rank.ToString();

            if (rank <= 3)
            {
                trophyIcon.gameObject.SetActive(true);
                texts[0].gameObject.SetActive(false);
                if (rank == 1) trophyIcon.sprite = goldTrophy;
                else if (rank == 2) trophyIcon.sprite = silverTrophy;
                else if (rank == 3) trophyIcon.sprite = bronzeTrophy;
            }

            // Player Name
            string playerName = string.IsNullOrEmpty(entry.DisplayName) ? 
                "Player_" + entry.PlayFabId.Substring(0, 6) : entry.DisplayName;
            texts[1].text = playerName;

            // Score
            texts[2].text = entry.StatValue.ToString();

            // Highlight if this is the current player in the top 10
            Image background = entryGO.GetComponent<Image>();
            if (background != null)
            {
                bool isPlayer = PlayFabManager.Instance != null && entry.PlayFabId == PlayFabManager.Instance.PlayFabId;
                if (isPlayer && rank <= 10)
                {
                    background.color = highlightColor;
                    foreach (var text in texts)
                    {
                        text.fontStyle = FontStyles.Bold;
                    }
                }
                else
                {
                    background.color = normalColor;
                    foreach (var text in texts)
                    {
                        text.fontStyle = FontStyles.Normal;
                    }
                }
            }
        }
    }

    private void CreatePlayerFixedEntry(PlayerLeaderboardEntry entry)
    {
        if (playerEntryPrefab == null || playerPositionContainer == null)
            return;

        // Instantiate the entry prefab
        GameObject entryGO = Instantiate(playerEntryPrefab, playerPositionContainer.transform);

        // Get references to the TextMeshProUGUI components
        TextMeshProUGUI[] texts = entryGO.GetComponentsInChildren<TextMeshProUGUI>();
        Image trophyIcon = entryGO.transform.Find("TrophyIcon").GetComponent<Image>();

        if (texts.Length >= 3 && trophyIcon != null)
        {
            // Always hide trophy and show rank for the static entry
            trophyIcon.gameObject.SetActive(false);
            texts[0].gameObject.SetActive(true);

            // Rank - Always show for static entry
            texts[0].text = (entry.Position + 1).ToString();

            // Player Name - Always "You"
            texts[1].text = "You";

            // Score
            texts[2].text = entry.StatValue.ToString();

            // Highlight the current player's entry
            Image background = entryGO.GetComponent<Image>();
            if (background != null)
            {
                background.color = highlightColor;
            }

            // Make text bold
            foreach (var text in texts)
            {
                text.fontStyle = FontStyles.Bold;
            }
        }
    }

    private void ClearLeaderboardEntries()
    {
        if (contentParent == null)
            return;
            
        // Destroy all children of the content parent
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
    
    private void UpdateButtonHighlights()
    {
        if (classicButton != null && timeAttackButton != null)
        {
            // Get the button colors
            ColorBlock classicColors = classicButton.colors;
            ColorBlock timeAttackColors = timeAttackButton.colors;
            
            // Reset both buttons
            classicColors.normalColor = Color.white;
            timeAttackColors.normalColor = Color.white;
            
            // Highlight the selected button
            if (currentMode == GameMode.Classic)
                classicColors.normalColor = highlightColor;
            else
                timeAttackColors.normalColor = highlightColor;
                
            // Apply the colors
            classicButton.colors = classicColors;
            timeAttackButton.colors = timeAttackColors;
        }
    }
    
    private void UpdateTitleText()
    {
        if (titleText != null)
        {
            titleText.text = currentMode == GameMode.Classic ? 
                "Classic Mode Leaderboard" : "Time Attack Leaderboard";
        }
    }
}
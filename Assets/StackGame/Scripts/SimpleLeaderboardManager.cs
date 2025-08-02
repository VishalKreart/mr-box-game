using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SimpleLeaderboardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leaderboardPanel;
    public Transform contentParent;
    public GameObject entryPrefab;
    public TextMeshProUGUI titleText;
    public UnityEngine.UI.Button classicButton;
    public UnityEngine.UI.Button timeAttackButton;
    public UnityEngine.UI.Button closeButton;
    
    private GameMode currentMode = GameMode.Classic;
    
    void Start()
    {
        SetupButtons();
    }
    
    void SetupButtons()
    {
        if (classicButton != null)
            classicButton.onClick.AddListener(() => ShowMode(GameMode.Classic));
            
        if (timeAttackButton != null)
            timeAttackButton.onClick.AddListener(() => ShowMode(GameMode.TimeAttack));
            
        if (closeButton != null)
            closeButton.onClick.AddListener(HideLeaderboard);
    }
    
    public void ShowLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            ShowMode(currentMode);
        }
    }
    
    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }
    
    void ShowMode(GameMode mode)
    {
        currentMode = mode;
        
        if (titleText != null)
        {
            titleText.text = mode + " Leaderboard";
        }
        
        LoadAndDisplayScores(mode);
    }
    
    void LoadAndDisplayScores(GameMode mode)
    {
        // Clear existing entries
        if (contentParent != null)
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }
        
        // Get the key for this mode
        string key = mode == GameMode.Classic ? "ClassicLeaderboard" : "TimeAttackLeaderboard";
        
        // Load data from PlayerPrefs
        if (PlayerPrefs.HasKey(key))
        {
            string jsonData = PlayerPrefs.GetString(key);
            var wrapper = JsonUtility.FromJson<LeaderboardWrapper>(jsonData);
            if (wrapper != null && wrapper.entries != null)
            {
                // Sort by score (highest first)
                var sortedEntries = wrapper.entries.OrderByDescending(e => e.score).ToArray();
                
                // Create UI entries
                for (int i = 0; i < sortedEntries.Length; i++)
                {
                    CreateEntryUI(sortedEntries[i], i + 1);
                }
            }
        }
    }
    
    void CreateEntryUI(LeaderboardEntry entry, int rank)
    {
        if (entryPrefab != null && contentParent != null)
        {
            GameObject entryGO = Instantiate(entryPrefab, contentParent);
            
            // Set rank
            TextMeshProUGUI rankText = entryGO.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
            if (rankText != null)
            {
                rankText.text = "#" + rank;
                if (rank == 1) rankText.color = Color.yellow;
                else if (rank == 2) rankText.color = Color.gray;
                else if (rank == 3) rankText.color = new Color(0.8f, 0.5f, 0.2f);
            }
            
            // Set name
            TextMeshProUGUI nameText = entryGO.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = entry.playerName;
            }
            
            // Set score
            TextMeshProUGUI scoreText = entryGO.transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            if (scoreText != null)
            {
                scoreText.text = entry.score.ToString();
            }
            
            // Set date
            TextMeshProUGUI dateText = entryGO.transform.Find("DateText")?.GetComponent<TextMeshProUGUI>();
            if (dateText != null)
            {
                dateText.text = entry.date;
            }
        }
    }
    

} 
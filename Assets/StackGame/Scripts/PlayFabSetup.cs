using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to set up PlayFab integration in the scene.
/// This script is meant to be used in the Unity Editor only.
/// </summary>
public class PlayFabSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("PlayFab Manager Setup")]
    public string playFabTitleId = "YOUR_TITLE_ID_HERE";
    
    [Header("UI References")]
    public GameObject leaderboardPanel;
    public Transform contentParent;
    public GameObject entryPrefab;
    public TextMeshProUGUI titleText;
    public Button classicButton;
    public Button timeAttackButton;
    public Button closeButton;
    public Button onlineButton;
    
    [Header("UI Creation")]
    public Font defaultFont;
    public bool createLeaderboardUI = true;
    
    public void SetupPlayFabIntegration()
    {
        // Create PlayFabManager if it doesn't exist
        GameObject playFabManagerObj = GameObject.Find("PlayFabManager");
        if (playFabManagerObj == null)
        {
            playFabManagerObj = new GameObject("PlayFabManager");
            PlayFabManager playFabManager = playFabManagerObj.AddComponent<PlayFabManager>();
            playFabManager.titleId = playFabTitleId;
            Debug.Log("Created PlayFabManager GameObject");
        }
        else
        {
            Debug.Log("PlayFabManager already exists");
        }
        
        // Create PlayFabLeaderboardUI if it doesn't exist and is requested
        if (createLeaderboardUI)
        {
            GameObject playFabLeaderboardObj = GameObject.Find("PlayFabLeaderboardUI");
            if (playFabLeaderboardObj == null)
            {
                playFabLeaderboardObj = new GameObject("PlayFabLeaderboardUI");
                PlayFabLeaderboardUI leaderboardUI = playFabLeaderboardObj.AddComponent<PlayFabLeaderboardUI>();
                
                // Set references if available
                leaderboardUI.leaderboardPanel = leaderboardPanel;
                leaderboardUI.contentParent = contentParent;
                leaderboardUI.entryPrefab = entryPrefab;
                leaderboardUI.titleText = titleText;
                leaderboardUI.classicButton = classicButton;
                leaderboardUI.timeAttackButton = timeAttackButton;
                leaderboardUI.closeButton = closeButton;
                
                Debug.Log("Created PlayFabLeaderboardUI GameObject");
            }
            else
            {
                Debug.Log("PlayFabLeaderboardUI already exists");
            }
        }
        
        // Add online button to SimpleLeaderboardManager if it exists
        SimpleLeaderboardManager[] leaderboardManagers = FindObjectsOfType<SimpleLeaderboardManager>();
        if (leaderboardManagers.Length > 0 && onlineButton != null)
        {
            leaderboardManagers[0].onlineLeaderboardButton = onlineButton;
            Debug.Log("Assigned online button to SimpleLeaderboardManager");
        }
        
        Debug.Log("PlayFab integration setup complete!");
    }
    
    [ContextMenu("Setup PlayFab Integration")]
    private void ContextMenuSetup()
    {
        SetupPlayFabIntegration();
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayFabSetup))]
public class PlayFabSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        PlayFabSetup setupScript = (PlayFabSetup)target;
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Setup PlayFab Integration", GUILayout.Height(30)))
        {
            setupScript.SetupPlayFabIntegration();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This script helps you set up PlayFab integration in your scene. " +
                              "Fill in your PlayFab Title ID and assign UI references, then click the button above.", 
                              MessageType.Info);
    }
}
#endif
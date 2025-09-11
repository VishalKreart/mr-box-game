using UnityEngine;
using UnityEngine.UI;

public class SaveTowerDebugger : MonoBehaviour
{
    public SaveTowerUI saveTowerUI;
    public ScoreManager scoreManager;
    public Button testSaveButton;
    public Button testSuccessButton;
    public Button testFailButton;
    
    private void Start()
    {
        // Initialize references if not set in inspector
        if (saveTowerUI == null)
            saveTowerUI = FindObjectOfType<SaveTowerUI>();
            
        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();
        
        // Set up button click handlers
        if (testSaveButton != null)
            testSaveButton.onClick.AddListener(TestSaveTower);
            
        if (testSuccessButton != null)
            testSuccessButton.onClick.AddListener(TestPurchaseSuccess);
            
        if (testFailButton != null)
            testFailButton.onClick.AddListener(TestPurchaseFailed);
    }
    
    private void Update()
    {
        // Test with F1 key
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TestSaveTower();
        }
        // Test with F2 key
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            TestPurchaseSuccess();
        }
        // Test with F3 key
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            TestPurchaseFailed();
        }
    }
    
    private void TestSaveTower()
    {
        if (saveTowerUI != null && !saveTowerUI.IsWarningActive)
        {
            int currentScore = scoreManager != null ? scoreManager.GetScore() : 0;
            saveTowerUI.ShowSaveTowerUI(currentScore);
            Debug.Log($"[SaveTowerDebugger] Showing save tower UI with score: {currentScore}");
        }
    }
    
    private void TestPurchaseSuccess()
    {
        if (saveTowerUI != null && saveTowerUI.IsWarningActive)
        {
            Debug.Log("[SaveTowerDebugger] Simulating successful ad completion");
            // Call OnAdCompleted with success=true to simulate successful ad completion
            saveTowerUI.OnAdCompleted(true);
        }
    }
    
    private void TestPurchaseFailed()
    {
        if (saveTowerUI != null && saveTowerUI.IsWarningActive)
        {
            Debug.Log("[SaveTowerDebugger] Simulating failed/skipped ad");
            // Call OnAdCompleted with success=false to simulate ad failure or skip
            saveTowerUI.OnAdCompleted(false);
        }
    }
    
    private void OnDestroy()
    {
        // Clean up button listeners
        if (testSaveButton != null)
            testSaveButton.onClick.RemoveListener(TestSaveTower);
            
        if (testSuccessButton != null)
            testSuccessButton.onClick.RemoveListener(TestPurchaseSuccess);
            
        if (testFailButton != null)
            testFailButton.onClick.RemoveListener(TestPurchaseFailed);
    }
}

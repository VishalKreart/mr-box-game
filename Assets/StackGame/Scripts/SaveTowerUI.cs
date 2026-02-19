using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SaveTowerUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject saveTowerPanel;
    public Button purchaseButton;
    public Button cancelButton;
    public TextMeshProUGUI messageText;
    
    [Header("Tower Settings")]
    public int boxesToSpawn = 6;
    public float boxSpacing = 1f;
    
    private int currentScore;
    private bool isWarningActive = false;
    
    public bool IsWarningActive => isWarningActive;
    
    private void Start()
    {
        if (saveTowerPanel != null)
            saveTowerPanel.SetActive(false);
            
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
            
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelClicked);
    }
    
    public void ShowSaveTowerUI(int score)
    {
        if (saveTowerPanel == null || saveTowerPanel.activeSelf) 
            return;
            
        currentScore = score;
        isWarningActive = true;
        
        if (messageText != null)
            messageText.text = $"Save your tower and continue from {score} points?";
            
        saveTowerPanel.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }
    
    private void OnPurchaseClicked()
    {
        if (MonetizationManager.Instance == null)
        {
            Debug.LogError("MonetizationManager not found!");
            return;
        }
        
        SetButtonsInteractable(false);
        
        // Use the existing IAP purchase flow from MonetizationManager
        //MonetizationManager.Instance.PurchaseProduct("save_tower", (success) => {
        //    if (success)
        //    {
        //        StartCoroutine(ResetTowerAndContinue());
        //    }
        //    else
        //    {
        //        if (messageText != null)
        //            messageText.text = "Purchase failed. Please try again.";
        //        SetButtonsInteractable(true);
        //    }
        //});
    }
    
    public void OnAdCompleted(bool success)
    {
        if (success)
        {
            // If ad was successful, proceed with the purchase flow
            OnPurchaseClicked();
        }
        else
        {
            // If ad failed or was skipped, close the panel
            OnCancelClicked();
        }
    }
    
    private IEnumerator ResetTowerAndContinue()
    {
        ClosePanel();
        
        var boxSpawner = FindObjectOfType<BoxSpawner>();
        var scoreManager = FindObjectOfType<ScoreManager>();
        var cameraFollow = FindObjectOfType<CameraStackFollow>();
        
        if (boxSpawner == null || scoreManager == null || cameraFollow == null)
        {
            Debug.LogError("Required components not found!");
            yield break;
        }
        
        // Save the current score before resetting
        int savedScore = scoreManager.GetScore();
        
        // Clear existing boxes
        var boxes = GameObject.FindGameObjectsWithTag("Box");
        foreach (var box in boxes)
        {
            Destroy(box);
        }
        
        yield return null; // Wait for all boxes to be destroyed
        
        // Reset camera position
        if (cameraFollow != null)
        {
            // Camera will automatically adjust its position in LateUpdate
            // No need to manually reset it
        }
        
        // Spawn initial stack of 7 boxes by starting the spawner
        if (boxSpawner != null)
        {
            // Start the spawner coroutine which will handle spawning boxes
            boxSpawner.ResetSpawnerState();
        }
        
        // Restore the score
        if (scoreManager != null)
        {
            scoreManager.AddScore(savedScore - scoreManager.GetScore());
        }
        
        Time.timeScale = 1f; // Resume game
        isWarningActive = false;
    }
    
    private void OnCancelClicked()
    {
        ClosePanel();
        Time.timeScale = 1f;
        isWarningActive = false;
    }
    
    private void ClosePanel()
    {
        if (saveTowerPanel != null)
            saveTowerPanel.SetActive(false);
        SetButtonsInteractable(true);
    }
    
    private void SetButtonsInteractable(bool interactable)
    {
        if (purchaseButton != null)
            purchaseButton.interactable = interactable;
            
        if (cancelButton != null)
            cancelButton.interactable = interactable;
    }
}

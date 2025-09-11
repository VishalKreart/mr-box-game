using UnityEngine;

public class FallDetector : MonoBehaviour
{
    public SaveTowerUI saveTowerUI;
    public int minBoxesForSave = 5; // Minimum boxes before showing save option
    private bool gameOver = false;
    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameOver) return;
        
        if (other.CompareTag("Box"))
        {
            Debug.Log("Box entered fall detector");
            var box = other.GetComponent<BoxCollider2D>();
            if (box == null) return;
            
            // Check if we have enough boxes to show save option
            var boxes = GameObject.FindGameObjectsWithTag("Box");
            int placedBoxes = boxes.Length; // Simplified - assumes all boxes with tag are placed
            
            Debug.Log($"Found {placedBoxes} boxes, min required: {minBoxesForSave}");
            Debug.Log($"SaveTowerUI reference: {saveTowerUI != null}");
            
            if (placedBoxes >= minBoxesForSave && saveTowerUI != null && scoreManager != null)
            {
                Debug.Log("Showing save tower UI");
                saveTowerUI.ShowSaveTowerUI(scoreManager.GetScore());
            }
            else
            {
                Debug.Log("Not showing save UI because: " + 
                    $"placedBoxes >= minBoxesForSave: {placedBoxes >= minBoxesForSave}, " +
                    $"saveTowerUI != null: {saveTowerUI != null}, " +
                    $"scoreManager != null: {scoreManager != null}");
                
                // If no save option or not enough boxes, game over
                GameOver();
            }
        }
    }
    
    private void GameOver()
    {
        if (gameOver) return;
        gameOver = true;
        
        // Notify game manager or show game over UI
        var gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }
    
    public void ResetDetector()
    {
        gameOver = false;
    }
}

using UnityEngine;

public class TowerStabilityMonitor : MonoBehaviour
{
    public SaveTowerUI saveTowerUI;
    public float maxLeanAngle = 15f;
    public float checkInterval = 0.5f;
    public int minBoxesForWarning = 5;
    
    private float nextCheckTime;
    private bool warningActive;
    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        
        // Try to find SaveTowerUI if not assigned
        if (saveTowerUI == null)
        {
            saveTowerUI = FindObjectOfType<SaveTowerUI>();
            if (saveTowerUI != null)
            {
                Debug.Log("Found SaveTowerUI in TowerStabilityMonitor: " + saveTowerUI.name);
            }
            else
            {
                Debug.LogError("SaveTowerUI not found in scene by TowerStabilityMonitor!");
            }
        }
    }

    private void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            CheckTowerStability();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void CheckTowerStability()
    {
        if (saveTowerUI == null || scoreManager == null) return;
        
        // Count placed boxes using tags instead of Box component
        var boxes = GameObject.FindGameObjectsWithTag("Box");
        if (boxes.Length < minBoxesForWarning) return;
        
        // Calculate center of mass
        Vector2 centerOfMass = Vector2.zero;
        foreach (var box in boxes)
        {
            centerOfMass += (Vector2)box.transform.position;
        }
        centerOfMass /= boxes.Length;
        
        // Check if tower is leaning too much
        float maxDistance = 0f;
        foreach (var box in boxes)
        {
            float distance = Vector2.Distance(box.transform.position, centerOfMass);
            maxDistance = Mathf.Max(maxDistance, distance);
        }
        
        // If tower is unstable and we're not already showing a warning
        if (maxDistance > maxLeanAngle && !warningActive && !saveTowerUI.IsWarningActive)
        {
            saveTowerUI.ShowSaveTowerUI(scoreManager.GetScore());
            warningActive = true;
        }
        else if (maxDistance <= maxLeanAngle / 2f)
        {
            // Reset warning if tower becomes stable
            warningActive = false;
        }
    }
    
    public void ResetMonitor()
    {
        warningActive = false;
    }
}

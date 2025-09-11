using System.Collections;
using System.Linq;
using UnityEngine;

public class TowerRebalancer : MonoBehaviour
{
    [Header("Rebalance Settings")]
    [SerializeField] private float rebalanceForce = 50f;
    [SerializeField] private float rebalanceDuration = 3f;
    [SerializeField] private float gravityScale = 0.2f;
    [SerializeField] private float stabilizationTime = 2f;
    
    [Header("References")]
    [SerializeField] private BoxSpawner boxSpawner;
    private Rigidbody2D[] boxRigidbodies;
    private float originalGravityScale;
    private bool isRebalancing = false;
    private bool hasRebalanced = false;
    
    public bool IsRebalancing => isRebalancing;
    public bool HasRebalanced => hasRebalanced;
    
    private void Start()
    {
        if (boxSpawner == null)
            boxSpawner = FindObjectOfType<BoxSpawner>();
    }
    
    public void RebalanceTower()
    {
        if (isRebalancing) 
        {
            Debug.LogWarning("Rebalancing already in progress");
            return;
        }
        
        // Get all box rigidbodies
        var boxes = FindObjectsOfType<Rigidbody2D>()
            .Where(rb => rb.CompareTag("Box") && rb.bodyType != RigidbodyType2D.Static)
            .ToArray();
            
        if (boxes.Length == 0)
        {
            Debug.LogWarning("No boxes found for rebalancing!");
            return;
        }
        
        // Store original gravity scale from the first box
        originalGravityScale = boxes[0].gravityScale;
        
        isRebalancing = true;
        hasRebalanced = true;
        StartCoroutine(RebalanceCoroutine(boxes));
    }
    
    private IEnumerator RebalanceCoroutine(Rigidbody2D[] boxes)
    {
        Debug.Log("Starting tower rebalancing...");
        
        // Freeze all boxes
        foreach (var rb in boxes)
        {
            if (rb != null)
            FreezeBox(rb);
        }
        
        yield return null;
        
        // Sort boxes by Y position
        var sortedBoxes = boxes
            .Where(rb => rb != null)
            .OrderBy(b => b.transform.position.y)
            .ToList();
            
        if (sortedBoxes.Count == 0) 
        {
            Debug.LogWarning("No valid boxes found for rebalancing");
            isRebalancing = false;
            yield break;
        }
        
        // Stack all boxes directly above the first one
        Vector2 basePos = sortedBoxes[0].position;
        float height = sortedBoxes[0].GetComponent<Collider2D>().bounds.size.y;
        
        // First, reset all boxes to be perfectly aligned
        for (int i = 0; i < sortedBoxes.Count; i++)
        {
            if (sortedBoxes[i] == null) continue;
            
            // Calculate position for each box
            Vector2 newPos = new Vector2(basePos.x, basePos.y + (height * i));
            
            // Use transform.position for immediate positioning
            sortedBoxes[i].transform.position = newPos;
            sortedBoxes[i].transform.rotation = Quaternion.identity;
            
            // Reset physics properties
            sortedBoxes[i].velocity = Vector2.zero;
            sortedBoxes[i].angularVelocity = 0f;
        }
        
        // Wait one frame for physics to update
        yield return null;
        
        // Now apply physics-based positioning
        for (int i = 1; i < sortedBoxes.Count; i++)
        {
            if (sortedBoxes[i] == null || sortedBoxes[i-1] == null) continue;
            
            // Get the box below
            Rigidbody2D boxBelow = sortedBoxes[i - 1];
            Collider2D boxBelowCollider = boxBelow.GetComponent<Collider2D>();
            
            if (boxBelowCollider != null)
            {
                // Position the current box on top of the one below
                float currentHeight = sortedBoxes[i].GetComponent<Collider2D>().bounds.size.y;
                Vector2 newPos = new Vector2(
                    boxBelow.position.x,
                    boxBelow.position.y + (boxBelowCollider.bounds.size.y / 2) + (currentHeight / 2)
                );
                
                sortedBoxes[i].MovePosition(newPos);
                sortedBoxes[i].MoveRotation(0f);
            }
            
            // Small delay between each box to prevent physics glitches
            yield return new WaitForFixedUpdate();
        }
        
        // Apply gentle forces to settle the tower
        float timer = 0f;
        while (timer < rebalanceDuration)
        {
            foreach (var rb in sortedBoxes)
            {
                if (rb == null) continue;
                
                // Apply a small upward force to prevent sinking
                rb.AddForce(Vector2.up * rebalanceForce * Time.fixedDeltaTime);
                
                // Reduce angular velocity
                rb.angularVelocity *= 0.95f;
            }
            
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        // Reset gravity and unfreeze boxes
        foreach (var rb in sortedBoxes)
        {
            if (rb != null)
            {
                rb.gravityScale = originalGravityScale;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        
        // Allow physics to settle
        yield return new WaitForSeconds(stabilizationTime);
        
        Debug.Log("Tower rebalancing complete");
        
        // Stabilize the tower - convert List to array
        StartCoroutine(StabilizeTower(sortedBoxes.ToArray()));
    }
    
    private IEnumerator StabilizeTower(Rigidbody2D[] boxes)
    {
        Debug.Log("Stabilizing tower...");
        
        // Apply gentle stabilization
        float timer = 0f;
        while (timer < rebalanceDuration)
        {
            foreach (var rb in boxes)
            {
                if (rb == null) continue;
                
                // Reduce gravity effect
                rb.gravityScale = gravityScale;
                
                // Dampen horizontal movement
                rb.velocity = new Vector2(rb.velocity.x * 0.9f, rb.velocity.y);
                
                // Reduce rotation
                rb.angularVelocity *= 0.9f;
                
                // Small upward force to prevent sinking
                rb.AddForce(Vector2.up * rebalanceForce * Time.fixedDeltaTime);
            }
            
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        // Restore original physics
        foreach (var rb in boxes)
        {
            if (rb != null)
            {
                rb.gravityScale = originalGravityScale;
            }
        }
        
        // Allow physics to settle
        yield return new WaitForSeconds(stabilizationTime);
        
        Debug.Log("Tower stabilization complete");
        isRebalancing = false;
        
        // Notify BoxSpawner that it's safe to resume
        if (boxSpawner != null)
        {
            boxSpawner.ResetSpawnerState();
        }
    }
    
    private void FreezeBox(Rigidbody2D rb)
    {
        if (rb == null) return;
        
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    
    // Add a context menu item for testing
    [ContextMenu("Test Rebalance")]
    private void TestRebalance()
    {
        if (!isRebalancing)
            RebalanceTower();
    }
}

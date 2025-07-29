using UnityEngine;
using System.Collections;

public class BoxSpawner : MonoBehaviour
{
    [Header("Box System")]
    public GameObject[] boxPrefabs; // Legacy support
    public BoxVariations boxVariations; // New system
    
    [Header("Spawn Settings")]
    public bool useDifficultyProgression = true;
    public bool useWeightedSpawning = true;
    
    private GameObject currentBox;
    private bool isSpawning = false;
    private bool canDrop = true;
    private bool boxLanded = false;
    public bool externalDropControl = false; // Set true to disable internal input handling

    public GameObject GetCurrentBox() { return currentBox; }

    void Start()
    {
        if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();
        StartCoroutine(SpawnNewBoxCoroutine());
    }

    void Update()
    {
        if (externalDropControl) return;
        if (currentBox == null || isSpawning || !canDrop) return;

        // If DraggableBox is present and box is in Spawned state, do not allow auto-drop
        DraggableBox drag = currentBox.GetComponent<DraggableBox>();
        BoxState boxState = currentBox.GetComponent<BoxState>();
        if (drag != null && boxState != null && boxState.state == BoxState.State.Spawned)
        {
            return;
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
        {
            Rigidbody2D rb = currentBox.GetComponent<Rigidbody2D>();
            //BoxState boxState = currentBox.GetComponent<BoxState>(); // already defined above
            rb.gravityScale = 1f;
            canDrop = false;
            boxLanded = false;
            if (boxState != null) boxState.state = BoxState.State.Falling;
            BoxLandingDetector detector = currentBox.AddComponent<BoxLandingDetector>();
            detector.spawner = this;
            detector.boxState = boxState;
            StartCoroutine(WaitForBoxToLand(rb, boxState));
        }
    }

    public void ExternalDropBox()
    {
        if (currentBox == null || isSpawning || !canDrop) return;
        Rigidbody2D rb = currentBox.GetComponent<Rigidbody2D>();
        BoxState boxState = currentBox.GetComponent<BoxState>();
        rb.gravityScale = 1f;
        canDrop = false;
        boxLanded = false;
        if (boxState != null) boxState.state = BoxState.State.Falling;
        BoxLandingDetector detector = currentBox.AddComponent<BoxLandingDetector>();
        detector.spawner = this;
        detector.boxState = boxState;
        StartCoroutine(WaitForBoxToLand(rb, boxState));
    }

    IEnumerator SpawnNewBoxCoroutine()
    {
        isSpawning = true;
        
        // Choose which box to spawn
        GameObject boxToSpawn = GetNextBox();
        
        currentBox = Instantiate(boxToSpawn, transform.position, Quaternion.identity);
        currentBox.GetComponent<Rigidbody2D>().gravityScale = 0f;
        BoxState boxState = currentBox.GetComponent<BoxState>();
        if (boxState != null) boxState.state = BoxState.State.Spawned;
        
        // Apply visual enhancements (borders, colors, etc.)
        if (boxVariations != null)
        {
            boxVariations.ApplyVisualEnhancements(currentBox);
        }
        
        isSpawning = false;
        canDrop = true;
        yield return null;
    }
    
    private GameObject GetNextBox()
    {
        // Use new BoxVariations system if available
        if (boxVariations != null)
        {
            if (useDifficultyProgression)
            {
                int currentScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetScore() : 0;
                return boxVariations.GetBoxByDifficulty(currentScore);
            }
            else if (useWeightedSpawning)
            {
                return boxVariations.GetWeightedRandomBox();
            }
            else
            {
                return boxVariations.GetRandomBox();
            }
        }
        
        // Fallback to legacy system
        if (boxPrefabs.Length > 0)
        {
            int index = Random.Range(0, boxPrefabs.Length);
            return boxPrefabs[index];
        }
        
        Debug.LogError("No box prefabs available!");
        return null;
    }

    IEnumerator WaitForBoxToLand(Rigidbody2D rb, BoxState boxState)
    {
        // Wait until the box is nearly stopped and has landed
        while (!(boxLanded && Mathf.Abs(rb.velocity.y) < 0.05f))
        {
            yield return null;
        }
        if (boxState != null) boxState.state = BoxState.State.Sleep;
        
        // Add score based on box type
        int pointsToAdd = GetBoxPoints(currentBox);
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(pointsToAdd);
        
        currentBox = null;
        yield return StartCoroutine(SpawnNewBoxCoroutine());
    }
    
    private int GetBoxPoints(GameObject box)
    {
        // Check if using new system
        if (boxVariations != null)
        {
            BoxVariation variation = boxVariations.GetBoxVariation(box);
            if (variation != null)
            {
                return variation.pointValue;
            }
        }
        
        // Default points
        return 1;
    }

    // Called by BoxLandingDetector when the box lands
    public void NotifyBoxLanded()
    {
        boxLanded = true;
    }
}

// Helper component to detect when a box lands
public class BoxLandingDetector : MonoBehaviour
{
    [HideInInspector]
    public BoxSpawner spawner;
    [HideInInspector]
    public BoxState boxState;
    private bool hasLanded = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded)
        {
            hasLanded = true;
            spawner.NotifyBoxLanded();
            if (boxState != null) boxState.state = BoxState.State.Sleep;
        }
    }
}

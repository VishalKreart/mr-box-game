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
    private bool stopSpawning = false; // For Time Attack mode

    public GameObject GetCurrentBox() { return currentBox; }

    void Start()
    {
        if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();
        StartCoroutine(SpawnNewBoxCoroutine());
    }

    [Header("Safe Area")]
    public float SafeAreaVerticalOffset = 0f;

    void AdjustForSafeArea()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        // Get the top of the screen in world coordinates
        float cameraZ = transform.position.z - Camera.main.transform.position.z;
        Vector3 screenTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height, cameraZ));

        // Get the top of the safe area in world coordinates
        Rect safeArea = Screen.safeArea;
        Vector3 safeAreaTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, safeArea.yMax, cameraZ));

        // Calculate the offset
        float offsetY = screenTop.y - safeAreaTop.y + SafeAreaVerticalOffset;

        // Apply the offset to the spawner's position
        transform.position = new Vector3(transform.position.x, transform.position.y - offsetY, transform.position.z);
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
        
        // Trigger falling animation
        BoxAnimationController animController = currentBox.GetComponent<BoxAnimationController>();
        if (animController != null)
        {
            animController.ForceFallingState();
        }
        
        BoxLandingDetector detector = currentBox.AddComponent<BoxLandingDetector>();
        detector.spawner = this;
        detector.boxState = boxState;
        StartCoroutine(WaitForBoxToLand(rb, boxState));
    }

    IEnumerator SpawnNewBoxCoroutine()
    {
        // Only spawn if not stopped (for Time Attack mode)
        if (stopSpawning) yield break;
    
        isSpawning = true;
        
        // Choose which box to spawn
        GameObject boxToSpawn = GetNextBox();
        
        currentBox = Instantiate(boxToSpawn, transform.position, Quaternion.identity);
        
        // Ensure the box maintains its original scale
        Vector3 originalScale = boxToSpawn.transform.localScale;
        currentBox.transform.localScale = originalScale;
        
        currentBox.GetComponent<Rigidbody2D>().gravityScale = 0f;
        BoxState boxState = currentBox.GetComponent<BoxState>();
        if (boxState != null) boxState.state = BoxState.State.Spawned;
        
        // Apply visual enhancements (borders, colors, etc.)
        if (boxVariations != null)
        {
            boxVariations.ApplyVisualEnhancements(currentBox);
        }
        
        // Ensure scale is preserved after visual enhancements
        currentBox.transform.localScale = originalScale;
        
        // Initialize animation controller
        BoxAnimationController animController = currentBox.GetComponent<BoxAnimationController>();
        if (animController != null)
        {
            // Disable all visual effects to reduce shaking
            animController.usePulseOnSpawn = false;
            animController.useShakeOnSettle = false;
            animController.ForceSpawnedState();
        }
        
        // Disable facial expression effects (keep blinking for expressions)
        BoxFacialExpressions facialExpressions = currentBox.GetComponent<BoxFacialExpressions>();
        if (facialExpressions != null)
        {
            facialExpressions.useShakeOnScared = false;
            facialExpressions.useTearsOnFalling = false;
            facialExpressions.useBlinking = true; // Keep blinking for facial expressions
        }
        
        isSpawning = false;
        canDrop = true;
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
        while (!(boxLanded && rb != null && Mathf.Abs(rb.velocity.y) < 0.05f))
        {
            // Check if the box was destroyed
            if (rb == null || currentBox == null)
            {
                Debug.Log("Box was destroyed, stopping coroutine");
                currentBox = null;
                isSpawning = false;
                yield break;
            }
            yield return null;
        }
        
        // Check again before proceeding
        if (rb == null || currentBox == null)
        {
            Debug.Log("Box was destroyed before landing");
            currentBox = null;
            isSpawning = false;
            yield break;
        }
        
        if (boxState != null) boxState.state = BoxState.State.Sleep;
        
        // Ensure settled animation is triggered
        if (currentBox != null)
        {
            BoxAnimationController animController = currentBox.GetComponent<BoxAnimationController>();
            if (animController != null)
            {
                animController.ForceSettledState();
            }
        }
        
        // Add score based on box type
        int pointsToAdd = GetBoxPoints(currentBox);
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(pointsToAdd);
        
        // Notify Time Attack Manager (if in Time Attack mode)
        TimeAttackManager timeAttackManager = FindObjectOfType<TimeAttackManager>();
        if (timeAttackManager != null && timeAttackManager.IsTimeAttackMode())
        {
            timeAttackManager.OnBoxLanded();
        }
        
        currentBox = null;
        
        // Check if we should continue spawning (for Time Attack mode)
        if (!stopSpawning)
        {
            yield return StartCoroutine(SpawnNewBoxCoroutine());
        }
        else
        {
            isSpawning = false;
        }
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
    
    // Stop spawning boxes (for Time Attack mode)
    public void StopSpawning()
    {
        stopSpawning = true;
        Debug.Log("Box spawning stopped");
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
            
            // Trigger settled animation
            BoxAnimationController animController = GetComponent<BoxAnimationController>();
            if (animController != null)
            {
                animController.ForceSettledState();
            }
        }
    }
}

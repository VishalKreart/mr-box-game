using UnityEngine;
using System.Collections;

public class BoxSpawner : MonoBehaviour
{

    [Header("Box Movement")]
    public float moveSpeed = 3f;          // Speed of horizontal movement
    public float moveRange = 2f;          // How far left/right the box can move
    private bool movingRight = true;      // Current movement direction
    private float baseXPosition;   


    [Header("Box System")]
    public GameObject[] boxPrefabs; // Legacy support
    public BoxVariations boxVariations; // New system
    
    [Header("Spawn Settings")]
    public bool useDifficultyProgression = true;
    public bool useWeightedSpawning = true;
    
    [Header("References")]
    public TowerRebalancer towerRebalancer; // Assign in inspector
    
    [Header("Spawning")]
    public float spawnDelayAfterRebalance = 1f;
    
    private GameObject currentBox;
    private bool isSpawning = false;
    private bool canDrop = true;
    private bool boxLanded = false;
    //public bool externalDropControl = false; // Set true to disable internal input handling
    private bool stopSpawning = false; // For Time Attack mode
    private bool isWaitingForRebalance = false;

    public GameObject GetCurrentBox() { return currentBox; }


    private bool _externalDropControl;
    public bool externalDropControl
    {
        get => _externalDropControl;
        set
        {
            Debug.Log($"externalDropControl changed from {_externalDropControl} to {value}");
            _externalDropControl = value;
        }
    }


    //private void OnEnable()
    //{
    //    CameraStackFollow.OnCameraMoving += HandleCameraMoving;
    //}

    //private void OnDisable()
    //{
    //    CameraStackFollow.OnCameraMoving -= HandleCameraMoving;
    //}
    //private void HandleCameraMoving(bool moving)
    //{
    //    cameraIsMoving = moving;
    //}


    [ContextMenu("Reset Spawner State")]
    public void ResetSpawnerState()
    {
        // Reset spawner state
        _externalDropControl = false;
        externalDropControl = false;
        canDrop = true;
        isSpawning = false;
        isWaitingForRebalance = false;
        stopSpawning = false;
        boxLanded = false;
        
        // Stop any running coroutines and start fresh
        StopAllCoroutines();
        
        // Start the appropriate coroutine based on what's available
        if (this != null && this.isActiveAndEnabled)
        {
            StartCoroutine(SpawnNewBoxCoroutine());
        }
        
        Debug.Log("Spawner state reset");
    }


    [ContextMenu("Reset Spawner")]
    public void DebugResetSpawner()
    {
        canDrop = true;
        isSpawning = false;
        externalDropControl = false;
        Debug.Log("Spawner reset. " +
                 $"canDrop: {canDrop}, " +
                 $"isSpawning: {isSpawning}, " +
                 $"externalDropControl: {externalDropControl}");
    }


    void Start()
    {

        if (ScoreManager.Instance != null) ScoreManager.Instance.ResetScore();
        
        StartCoroutine(SpawnNewBoxCoroutine());

        baseXPosition = transform.position.x;
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


    // Add this field at the class level
    private float lastDropTime;

    private void MoveBoxHorizontally()
    {
        if (currentBox == null) return;
        
        var boxState = currentBox.GetComponent<BoxState>();
        if (boxState == null || boxState.state != BoxState.State.Spawned) return;
        
        // Calculate target position
        float currentX = currentBox.transform.position.x;
        float targetX = movingRight ? baseXPosition + moveRange : baseXPosition - moveRange;
        
        // Move towards target
        float newX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
        currentBox.transform.position = new Vector3(newX, 
                                                currentBox.transform.position.y, 
                                                currentBox.transform.position.z);
        
        // Change direction if we've reached the target
        if (Mathf.Abs(newX - targetX) < 0.01f)
        {
            movingRight = !movingRight;
        }
    }
    void Update()
    {
        //if (cameraIsMoving)
        //    return;
        

        if (externalDropControl)
        {
            //Debug.Log($"External drop control is active. Time: {Time.time}");
            return;
        }

        if (currentBox == null)
        {
            Debug.Log("Current box is null");
            return;
        }

        if (isSpawning)
        {
            Debug.Log("Already spawning a box");
            return;
        }

        if (!canDrop)
        {
            // Add detailed debug info
            //var boxState1 = currentBox?.GetComponent<BoxState>();
            //Debug.Log($"Cannot drop box. " +
            //         $"canDrop: {canDrop}, " +
            //         $"isSpawning: {isSpawning}, " +
            //         $"Box State: {boxState1?.state}, " +
            //         $"Time: {Time.time}");
            return;
        }

        if (isWaitingForRebalance)
        {
            Debug.Log("Waiting for rebalance");
            return;
        }

        // Temporary auto-reset for testing
        if (_externalDropControl && Time.time - lastDropTime > 3f) // Reset after 3 seconds if stuck
        {
            Debug.LogWarning("Auto-resetting externalDropControl");
            _externalDropControl = false;
        }

        if (_externalDropControl)
        {
            Debug.Log("Blocked by externalDropControl");
            return;
        }

        if (externalDropControl)
        {
            Debug.Log("External drop control is active");
            return;
        }

        if (currentBox == null)
        {
            Debug.Log("Current box is null");
            return;
        }

        if (isSpawning)
        {
            Debug.Log("Already spawning a box");
            return;
        }

        if (!canDrop)
        {
            Debug.Log("Cannot drop box now");
            return;
        }

        DraggableBox drag = currentBox.GetComponent<DraggableBox>();
        BoxState boxState = currentBox.GetComponent<BoxState>();

        if (drag != null && boxState != null && boxState.state == BoxState.State.Spawned)
        {
            Debug.Log($"Box is in Spawned state - Draggable: {drag != null}, State: {boxState.state}");
            return;
        }
        MoveBoxHorizontally();

        bool inputDetected = false;
#if UNITY_EDITOR
        inputDetected = Input.GetMouseButtonDown(0);
#elif UNITY_ANDROID || UNITY_IOS
    inputDetected = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#endif

        if (inputDetected)
        {
            lastDropTime = Time.time;
            Debug.Log("Input detected, attempting to drop box...");
            Rigidbody2D rb = currentBox.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("No Rigidbody2D found on current box!");
                return;
            }

            rb.gravityScale = 1f;
            canDrop = false;
            boxLanded = false;

            if (boxState != null)
            {
                boxState.state = BoxState.State.Falling;
                Debug.Log($"Box state set to: {boxState.state}");
            }
            else
            {
                Debug.LogError("No BoxState component found on current box!");
            }

            BoxLandingDetector detector = currentBox.AddComponent<BoxLandingDetector>();
            if (detector != null)
            {
                detector.spawner = this;
                detector.boxState = boxState;
                Debug.Log("Added BoxLandingDetector");
            }
            else
            {
                Debug.LogError("Failed to add BoxLandingDetector!");
            }

            StartCoroutine(WaitForBoxToLand(rb, boxState));
        }
        else
        {
            //Debug.Log("No valid input detected");
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

    public void OnTowerRebalancing()
    {
        // Called when tower rebalancing starts
        isWaitingForRebalance = true;
        canDrop = false;
    }

    public void OnTowerRebalanced()
    {
        // Called when tower rebalancing is complete
        StartCoroutine(ResumeAfterRebalance());
    }

    private IEnumerator ResumeAfterRebalance()
    {
        Debug.Log("Waiting " + spawnDelayAfterRebalance + " seconds after rebalance...");
        yield return new WaitForSeconds(spawnDelayAfterRebalance);
        
        isWaitingForRebalance = false;
        canDrop = true;
        
        // Make sure we don't have a current box before spawning a new one
        if (currentBox == null && !isSpawning)
        {
            StartCoroutine(SpawnNewBoxCoroutine());
        }
    }

    IEnumerator SpawnNewBoxCoroutine()
    {
        //while (cameraIsMoving)
        //    yield return null;

        // Only spawn if not stopped (for Time Attack mode)
        if (stopSpawning) yield break;
    
        isSpawning = true;
        
        // Reset spawner position to center when spawning a new box
        // transform.position = new Vector3(0, transform.position.y, transform.position.z);
        // movingRight = true; // Reset direction

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
            //animController.usePulseOnSpawn = false;
            //animController.useShakeOnSettle = false;
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
        float timeout = 5f; // 5 second timeout
        float startTime = Time.time;
        
        // Wait until the box is nearly stopped and has landed
        while (!(boxLanded && rb != null && Mathf.Abs(rb.velocity.y) < 0.05f))
        {
            // Check if the box was destroyed
            if (rb == null || currentBox == null)
            {
                Debug.Log("Box was destroyed, stopping coroutine");
                currentBox = null;
                isSpawning = false;
                canDrop = true;
                yield break;
            }
            
            // Check for timeout
            if (Time.time - startTime > timeout)
            {
                Debug.LogWarning($"Box landing timed out after {timeout} seconds. Forcing box to land.");
                boxLanded = true;
                break;
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

    IEnumerator SpawnBoxes()
    {
        // Implement your spawning logic here
        yield return null;
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

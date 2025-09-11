using UnityEngine;

public class BoxAnimationController : MonoBehaviour
{
    [Header("Animation States")]
    public string spawnedAnimationTrigger = "Spawned";
    public string fallingAnimationTrigger = "Falling";
    public string smileAnimationTrigger = "Smile"; // Using smile animation instead of settled
    
    [Header("Animation Settings")]
    public float spawnAnimationDuration = 0.5f;
    public float fallAnimationSpeed = 1.2f;
    public float settleAnimationDuration = 0.3f;
    
    [Header("Visual Effects")]
    public bool usePulseOnSpawn = false; // Disabled to reduce shaking
    public bool useShakeOnSettle = false; // Disabled to reduce shaking
    public float pulseIntensity = 0.1f;
    public float shakeIntensity = 0.05f;
    
    private Animator animator;
    private BoxState boxState;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        boxState = GetComponentInParent<BoxState>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
        
        // Don't capture scale here - it might be 0
        // We'll capture it when needed
    }
    
    void Update()
    {
        if (boxState == null) return;
        
        // Update animation based on current state
        switch (boxState.state)
        {
            case BoxState.State.Spawned:
                // Spawned animation is already playing
                break;
                
            case BoxState.State.Falling:
                PlayFallingAnimation();
                break;
                
            case BoxState.State.Sleep:
                PlaySettledAnimation();
                break;
        }
    }
    
    public void PlaySpawnedAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(spawnedAnimationTrigger);
        }
        
        if (usePulseOnSpawn)
        {
            StartCoroutine(PulseAnimation());
        }
        
        // Trigger scared facial expression
        //BoxFacialExpressions facialExpressions = GetComponent<BoxFacialExpressions>();
        //if (facialExpressions != null)
        //{
        //    facialExpressions.ForceScaredExpression();
        //}
    }
    
    public void PlayFallingAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(fallingAnimationTrigger);
            animator.speed = fallAnimationSpeed;
        }
        
        // Trigger screaming facial expression
        //BoxFacialExpressions facialExpressions = GetComponent<BoxFacialExpressions>();
        //if (facialExpressions != null)
        //{
        //    facialExpressions.ForceScreamingExpression();
        //}
    }
    
    public void PlaySettledAnimation()
    {
        if (animator != null)
        {
            // Use the smile animation trigger directly
            animator.SetTrigger(smileAnimationTrigger);
        }
        
        // Trigger happy facial expression
        //BoxFacialExpressions facialExpressions = GetComponent<BoxFacialExpressions>();
        //if (facialExpressions != null)
        //{
        //    facialExpressions.ForceHappyExpression();
        //}
    }
    
    private System.Collections.IEnumerator PulseAnimation()
    {
        // Capture the current scale (should be correct by now)
        Vector3 currentScale = transform.localScale;
        
        if (currentScale == Vector3.zero)
        {
            // If scale is still 0, use a default scale
            currentScale = Vector3.one;
            transform.localScale = currentScale;
        }
        
        float elapsed = 0f;
        Vector3 startScale = currentScale * 0.8f;
        Vector3 endScale = currentScale * 1.2f;
        
        transform.localScale = startScale;
        
        while (elapsed < spawnAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / spawnAnimationDuration;
            
            // Smooth pulse effect
            float pulse = Mathf.Sin(progress * Mathf.PI * 4) * pulseIntensity + 1f;
            transform.localScale = Vector3.Lerp(startScale, endScale, progress) * pulse;
            
            yield return null;
        }
        
        transform.localScale = currentScale;
    }
    
    private System.Collections.IEnumerator ShakeAnimation()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        
        while (elapsed < settleAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / settleAnimationDuration;
            
            // Shake effect
            float shakeX = Mathf.Sin(progress * Mathf.PI * 8) * shakeIntensity * (1f - progress);
            float shakeY = Mathf.Cos(progress * Mathf.PI * 6) * shakeIntensity * (1f - progress);
            
            transform.position = startPos + new Vector3(shakeX, shakeY, 0);
            
            yield return null;
        }
        
        transform.position = startPos;
    }
    
    // Public methods for external control
    public void ForceSpawnedState()
    {
        if (boxState != null)
        {
            boxState.state = BoxState.State.Spawned;
        }
        PlaySpawnedAnimation();
    }
    
    public void ForceFallingState()
    {
        if (boxState != null)
        {
            boxState.state = BoxState.State.Falling;
        }
        PlayFallingAnimation();
    }
    
    public void ForceSettledState()
    {
        if (boxState != null)
        {
            boxState.state = BoxState.State.Sleep;
        }
        PlaySettledAnimation();
        
    }
    
    // Method to reset animations
    public void ResetAnimations()
    {
        if (animator != null)
        {
            animator.ResetTrigger(spawnedAnimationTrigger);
            animator.ResetTrigger(fallingAnimationTrigger);
            animator.ResetTrigger(smileAnimationTrigger); // Reset smile animation trigger
            animator.speed = 1f;
        }
        
        transform.localScale = originalScale;
        transform.position = originalPosition;
    }
} 
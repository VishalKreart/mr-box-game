using UnityEngine;
using System.Collections;

public class BoxFacialExpressions : MonoBehaviour
{
    [Header("Facial Components")]
    public SpriteRenderer eyesRenderer;
    public SpriteRenderer mouthRenderer;
    
    [Header("Eye Expressions")]
    public Sprite scaredEyes;      // When at spawner (scared of height)
    public Sprite screamingEyes;   // When falling
    public Sprite relaxedEyes;     // When settled
    
    [Header("Mouth Expressions")]
    public Sprite scaredMouth;     // When at spawner
    public Sprite screamingMouth;  // When falling
    public Sprite relaxedMouth;    // When settled
    
    [Header("Animation Settings")]
    public float expressionChangeSpeed = 0.2f;
    public bool useBlinking = true;
    public float blinkInterval = 2f;
    public float blinkDuration = 0.1f;
    
    [Header("Happy Winking Settings")]
    public bool useHappyWink = true; // Happy wink animation for relaxed state
    public float happyWinkMinInterval = 2f; // Minimum time between winks
    public float happyWinkMaxInterval = 4f; // Maximum time between winks
    public float happyWinkDuration = 0.2f; // How long each wink lasts
    public float happyWinkPause = 0.5f; // Pause between double winks
    
    [Header("Visual Effects")]
    public bool useShakeOnScared = false; // Disabled to reduce shaking
    public bool useTearsOnFalling = false; // Disabled to reduce shaking
    public float shakeIntensity = 0.02f;
    public GameObject tearPrefab;
    
    private BoxState boxState;
    private BoxAnimationController animController;
    private Vector3 originalEyesPosition;
    private Vector3 originalMouthPosition;
    private bool isBlinking = false;
    private Coroutine blinkCoroutine;
    private Coroutine shakeCoroutine;
    private Coroutine tearCoroutine;
    private Coroutine happyWinkCoroutine;
    
    void Start()
    {
        boxState = GetComponent<BoxState>();
        animController = GetComponent<BoxAnimationController>();
        
        // Set main box sprite to lowest sorting order
        SpriteRenderer mainRenderer = GetComponent<SpriteRenderer>();
        if (mainRenderer != null)
        {
            mainRenderer.sortingOrder = 0;
        }
        
        // Auto-position facial features correctly
        if (eyesRenderer != null)
        {
            eyesRenderer.transform.localPosition = new Vector3(0, 0.3f, 0);
            originalEyesPosition = eyesRenderer.transform.localPosition;
            // Set sorting order to render above main box sprite
            eyesRenderer.sortingOrder = 2;
        }
        if (mouthRenderer != null)
        {
            mouthRenderer.transform.localPosition = new Vector3(0, -0.3f, 0);
            originalMouthPosition = mouthRenderer.transform.localPosition;
            // Set sorting order to render above main box sprite
            mouthRenderer.sortingOrder = 2;
        }
        
        // Start with scared expression (at spawner)
        SetScaredExpression();
        
        // Start blinking if enabled
        if (useBlinking)
        {
            blinkCoroutine = StartCoroutine(BlinkRoutine());
        }
        
        // Start happy wink animation if enabled
        if (useHappyWink)
        {
            happyWinkCoroutine = StartCoroutine(HappyWinkAnimation());
        }
    }
    
    void Update()
    {
        if (boxState == null) return;
        
        // Update facial expression based on box state
        switch (boxState.state)
        {
            case BoxState.State.Spawned:
                SetScaredExpression();
                break;
                
            case BoxState.State.Falling:
                SetScreamingExpression();
                break;
                
            case BoxState.State.Sleep:
                ForceHappyExpression();
                break;
        }
    }
    
    public void SetScaredExpression()
    {
        if (eyesRenderer != null && scaredEyes != null)
        {
            eyesRenderer.sprite = scaredEyes;
        }
        
        if (mouthRenderer != null && scaredMouth != null)
        {
            mouthRenderer.sprite = scaredMouth;
        }
        
        // Start scared shake effect
        if (useShakeOnScared)
        {
            StopShake();
            shakeCoroutine = StartCoroutine(ScaredShake());
        }
        
        // Stop tears if falling
        StopTears();
        StopHappyWink();
    }
    
    public void SetScreamingExpression()
    {
        if (eyesRenderer != null && screamingEyes != null)
        {
            eyesRenderer.sprite = screamingEyes;
        }
        
        if (mouthRenderer != null && screamingMouth != null)
        {
            mouthRenderer.sprite = screamingMouth;
        }
        
        // Stop scared shake
        StopShake();
        StopHappyWink();
        
        // Start tears effect
        if (useTearsOnFalling)
        {
            StopTears();
            tearCoroutine = StartCoroutine(TearsEffect());
        }
    }
    
    public void SetRelaxedExpression()
    {
        if (eyesRenderer != null && relaxedEyes != null)
        {
            eyesRenderer.sprite = relaxedEyes;
        }
        
        if (mouthRenderer != null && relaxedMouth != null)
        {
            mouthRenderer.sprite = relaxedMouth;
        }
        
        // Stop all effects
        StopShake();
        StopTears();
        
        // Ensure happy wink animation is running for relaxed state
        if (useHappyWink && happyWinkCoroutine == null)
        {
            happyWinkCoroutine = StartCoroutine(HappyWinkAnimation());
        }
    }
    
    private System.Collections.IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(blinkInterval);
            
            if (boxState != null && boxState.state == BoxState.State.Spawned)
            {
                yield return StartCoroutine(Blink());
            }
        }
    }
    
    private System.Collections.IEnumerator HappyWinkAnimation()
    {
        while (true)
        {
            // Only animate when box is in relaxed state
            if (boxState != null && boxState.state == BoxState.State.Sleep)
            {
                // Wait random time between winks (using inspector settings)
                yield return new WaitForSeconds(Random.Range(happyWinkMinInterval, happyWinkMaxInterval));
                
                // Wink left eye (if we have separate eye renderers)
                if (eyesRenderer != null)
                {
                    // Create a simple wink effect by scaling the eyes
                    Vector3 originalScale = eyesRenderer.transform.localScale;
                    
                    // Quick wink effect
                    eyesRenderer.transform.localScale = new Vector3(originalScale.x * 0.3f, originalScale.y, originalScale.z);
                    yield return new WaitForSeconds(happyWinkDuration);
                    eyesRenderer.transform.localScale = originalScale;
                    
                    yield return new WaitForSeconds(happyWinkPause);
                    
                    // Wink again (right eye effect)
                    eyesRenderer.transform.localScale = new Vector3(originalScale.x * 0.3f, originalScale.y, originalScale.z);
                    yield return new WaitForSeconds(happyWinkDuration);
                    eyesRenderer.transform.localScale = originalScale;
                }
            }
            else
            {
                // Wait a bit before checking again
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    private System.Collections.IEnumerator Blink()
    {
        if (eyesRenderer == null) yield break;
        
        isBlinking = true;
        
        // Close eyes
        Vector3 closedEyesPos = originalEyesPosition + Vector3.down * 0.1f;
        eyesRenderer.transform.localPosition = closedEyesPos;
        
        yield return new WaitForSeconds(blinkDuration);
        
        // Open eyes
        eyesRenderer.transform.localPosition = originalEyesPosition;
        
        isBlinking = false;
    }
    
    private System.Collections.IEnumerator ScaredShake()
    {
        Vector3 originalPos = eyesRenderer != null ? eyesRenderer.transform.localPosition : Vector3.zero;
        
        while (boxState != null && boxState.state == BoxState.State.Spawned)
        {
            float shakeX = Mathf.Sin(Time.time * 20f) * shakeIntensity;
            float shakeY = Mathf.Cos(Time.time * 15f) * shakeIntensity;
            
            if (eyesRenderer != null)
            {
                eyesRenderer.transform.localPosition = originalPos + new Vector3(shakeX, shakeY, 0);
            }
            if (mouthRenderer != null)
            {
                mouthRenderer.transform.localPosition = originalMouthPosition + new Vector3(shakeX, shakeY, 0);
            }
            
            yield return null;
        }
        
        // Reset positions
        if (eyesRenderer != null)
            eyesRenderer.transform.localPosition = originalPos;
        if (mouthRenderer != null)
            mouthRenderer.transform.localPosition = originalMouthPosition;
    }
    
    private System.Collections.IEnumerator TearsEffect()
    {
        if (tearPrefab == null) yield break;
        
        while (boxState != null && boxState.state == BoxState.State.Falling && this != null)
        {
            // Spawn tear from eyes
            if (eyesRenderer != null && tearPrefab != null)
            {
                Vector3 tearSpawnPos = eyesRenderer.transform.position + Vector3.down * 0.3f;
                GameObject tear = Instantiate(tearPrefab, tearSpawnPos, Quaternion.identity);
                
                // Make tear fall
                if (tear != null)
                {
                    StartCoroutine(FallTear(tear));
                }
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private System.Collections.IEnumerator FallTear(GameObject tear)
    {
        if (tear == null) yield break;
        
        float fallSpeed = 2f;
        float fadeTime = 1f;
        float elapsed = 0f;
        
        SpriteRenderer tearSprite = tear.GetComponent<SpriteRenderer>();
        Color originalColor = tearSprite != null ? tearSprite.color : Color.white;
        
        while (elapsed < fadeTime && tear != null)
        {
            elapsed += Time.deltaTime;
            
            // Check if tear still exists
            if (tear == null) yield break;
            
            // Move tear down
            tear.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            
            // Fade out
            if (tearSprite != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                tearSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }
            
            yield return null;
        }
        
        // Only destroy if tear still exists
        if (tear != null)
        {
            Destroy(tear);
        }
    }
    
    private void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
    }
    
    private void StopTears()
    {
        if (tearCoroutine != null)
        {
            StopCoroutine(tearCoroutine);
            tearCoroutine = null;
        }
    }
    
    private void StopHappyWink()
    {
        if (happyWinkCoroutine != null)
        {
            StopCoroutine(happyWinkCoroutine);
            happyWinkCoroutine = null;
        }
    }
    
    // Public methods for external control
    public void ForceScaredExpression()
    {
        SetScaredExpression();
    }
    
    public void ForceScreamingExpression()
    {
        SetScreamingExpression();
    }
    
    public void ForceHappyExpression()
    {
        SetRelaxedExpression();
    }
    
    // Method to reset expressions
    public void ResetExpressions()
    {
        StopShake();
        StopTears();
        
        if (eyesRenderer != null)
            eyesRenderer.transform.localPosition = originalEyesPosition;
        if (mouthRenderer != null)
            mouthRenderer.transform.localPosition = originalMouthPosition;
    }
    
    [ContextMenu("Reset Facial Positions")]
    public void ResetFacialPositions()
    {
        if (eyesRenderer != null)
        {
            eyesRenderer.transform.localPosition = new Vector3(0, 0.3f, 0);
            originalEyesPosition = eyesRenderer.transform.localPosition;
            eyesRenderer.sortingOrder = 2;
            Debug.Log("Reset eyes position to: " + eyesRenderer.transform.localPosition + " with sorting order: " + eyesRenderer.sortingOrder);
        }
        if (mouthRenderer != null)
        {
            mouthRenderer.transform.localPosition = new Vector3(0, -0.3f, 0);
            originalMouthPosition = mouthRenderer.transform.localPosition;
            mouthRenderer.sortingOrder = 2;
            Debug.Log("Reset mouth position to: " + mouthRenderer.transform.localPosition + " with sorting order: " + mouthRenderer.sortingOrder);
        }
    }
    
    [ContextMenu("Debug Facial Positions")]
    public void DebugFacialPositions()
    {
        if (eyesRenderer != null)
        {
            Debug.Log("Eyes position: " + eyesRenderer.transform.localPosition + ", sorting order: " + eyesRenderer.sortingOrder);
        }
        if (mouthRenderer != null)
        {
            Debug.Log("Mouth position: " + mouthRenderer.transform.localPosition + ", sorting order: " + mouthRenderer.sortingOrder);
        }
        
        // Check main box sprite sorting order
        SpriteRenderer mainRenderer = GetComponent<SpriteRenderer>();
        if (mainRenderer != null)
        {
            Debug.Log("Main box sprite sorting order: " + mainRenderer.sortingOrder);
        }
    }
    
    [ContextMenu("Fix All Sorting Orders")]
    public void FixAllSortingOrders()
    {
        // Set main box sprite to lowest sorting order
        SpriteRenderer mainRenderer = GetComponent<SpriteRenderer>();
        if (mainRenderer != null)
        {
            mainRenderer.sortingOrder = 0;
            Debug.Log("Main box sprite sorting order set to: " + mainRenderer.sortingOrder);
        }
        
        // Set facial features to higher sorting order
        if (eyesRenderer != null)
        {
            eyesRenderer.sortingOrder = 2;
            Debug.Log("Eyes sorting order set to: " + eyesRenderer.sortingOrder);
        }
        if (mouthRenderer != null)
        {
            mouthRenderer.sortingOrder = 2;
            Debug.Log("Mouth sorting order set to: " + mouthRenderer.sortingOrder);
        }
    }
    
    void OnDestroy()
    {
        StopShake();
        StopTears();
        StopHappyWink();
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
    }
} 
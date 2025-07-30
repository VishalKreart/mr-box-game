using UnityEngine;

public class TearDrop : MonoBehaviour
{
    [Header("Tear Settings")]
    public float fallSpeed = 2f;
    public float fadeTime = 1f;
    public float lifeTime = 2f;
    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float elapsed = 0f;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        // Destroy after lifetime
        Destroy(gameObject, lifeTime);
    }
    
    void Update()
    {
        elapsed += Time.deltaTime;
        
        // Move tear down
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        
        // Fade out over time
        if (spriteRenderer != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy tear when it hits something
        if (other.CompareTag("Platform") || other.CompareTag("Box"))
        {
            Destroy(gameObject);
        }
    }
} 
using UnityEngine;

public class MainMenuBackground : MonoBehaviour
{
    [Header("Background Settings")]
    public GameObject[] boxPrefabs; // Assign your box prefabs
    public int maxBoxes = 5;
    public float spawnInterval = 2f;
    public float boxLifetime = 8f;
    public float fallSpeed = 2f;
    
    [Header("Spawn Area")]
    public float minX = -8f;
    public float maxX = 8f;
    public float spawnY = 12f;
    
    private float nextSpawnTime;
    
    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }
    
    void Update()
    {
        if (Time.time >= nextSpawnTime && boxPrefabs.Length > 0)
        {
            SpawnBackgroundBox();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
    
    void SpawnBackgroundBox()
    {
        // Count existing background boxes
        GameObject[] existingBoxes = GameObject.FindGameObjectsWithTag("BackgroundBox");
        if (existingBoxes.Length >= maxBoxes) return;
        
        // Spawn a random box
        int randomIndex = Random.Range(0, boxPrefabs.Length);
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 10f); // Behind UI
        
        GameObject newBox = Instantiate(boxPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        newBox.tag = "BackgroundBox"; // Tag it as background
        
        // Add a simple falling script
        BackgroundBoxFaller faller = newBox.AddComponent<BackgroundBoxFaller>();
        faller.fallSpeed = fallSpeed;
        faller.lifetime = boxLifetime;
        
        // Make it semi-transparent
        SpriteRenderer renderer = newBox.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = 0.3f; // Semi-transparent
            renderer.color = color;
        }
        
        // Disable physics for background boxes
        Rigidbody2D rb = newBox.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.isKinematic = true;
        }
        
        // Disable colliders
        Collider2D[] colliders = newBox.GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
    }
}

// Simple script for background boxes to fall
public class BackgroundBoxFaller : MonoBehaviour
{
    public float fallSpeed = 2f;
    public float lifetime = 8f;
    
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        
        // Destroy if too far down
        if (transform.position.y < -15f)
        {
            Destroy(gameObject);
        }
    }
} 
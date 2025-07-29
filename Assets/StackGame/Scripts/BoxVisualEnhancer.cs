using UnityEngine;
using UnityEngine.UI;

public class BoxVisualEnhancer : MonoBehaviour
{
    [Header("Border Settings")]
    public float borderWidth = 0.1f;
    public Color borderColor = Color.black;
    public float cornerRadius = 0.1f;
    
    [Header("Box Settings")]
    public Color boxColor = Color.white;
    public bool useGradient = false;
    public Color gradientTop = Color.white;
    public Color gradientBottom = Color.gray;
    
    private GameObject borderObject;
    private Image borderImage;
    private Image boxImage;
    
    void Start()
    {
        CreateBoxWithBorder();
    }
    
    void CreateBoxWithBorder()
    {
        // Get the sprite renderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;
        
        // Create a UI Canvas for this box
        GameObject canvasObj = new GameObject("BoxCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = Vector3.zero;
        canvasObj.transform.localRotation = Quaternion.identity;
        canvasObj.transform.localScale = Vector3.one;
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = spriteRenderer.sortingOrder + 1;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Create the border (outer rectangle)
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(canvasObj.transform);
        borderObj.transform.localPosition = Vector3.zero;
        borderObj.transform.localScale = Vector3.one;
        
        borderImage = borderObj.AddComponent<Image>();
        borderImage.color = borderColor;
        borderImage.sprite = CreateRoundedRectangleSprite(1f + borderWidth * 2, 1f + borderWidth * 2, cornerRadius);
        
        // Create the box (inner rectangle)
        GameObject boxObj = new GameObject("Box");
        boxObj.transform.SetParent(canvasObj.transform);
        boxObj.transform.localPosition = Vector3.zero;
        boxObj.transform.localScale = Vector3.one;
        
        boxImage = boxObj.AddComponent<Image>();
        if (useGradient)
        {
            boxImage.sprite = CreateGradientSprite(1f, 1f, cornerRadius);
        }
        else
        {
            boxImage.color = boxColor;
            boxImage.sprite = CreateRoundedRectangleSprite(1f, 1f, cornerRadius);
        }
        
        // Hide the original sprite renderer
        spriteRenderer.enabled = false;
        
        // Store reference
        borderObject = borderObj;
    }
    
    Sprite CreateRoundedRectangleSprite(float width, float height, float radius)
    {
        // Create a simple rounded rectangle texture
        int textureSize = 64;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                float normalizedX = (float)x / textureSize;
                float normalizedY = (float)y / textureSize;
                
                // Check if pixel is in rounded corners
                bool inCorner = false;
                float cornerRadius = radius / Mathf.Max(width, height);
                
                // Top-left corner
                if (normalizedX < cornerRadius && normalizedY > 1f - cornerRadius)
                {
                    float distance = Vector2.Distance(new Vector2(normalizedX, normalizedY), 
                                                    new Vector2(cornerRadius, 1f - cornerRadius));
                    inCorner = distance > cornerRadius;
                }
                // Top-right corner
                else if (normalizedX > 1f - cornerRadius && normalizedY > 1f - cornerRadius)
                {
                    float distance = Vector2.Distance(new Vector2(normalizedX, normalizedY), 
                                                    new Vector2(1f - cornerRadius, 1f - cornerRadius));
                    inCorner = distance > cornerRadius;
                }
                // Bottom-left corner
                else if (normalizedX < cornerRadius && normalizedY < cornerRadius)
                {
                    float distance = Vector2.Distance(new Vector2(normalizedX, normalizedY), 
                                                    new Vector2(cornerRadius, cornerRadius));
                    inCorner = distance > cornerRadius;
                }
                // Bottom-right corner
                else if (normalizedX > 1f - cornerRadius && normalizedY < cornerRadius)
                {
                    float distance = Vector2.Distance(new Vector2(normalizedX, normalizedY), 
                                                    new Vector2(1f - cornerRadius, cornerRadius));
                    inCorner = distance > cornerRadius;
                }
                
                if (inCorner)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
                else
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
    
    Sprite CreateGradientSprite(float width, float height, float radius)
    {
        int textureSize = 64;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                float normalizedX = (float)x / textureSize;
                float normalizedY = (float)y / textureSize;
                
                // Check if pixel is in rounded corners
                bool inCorner = false;
                float cornerRadius = radius / Mathf.Max(width, height);
                
                // Top-left corner
                if (normalizedX < cornerRadius && normalizedY > 1f - cornerRadius)
                {
                    float distance = Vector2.Distance(new Vector2(normalizedX, normalizedY), 
                                                    new Vector2(cornerRadius, 1f - cornerRadius));
                    inCorner = distance > cornerRadius;
                }
                // Top-right corner
                else if (normalizedX > 1f - cornerRadius && normalizedY > 1f - cornerRadius)
                {
                    float distance = Vector2.Distance(new Vector2(normalizedX, normalizedY), 
                                                    new Vector2(1f - cornerRadius, 1f - cornerRadius));
                    inCorner = distance > cornerRadius;
                }
                // Bottom-left corner
                else if (normalizedX < cornerRadius && normalizedY < cornerRadius)
                {
                    float distance = Vector2.Distance(new Vector2(normalizedX, normalizedY), 
                                                    new Vector2(cornerRadius, cornerRadius));
                    inCorner = distance > cornerRadius;
                }
                // Bottom-right corner
                else if (normalizedX > 1f - cornerRadius && normalizedY < cornerRadius)
                {
                    float distance = Vector2.Distance(new Vector2(normalizedX, normalizedY), 
                                                    new Vector2(1f - cornerRadius, cornerRadius));
                    inCorner = distance > cornerRadius;
                }
                
                if (inCorner)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
                else
                {
                    // Create gradient from top to bottom
                    Color gradientColor = Color.Lerp(gradientTop, gradientBottom, 1f - normalizedY);
                    texture.SetPixel(x, y, gradientColor);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
    
    public void SetBoxColor(Color color)
    {
        if (boxImage != null)
        {
            boxImage.color = color;
        }
    }
    
    public void SetBorderColor(Color color)
    {
        if (borderImage != null)
        {
            borderImage.color = color;
        }
    }
    
    public void SetBorderWidth(float width)
    {
        if (borderImage != null)
        {
            borderImage.transform.localScale = Vector3.one * (1f + width * 2);
        }
    }
} 
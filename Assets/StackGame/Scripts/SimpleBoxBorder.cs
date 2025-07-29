using UnityEngine;

public class SimpleBoxBorder : MonoBehaviour
{
    [Header("Border Settings")]
    public float borderWidth = 0.05f;
    public Color borderColor = Color.black;
    
    [Header("Visual Effects")]
    public bool useShadow = true;
    public float shadowOffset = 0.02f;
    public Color shadowColor = new Color(0, 0, 0, 0.3f);
    
    [Header("Color Override (Optional)")]
    public bool overrideBoxColor = false;
    public Color overrideColor = Color.white;
    
    private GameObject borderObject;
    private GameObject shadowObject;
    private SpriteRenderer originalRenderer;
    private SpriteRenderer borderRenderer;
    private SpriteRenderer shadowRenderer;
    
    void Start()
    {
        CreateBorder();
    }
    
    public void CreateBorder()
    {
        originalRenderer = GetComponent<SpriteRenderer>();
        if (originalRenderer == null) return;
        
        // Create shadow (optional)
        if (useShadow)
        {
            CreateShadow();
        }
        
        // Create border
        CreateBorderObject();
        
        // Only override color if specifically requested
        if (overrideBoxColor)
        {
            originalRenderer.color = overrideColor;
        }
        // Otherwise, keep the original color from SpriteRenderer
    }
    
    void CreateShadow()
    {
        shadowObject = new GameObject("Shadow");
        shadowObject.transform.SetParent(transform);
        shadowObject.transform.localPosition = new Vector3(shadowOffset, -shadowOffset, 0.1f);
        shadowObject.transform.localScale = Vector3.one;
        
        shadowRenderer = shadowObject.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = originalRenderer.sprite;
        shadowRenderer.color = shadowColor;
        shadowRenderer.sortingOrder = originalRenderer.sortingOrder - 1;
    }
    
    void CreateBorderObject()
    {
        borderObject = new GameObject("Border");
        borderObject.transform.SetParent(transform);
        borderObject.transform.localPosition = Vector3.zero;
        borderObject.transform.localScale = Vector3.one * (1f + borderWidth * 2);
        
        borderRenderer = borderObject.AddComponent<SpriteRenderer>();
        borderRenderer.sprite = originalRenderer.sprite;
        borderRenderer.color = borderColor;
        borderRenderer.sortingOrder = originalRenderer.sortingOrder - 1;
    }
    
    public void SetBoxColor(Color color)
    {
        if (originalRenderer != null)
        {
            originalRenderer.color = color;
        }
    }
    
    public void SetBorderColor(Color color)
    {
        if (borderRenderer != null)
        {
            borderRenderer.color = color;
        }
    }
    
    public void SetBorderWidth(float width)
    {
        if (borderObject != null)
        {
            borderObject.transform.localScale = Vector3.one * (1f + width * 2);
        }
    }
    
    public void SetShadow(bool enabled)
    {
        if (shadowObject != null)
        {
            shadowObject.SetActive(enabled);
        }
    }
    
    // Method to update colors based on box type (only if override is enabled)
    public void UpdateBoxAppearance(string boxType)
    {
        if (!overrideBoxColor) return; // Don't override if not enabled
        
        switch (boxType.ToLower())
        {
            case "normal":
                SetBoxColor(Color.white);
                SetBorderColor(Color.black);
                break;
            case "wide":
                SetBoxColor(new Color(0.3f, 0.6f, 1f)); // Blue
                SetBorderColor(Color.black);
                break;
            case "tall":
                SetBoxColor(new Color(0.3f, 0.8f, 0.3f)); // Green
                SetBorderColor(Color.black);
                break;
            case "small":
                SetBoxColor(new Color(1f, 1f, 0.3f)); // Yellow
                SetBorderColor(Color.black);
                break;
            case "large":
                SetBoxColor(new Color(1f, 0.3f, 0.3f)); // Red
                SetBorderColor(Color.black);
                break;
            case "golden":
                SetBoxColor(new Color(1f, 0.8f, 0f)); // Gold
                SetBorderColor(new Color(0.8f, 0.6f, 0f)); // Dark gold border
                break;
            default:
                SetBoxColor(Color.white);
                SetBorderColor(Color.black);
                break;
        }
    }
    
    // Get the current box color (useful for BoxVariations)
    public Color GetCurrentBoxColor()
    {
        return originalRenderer != null ? originalRenderer.color : Color.white;
    }
} 
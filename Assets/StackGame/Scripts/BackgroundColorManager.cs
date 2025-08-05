using UnityEngine;

public class BackgroundColorManager : MonoBehaviour
{
    [Header("Background Settings")]
    public Camera mainCamera;
    public bool changeOnGameStart = true;
    
    [Header("Color Palette")]
    public Color[] backgroundColors = new Color[]
    {
        new Color(0.2f, 0.6f, 0.8f), // Light Blue
        new Color(0.8f, 0.4f, 0.6f), // Pink
        new Color(0.4f, 0.8f, 0.4f), // Light Green
        new Color(0.9f, 0.7f, 0.3f), // Orange
        new Color(0.6f, 0.4f, 0.8f), // Purple
        new Color(0.8f, 0.6f, 0.4f), // Brown
        new Color(0.3f, 0.7f, 0.8f), // Cyan
        new Color(0.8f, 0.3f, 0.5f), // Rose
        new Color(0.5f, 0.8f, 0.6f), // Mint
        new Color(0.7f, 0.5f, 0.8f)  // Lavender
    };
    
    private Color currentColor;
    private bool colorSet = false;
    
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (changeOnGameStart && !colorSet)
        {
            SetRandomBackgroundColor();
        }
    }
    
    public void SetRandomBackgroundColor()
    {
        if (backgroundColors.Length == 0) return;
        
        // Get a random color
        currentColor = backgroundColors[Random.Range(0, backgroundColors.Length)];
        mainCamera.backgroundColor = currentColor;
        colorSet = true;
    }
    
    public void SetSpecificBackgroundColor(Color color)
    {
        currentColor = color;
        mainCamera.backgroundColor = currentColor;
        colorSet = true;
    }
    
    // Call this when game starts to get a fresh color
    public void OnGameStart()
    {
        colorSet = false; // Reset so new game gets new color
        SetRandomBackgroundColor();
    }
    
    // Call this when game ends (no longer needed, but keeping for compatibility)
    public void OnGameEnd()
    {
        // Color stays the same during game
    }
} 
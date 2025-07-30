using UnityEngine;
using System.IO;

public class SpriteExpressionGenerator : MonoBehaviour
{
    [Header("Sprite Generation")]
    public bool generateOnStart = false;
    public string savePath = "Assets/StackGame/Sprites/Expressions/";
    
    [Header("Sprite Settings")]
    public int spriteSize = 32;
    public Color outlineColor = Color.black;
    public Color fillColor = Color.white;
    public Color backgroundColor = Color.clear;
    
    void Start()
    {
        if (generateOnStart)
        {
            GenerateAllExpressions();
        }
    }
    
    [ContextMenu("Generate All Expressions")]
    public void GenerateAllExpressions()
    {
        // Create directory if it doesn't exist
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        
        // Generate all expression sprites
        GenerateScaredEyes();
        GenerateScreamingEyes();
        GenerateRelaxedEyes();
        GenerateScaredMouth();
        GenerateScreamingMouth();
        GenerateRelaxedMouth();
        GenerateTearDrop();
        
        Debug.Log("All expression sprites generated!");
    }
    
    [ContextMenu("Generate Scared Eyes")]
    public void GenerateScaredEyes()
    {
        Texture2D texture = new Texture2D(spriteSize, spriteSize);
        Color[] pixels = new Color[spriteSize * spriteSize];
        
        // Fill with background color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        
        // Draw scared eyes (wide, worried)
        DrawEye(pixels, 6, 8, true);   // Left eye
        DrawEye(pixels, 18, 8, true);  // Right eye
        
        // Draw eyebrows (raised, worried)
        DrawEyebrow(pixels, 6, 6, true);   // Left eyebrow
        DrawEyebrow(pixels, 18, 6, true);  // Right eyebrow
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        SaveTexture(texture, "ScaredEyes");
    }
    
    [ContextMenu("Generate Screaming Eyes")]
    public void GenerateScreamingEyes()
    {
        Texture2D texture = new Texture2D(spriteSize, spriteSize);
        Color[] pixels = new Color[spriteSize * spriteSize];
        
        // Fill with background color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        
        // Draw screaming eyes (squinted/closed)
        DrawSquintedEye(pixels, 6, 8);   // Left eye
        DrawSquintedEye(pixels, 18, 8);  // Right eye
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        SaveTexture(texture, "ScreamingEyes");
    }
    
    [ContextMenu("Generate Relaxed Eyes")]
    public void GenerateRelaxedEyes()
    {
        Texture2D texture = new Texture2D(spriteSize, spriteSize);
        Color[] pixels = new Color[spriteSize * spriteSize];
        
        // Fill with background color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        
        // Draw relaxed eyes (happy, normal)
        DrawEye(pixels, 6, 8, false);   // Left eye
        DrawEye(pixels, 18, 8, false);  // Right eye
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        SaveTexture(texture, "RelaxedEyes");
    }
    
    [ContextMenu("Generate Scared Mouth")]
    public void GenerateScaredMouth()
    {
        Texture2D texture = new Texture2D(spriteSize, spriteSize);
        Color[] pixels = new Color[spriteSize * spriteSize];
        
        // Fill with background color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        
        // Draw scared mouth (small, worried)
        DrawWorriedMouth(pixels, 8, 20);
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        SaveTexture(texture, "ScaredMouth");
    }
    
    [ContextMenu("Generate Screaming Mouth")]
    public void GenerateScreamingMouth()
    {
        Texture2D texture = new Texture2D(spriteSize, spriteSize);
        Color[] pixels = new Color[spriteSize * spriteSize];
        
        // Fill with background color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        
        // Draw screaming mouth (wide open)
        DrawScreamingMouth(pixels, 8, 18);
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        SaveTexture(texture, "ScreamingMouth");
    }
    
    [ContextMenu("Generate Relaxed Mouth")]
    public void GenerateRelaxedMouth()
    {
        Texture2D texture = new Texture2D(spriteSize, spriteSize);
        Color[] pixels = new Color[spriteSize * spriteSize];
        
        // Fill with background color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        
        // Draw relaxed mouth (happy smile)
        DrawHappyMouth(pixels, 8, 20);
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        SaveTexture(texture, "RelaxedMouth");
    }
    
    [ContextMenu("Generate Tear Drop")]
    public void GenerateTearDrop()
    {
        Texture2D texture = new Texture2D(16, 16);
        Color[] pixels = new Color[16 * 16];
        
        // Fill with background color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        
        // Draw tear drop
        DrawTearDrop(pixels, 4, 2);
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        SaveTexture(texture, "TearDrop");
    }
    
    private void DrawEye(Color[] pixels, int x, int y, bool scared)
    {
        int eyeWidth = scared ? 14 : 12;
        int eyeHeight = scared ? 10 : 8;
        
        // Draw eye outline
        for (int i = 0; i < eyeWidth; i++)
        {
            for (int j = 0; j < eyeHeight; j++)
            {
                int index = (y + j) * spriteSize + (x + i);
                if (index < pixels.Length && index >= 0)
                {
                    pixels[index] = outlineColor;
                }
            }
        }
        
        // Fill eye with white
        for (int i = 1; i < eyeWidth - 1; i++)
        {
            for (int j = 1; j < eyeHeight - 1; j++)
            {
                int index = (y + j) * spriteSize + (x + i);
                if (index < pixels.Length && index >= 0)
                {
                    pixels[index] = fillColor;
                }
            }
        }
        
        // Add pupil for scared eyes
        if (scared)
        {
            for (int i = 3; i < 7; i++)
            {
                for (int j = 3; j < 7; j++)
                {
                    int index = (y + j) * spriteSize + (x + i);
                    if (index < pixels.Length && index >= 0)
                    {
                        pixels[index] = outlineColor;
                    }
                }
            }
        }
    }
    
    private void DrawSquintedEye(Color[] pixels, int x, int y)
    {
        // Draw squinted eye (simple curved line)
        for (int i = 0; i < 12; i++)
        {
            int curve = Mathf.RoundToInt(Mathf.Sin(i * 0.5f) * 2);
            int index = (y + curve) * spriteSize + (x + i);
            if (index < pixels.Length && index >= 0)
            {
                pixels[index] = outlineColor;
            }
        }
    }
    
    private void DrawEyebrow(Color[] pixels, int x, int y, bool raised)
    {
        // Draw raised eyebrow
        for (int i = 0; i < 12; i++)
        {
            int curve = raised ? Mathf.RoundToInt(Mathf.Sin(i * 0.3f) * 1) : 0;
            int index = (y + curve) * spriteSize + (x + i);
            if (index < pixels.Length && index >= 0)
            {
                pixels[index] = outlineColor;
            }
        }
    }
    
    private void DrawWorriedMouth(Color[] pixels, int x, int y)
    {
        // Draw small, worried mouth
        for (int i = 0; i < 16; i++)
        {
            int curve = Mathf.RoundToInt(Mathf.Sin(i * 0.4f) * 1);
            int index = (y + curve) * spriteSize + (x + i);
            if (index < pixels.Length && index >= 0)
            {
                pixels[index] = outlineColor;
            }
        }
    }
    
    private void DrawScreamingMouth(Color[] pixels, int x, int y)
    {
        // Draw wide open mouth (oval)
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                // Create oval shape
                float normalizedX = (i - 8f) / 8f;
                float normalizedY = (j - 6f) / 6f;
                float distance = Mathf.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);
                
                if (distance <= 1f)
                {
                    int index = (y + j) * spriteSize + (x + i);
                    if (index < pixels.Length && index >= 0)
                    {
                        pixels[index] = distance > 0.8f ? outlineColor : fillColor;
                    }
                }
            }
        }
    }
    
    private void DrawHappyMouth(Color[] pixels, int x, int y)
    {
        // Draw happy smile
        for (int i = 0; i < 16; i++)
        {
            int curve = Mathf.RoundToInt(Mathf.Sin(i * 0.4f) * 2);
            int index = (y + curve) * spriteSize + (x + i);
            if (index < pixels.Length && index >= 0)
            {
                pixels[index] = outlineColor;
            }
        }
    }
    
    private void DrawTearDrop(Color[] pixels, int x, int y)
    {
        // Draw simple tear drop shape
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                // Create teardrop shape
                float normalizedX = (i - 4f) / 4f;
                float normalizedY = (j - 6f) / 6f;
                float distance = Mathf.Sqrt(normalizedX * normalizedX + normalizedY * normalizedY);
                
                if (distance <= 1f && normalizedY >= -0.5f)
                {
                    int index = (y + j) * 16 + (x + i);
                    if (index < pixels.Length && index >= 0)
                    {
                        pixels[index] = distance > 0.8f ? outlineColor : new Color(0.3f, 0.8f, 1f, 0.8f);
                    }
                }
            }
        }
    }
    
    private void SaveTexture(Texture2D texture, string name)
    {
        byte[] bytes = texture.EncodeToPNG();
        string filePath = savePath + name + ".png";
        File.WriteAllBytes(filePath, bytes);
        
        Debug.Log($"Saved {name}.png to {filePath}");
        
        // Refresh Unity's asset database
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }
} 
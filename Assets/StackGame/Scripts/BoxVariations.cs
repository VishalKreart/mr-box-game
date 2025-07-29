using UnityEngine;

[System.Serializable]
public class BoxVariation
{
    public string boxName;
    public GameObject boxPrefab;
    public float width = 1f;
    public float height = 1f;
    public float spawnWeight = 1f; // Higher weight = more likely to spawn
    public Color boxColor = Color.white;
    public Color borderColor = Color.black;
    public int pointValue = 1; // Points this box gives when stacked
    public bool useShadow = true;
    public float borderWidth = 0.05f;
    public bool overridePrefabColor = false; // Only override if this is true
}

public class BoxVariations : MonoBehaviour
{
    [Header("Box Variations")]
    public BoxVariation[] boxVariations;
    
    [Header("Spawn Settings")]
    public bool useWeightedSpawning = true;
    public float difficultyProgression = 0.1f; // How much harder it gets per box
    
    [Header("Visual Settings")]
    public bool autoApplyBorders = true;
    public bool useShadows = true;
    public float defaultBorderWidth = 0.05f;
    public bool respectPrefabColors = true; // Keep original prefab colors unless overridden
    
    private float totalWeight;
    
    void Start()
    {
        CalculateTotalWeight();
    }
    
    void CalculateTotalWeight()
    {
        totalWeight = 0f;
        foreach (BoxVariation variation in boxVariations)
        {
            totalWeight += variation.spawnWeight;
        }
    }
    
    public GameObject GetRandomBox()
    {
        if (boxVariations.Length == 0) return null;
        
        if (useWeightedSpawning)
        {
            return GetWeightedRandomBox();
        }
        else
        {
            return boxVariations[Random.Range(0, boxVariations.Length)].boxPrefab;
        }
    }
    
    public GameObject GetWeightedRandomBox()
    {
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        foreach (BoxVariation variation in boxVariations)
        {
            currentWeight += variation.spawnWeight;
            if (randomValue <= currentWeight)
            {
                return variation.boxPrefab;
            }
        }
        
        // Fallback to first box
        return boxVariations[0].boxPrefab;
    }
    
    public GameObject GetBoxByDifficulty(int currentScore)
    {
        // As score increases, spawn harder boxes
        float difficulty = currentScore * difficultyProgression;
        
        // Simple difficulty progression
        if (difficulty < 5f)
        {
            // Early game: mostly normal boxes
            return GetBoxByName("Normal");
        }
        else if (difficulty < 15f)
        {
            // Mid game: mix of normal and wide boxes
            return Random.Range(0f, 1f) < 0.7f ? GetBoxByName("Normal") : GetBoxByName("Wide");
        }
        else
        {
            // Late game: all variations
            return GetRandomBox();
        }
    }
    
    public GameObject GetBoxByName(string boxName)
    {
        foreach (BoxVariation variation in boxVariations)
        {
            if (variation.boxName == boxName)
            {
                return variation.boxPrefab;
            }
        }
        
        // Return first box if name not found
        return boxVariations.Length > 0 ? boxVariations[0].boxPrefab : null;
    }
    
    // Helper method to get box properties
    public BoxVariation GetBoxVariation(GameObject boxPrefab)
    {
        foreach (BoxVariation variation in boxVariations)
        {
            if (variation.boxPrefab == boxPrefab)
            {
                return variation;
            }
        }
        return null;
    }
    
    // Apply visual enhancements to a spawned box
    public void ApplyVisualEnhancements(GameObject spawnedBox)
    {
        if (!autoApplyBorders) return;
        
        // Try to get the box variation
        BoxVariation variation = null;
        foreach (BoxVariation var in boxVariations)
        {
            if (var.boxPrefab.name == spawnedBox.name.Replace("(Clone)", ""))
            {
                variation = var;
                break;
            }
        }
        
        // Apply SimpleBoxBorder script
        SimpleBoxBorder borderScript = spawnedBox.GetComponent<SimpleBoxBorder>();
        if (borderScript == null)
        {
            borderScript = spawnedBox.AddComponent<SimpleBoxBorder>();
        }
        
        // Configure the border
        if (variation != null)
        {
            borderScript.borderWidth = variation.borderWidth;
            borderScript.borderColor = variation.borderColor;
            borderScript.useShadow = variation.useShadow && useShadows;
            
            // Only override color if specifically requested
            if (variation.overridePrefabColor)
            {
                borderScript.overrideBoxColor = true;
                borderScript.overrideColor = variation.boxColor;
            }
            else
            {
                // Keep the original prefab color
                borderScript.overrideBoxColor = false;
            }
        }
        else
        {
            // Default settings
            borderScript.borderWidth = defaultBorderWidth;
            borderScript.borderColor = Color.black;
            borderScript.useShadow = useShadows;
            borderScript.overrideBoxColor = false; // Don't override prefab colors
        }
        
        // Force the border to be created
        borderScript.CreateBorder();
    }
} 
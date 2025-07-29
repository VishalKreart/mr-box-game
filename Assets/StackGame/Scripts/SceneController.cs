using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Check if we're starting from the main menu or directly in gameplay
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("Current scene: " + currentScene);
        
        // If we're in the gameplay scene and there's no main menu, create a simple one
        if (currentScene == "MainScene" && !SceneExists("MainMenu"))
        {
            Debug.LogWarning("MainMenu scene not found in build settings. Game will start directly in gameplay.");
        }
    }
    
    public bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameFromPath == sceneName)
                return true;
        }
        return false;
    }
    
    public void LoadScene(string sceneName)
    {
        if (SceneExists(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene '" + sceneName + "' not found in build settings!");
        }
    }
    
    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }
    
    public void LoadGameplay()
    {
        LoadScene("MainScene");
    }
} 
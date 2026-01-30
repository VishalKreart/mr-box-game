using UnityEngine;
using System;

public class ScreenshotTool : MonoBehaviour
{
    private static ScreenshotTool instance;

    void Awake()
    {
        // Singleton so it exists only once
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"WobblyTower_{Screen.width}x{Screen.height}_{timestamp}.png";

        // Saves to a safe & accessible location
        string path = System.IO.Path.Combine(
            Application.persistentDataPath,
            fileName
        );

        ScreenCapture.CaptureScreenshot(path, 1);

        Debug.Log($"Screenshot saved: {path}");
    }
}

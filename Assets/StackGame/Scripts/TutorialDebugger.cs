using UnityEngine;
using UnityEngine.UI;

public class TutorialDebugger : MonoBehaviour
{
    public TutorialManager tutorialManager;
    public HandController handController;
    
    void Update()
    {
        // Test tutorial reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            tutorialManager.ResetTutorial();
            Debug.Log("Tutorial reset! Restart the game to see it again.");
        }
        
        // Test hand animations manually
        if (handController != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                handController.PlayDragAnimation();
                Debug.Log("Playing drag animation");
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                handController.PlayTapAnimation();
                Debug.Log("Playing tap animation");
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                handController.PlayHideAnimation();
                Debug.Log("Playing hide animation");
            }
        }
        
        // Test skip tutorial
        if (Input.GetKeyDown(KeyCode.S))
        {
            tutorialManager.SkipTutorial();
            Debug.Log("Skipping tutorial");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("Tutorial Debug Controls:");
        GUILayout.Label("R - Reset Tutorial");
        GUILayout.Label("S - Skip Tutorial");
        GUILayout.Label("1 - Play Drag Animation");
        GUILayout.Label("2 - Play Tap Animation");
        GUILayout.Label("3 - Play Hide Animation");
        GUILayout.EndArea();
    }
} 
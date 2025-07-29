using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject overlayPanel; // The semi-transparent panel
    public TextMeshProUGUI tutorialText; // The TMP tutorial message
    public HandController handController;   // HandController for the hand/finger
    public Button skipButton;       // Skip button
    private int step = 0;
    private bool tutorialActive = false;

    void Start()
    {
        // Connect skip button
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
        }

        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            tutorialActive = true;
            ShowStep(0);
        }
        else
        {
            EndTutorial();
        }
    }

    void Update()
    {
        if (!tutorialActive) return;

        if (step == 0)
        {
            if (Input.GetMouseButton(0) && Mathf.Abs(Input.GetAxis("Mouse X")) > 0.1f)
            {
                ShowStep(1);
            }
            if (Input.touchCount > 0 && Mathf.Abs(Input.GetTouch(0).deltaPosition.x) > 5f)
            {
                ShowStep(1);
            }
        }
        else if (step == 1)
        {
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                ShowStep(2);
            }
        }
    }

    void ShowStep(int s)
    {
        step = s;
        switch (step)
        {
            case 0:
                overlayPanel.SetActive(true);
                tutorialText.text = "Welcome! Drag anywhere to move the box left or right.";
                if (handController != null) handController.PlayDragAnimation();
                break;
            case 1:
                tutorialText.text = "Release to drop the box. Or tap anywhere to drop it in place.";
                if (handController != null) handController.PlayTapAnimation();
                break;
            case 2:
                tutorialText.text = "Great! Stack as many boxes as you can. Good luck!";
                if (handController != null) handController.PlayHideAnimation();
                Invoke("EndTutorial", 1.5f);
                break;
        }
    }

    public void SkipTutorial()
    {
        Debug.Log("Skip Tutorial called");
        EndTutorial();
    }

    void EndTutorial()
    {
        Debug.Log("End Tutorial called");
        tutorialActive = false;
        
        // Hide all tutorial elements
        if (overlayPanel != null)
            overlayPanel.SetActive(false);
        
        if (tutorialText != null)
            tutorialText.gameObject.SetActive(false);
        
        if (skipButton != null)
            skipButton.gameObject.SetActive(false);
        
        // Hide hand
        if (handController != null)
            handController.HideHand();
        
        // Save tutorial completion
        PlayerPrefs.SetInt("TutorialComplete", 1);
        PlayerPrefs.Save();
        
        // Disable the entire tutorial manager
        gameObject.SetActive(false);
    }
    
    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialComplete", 0);
        PlayerPrefs.Save();
        Debug.Log("Tutorial reset - you can now see it again");
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class TutorialManager : MonoBehaviour
{
    public GameObject overlayPanel; // The semi-transparent panel
    public TextMeshProUGUI tutorialText; // The TMP tutorial message
    //public HandController handController;   // HandController for the hand/finger
    //public Button skipButton;       // Skip button
    private int step = 0;
    private bool tutorialActive = false;

    void Start()
    {
        // Connect skip button
        //if (skipButton != null)
        //{
        //    skipButton.onClick.AddListener(SkipTutorial);
        //}

        //if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        //{
        //    tutorialActive = true;
        //    ShowStep(0);
        //}
        //else
        //{
        //    EndTutorial();
        //}

         //Check if tutorial is already completed
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            StartTutorial();
        }
        else
        {
            EndTutorial();
        }
        
    }
    void StartTutorial()
    {
        Debug.Log("tutorial start");
        tutorialActive = true;
        overlayPanel.SetActive(true);
        tutorialText.text = "Tap anywhere to drop the box and Stack as many boxes as you can";

        
    }
    // Handle tap on the screen
    
    void EndTutorial()
    {
        //if (!tutorialActive) return;
        Debug.Log("End Tutorial called");
        tutorialActive = false;

        // Hide tutorial elements
        overlayPanel.SetActive(false);
        tutorialText.gameObject.SetActive(false);

        // Hide hand
        //if (handController != null)
        //{
        //    handController.HideHand();
        //}

        // Save tutorial completion
        PlayerPrefs.SetInt("TutorialComplete", 1);
        PlayerPrefs.Save();

        // Disable the tutorial manager
        gameObject.SetActive(false);
    }
    //void EndTutorial()
    //{
    //    Debug.Log("End Tutorial called");
    //    tutorialActive = false;

    //    // Hide all tutorial elements
    //    if (overlayPanel != null)
    //        overlayPanel.SetActive(false);

    //    if (tutorialText != null)
    //        tutorialText.gameObject.SetActive(false);

    //    if (skipButton != null)
    //        skipButton.gameObject.SetActive(false);

    //    // Hide hand
    //    if (handController != null)
    //        handController.HideHand();

    //    // Save tutorial completion
    //    PlayerPrefs.SetInt("TutorialComplete", 1);
    //    PlayerPrefs.Save();

    //    // Disable the entire tutorial manager
    //    gameObject.SetActive(false);
    //}

    void Update()
    {
        //if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        //{
        //    ShowStep(2);
        //}

        if (!tutorialActive) return;
        // Check for mouse click or touch
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            // Check if we're clicking on UI (if needed)
            //if (EventSystem.current.IsPointerOverGameObject() || 
            //    (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            //{
            //    // This is a UI click, handle it
            //    OnTutorialTap();
            //}
            //else
            {
                // This is a world-space click
                OnTutorialTap();
            }
        }
    }
 private void OnTutorialTap()
    {
        Debug.Log("Tutorial tapped");
        if (!tutorialActive) return;
        EndTutorial();
    }
    //void ShowStep(int s)
    //{
    //    step = s;
    //    switch (step)
    //    {
    //        case 0:
    //            overlayPanel.SetActive(true);
    //            tutorialText.text = "Welcome! Drag anywhere to move the box left or right.";
    //            if (handController != null) handController.PlayDragAnimation();
    //            break;
    //        case 1:
    //            tutorialText.text = "Release to drop the box. Or tap anywhere to drop it in place.";
    //            if (handController != null) handController.PlayTapAnimation();
    //            break;
    //        case 2:
    //            tutorialText.text = "Great! Stack as many boxes as you can. Good luck!";
    //            if (handController != null) handController.PlayHideAnimation();
    //            Invoke("EndTutorial", 1.5f);
    //            break;
    //    }
    //}

    public void SkipTutorial()
    {
        Debug.Log("Skip Tutorial called");
        EndTutorial();
    }

    
    
    public void ResetTutorial()
    {
        PlayerPrefs.SetInt("TutorialComplete", 0);
        PlayerPrefs.Save();
        Debug.Log("Tutorial reset - you can now see it again");
    }
}
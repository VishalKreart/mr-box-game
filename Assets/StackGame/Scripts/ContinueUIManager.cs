using UnityEngine;
using UnityEngine.UI;

public class ContinueUIManager : MonoBehaviour
{
    [Header("Popup UI")]
    [SerializeField] private GameObject continuePopup;   // The panel/popup root
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button cancelButton;

    private void OnEnable()
    {
        if (watchAdButton) watchAdButton.onClick.AddListener(OnWatchAd);
        if (cancelButton) cancelButton.onClick.AddListener(OnCancel);

        //// If a watch attempt finds no ad, we auto-close and finalize Game Over
        //if (MonetizationManager.Instance != null)
        //    MonetizationManager.Instance.OnNoRewardedAdAvailable += HandleNoAdAvailable;
    }

    private void OnDisable()
    {
        if (watchAdButton) watchAdButton.onClick.RemoveListener(OnWatchAd);
        if (cancelButton) cancelButton.onClick.RemoveListener(OnCancel);

        //if (MonetizationManager.Instance != null)
        //    MonetizationManager.Instance.OnNoRewardedAdAvailable -= HandleNoAdAvailable;
    }

    // Old name kept for backward compatibility
    public void ShowContinuePanel() => ShowContinuePopup();

    public void ShowContinuePopup()
    {
        if (continuePopup) continuePopup.SetActive(true);
    }

    private void Hide()
    {
        if (continuePopup) continuePopup.SetActive(false);
    }

    private void OnWatchAd()
    {
        Hide();
        //MonetizationManager.Instance.ShowRewardedAd(() =>
        //{
        //    // Reward complete -> resume game
        //    //GameManager.Instance.ResumeGame();
        //    FindAnyObjectByType<GameManager>().ResumeGame();
        //});
    }

    private void OnCancel()
    {
        Hide();
        //GameManager.Instance.GameOver();
        FindAnyObjectByType<GameManager>().GameOver();
    }

    private void HandleNoAdAvailable()
    {
        Hide();
        FindAnyObjectByType<GameManager>().GameOver();
    }
}

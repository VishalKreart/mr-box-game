using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class MonetizationManager : MonoBehaviour, IStoreListener
{
    public static MonetizationManager Instance;

    [Header("IronSource App Keys")]
    [SerializeField] private string androidAppKey = "YOUR_ANDROID_APP_KEY";
    [SerializeField] private string iosAppKey = "YOUR_IOS_APP_KEY";

    [Header("Ads")]
    [SerializeField] private int interstitialEveryXGames = 3;

    // IAP
    private const string REMOVE_ADS = "remove_ads"; // Non-consumable
    private IStoreController storeController;
    private bool iapInitialized;

    // State
    public bool adsRemoved = false; // kept public for legacy reads
    private int gamesSinceInterstitial = 0;

    // Optional: notify UI if Watch Ad pressed but no ad available
    public event Action OnNoRewardedAdAvailable;


    public static event Action<bool> OnAdsRemoved;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadState();
    }

    private void Start()
    {
        InitializeAds();
        InitializeIAP();
    }

    // ---------------- IronSource ----------------
    private void InitializeAds()
    {
#if UNITY_ANDROID
        string appKey = androidAppKey;
#elif UNITY_IOS
        string appKey = iosAppKey;
#else
        string appKey = "";
#endif
        if (string.IsNullOrEmpty(appKey))
        {
            Debug.LogWarning("[MonetizationManager] IronSource app key not set for this platform.");
            return;
        }
        //IronSource.Agent.setMetaData("is_test_suite", "enable");

        IronSource.Agent.init(appKey);
        IronSource.Agent.validateIntegration();
        

        // Subscribe to init complete callback
        IronSourceEvents.onSdkInitializationCompletedEvent += OnIronSourceInitComplete;

        // Preload interstitials
        //IronSource.Agent.loadInterstitial();

    }


    private void OnIronSourceInitComplete()
    {
        Debug.Log("[MonetizationManager] IronSource initialization completed successfully.");
        //IronSource.Agent.launchTestSuite();
        // Preload interstitials after init
        IronSource.Agent.loadInterstitial();
        IronSource.Agent.loadRewardedVideo();
    }
    // ---------------- Rewarded ----------------
    private Action rewardedCallback;

    /// <summary>
    /// Legacy signature kept. Calls the overload with null callback.
    /// </summary>
    public void ShowRewardedAd() => ShowRewardedAd(null);

    /// <summary>
    /// Shows rewarded ad; on completion invokes callback (used by Continue popup).
    /// </summary>
    public void ShowRewardedAd(Action onRewardGranted)
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            rewardedCallback = onRewardGranted;

            // Subscribe once with the correct LevelPlay signature
            void OnRewarded(IronSourcePlacement placement, IronSourceAdInfo adInfo)
            {
                try
                {
                    rewardedCallback?.Invoke();          // resume gameplay, etc.
                    OnContinueUsed?.Invoke();            // legacy event fire (if anyone listens)
                }
                finally
                {
                    rewardedCallback = null;
                    IronSourceRewardedVideoEvents.onAdRewardedEvent -= OnRewarded;
                }
            }

            IronSourceRewardedVideoEvents.onAdRewardedEvent += OnRewarded;
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("[MonetizationManager] No rewarded ad available.");
            rewardedCallback = null;
            OnNoRewardedAdAvailable?.Invoke(); // let UI decide (usually close & GameOver)
        }
    }

    /// <summary>
    /// Legacy helper used by some code paths: returns whether a rewarded is ready.
    /// </summary>
    public bool CanContinue()
    {
        return IronSource.Agent.isRewardedVideoAvailable();
    }

    /// <summary>
    /// Legacy helper: “use a continue” now means “try to show rewarded ad”.
    /// </summary>
    public void UseContinue()
    {
        ShowRewardedAd(() => { /* handled by UI caller; we keep for back-compat */ });
    }

    // ---------------- Interstitial ----------------
    /// <summary>
    /// Legacy hook. Call this at real Game Over to maybe show interstitial.
    /// </summary>
    public void OnGameOver()
    {
        MaybeShowInterstitial();
    }

    public void MaybeShowInterstitial()
    {
        if (IsAdsRemoved()) return;

        gamesSinceInterstitial++;
        if (gamesSinceInterstitial >= Mathf.Max(1, interstitialEveryXGames))
        {
            if (IronSource.Agent.isInterstitialReady())
            {
                Debug.Log("Interstitial Ad show");
                IronSource.Agent.showInterstitial();
                gamesSinceInterstitial = 0;
                IronSource.Agent.loadInterstitial(); // prepare next
            }
            else
            {
                Debug.Log("Interstitial Ad not ready");
                IronSource.Agent.loadInterstitial();
            }
        }
        Debug.Log("Interstitial Ad end");
        SaveState();
    }

    // ---------------- IAP (Remove Ads only) ----------------
    private void InitializeIAP()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(REMOVE_ADS, ProductType.NonConsumable);
        UnityPurchasing.Initialize(this, builder);
    }

    public void PurchaseRemoveAds()
    {
        if (iapInitialized && storeController != null)
        {
            Debug.Log("[MonetizationManager] IAP REMOVE ADS started");
            storeController.InitiatePurchase(REMOVE_ADS);
        }
        else
        {
            Debug.LogError("[MonetizationManager] IAP not initialized.");
        }
    }

    public bool IsAdsRemoved() => adsRemoved;

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        iapInitialized = true;
        Debug.Log("[MonetizationManager] IAP Initialized.");
        foreach (var p in controller.products.all)
        {
            Debug.Log($"IAP Product available: {p.definition.id} ({p.definition.type})");
        }
    }

    // Unity IAP requires BOTH overloads for compatibility
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"[MonetizationManager] IAP Init Failed: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"[MonetizationManager] IAP Init Failed: {error} - {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (e.purchasedProduct.definition.id == REMOVE_ADS)
        {
            HandleRemoveAdsPurchased();
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError($"[MonetizationManager] IAP Purchase Failed: {product?.definition?.id} - {reason}");
    }

    // ---------------- Persistence ----------------
    private void HandleRemoveAdsPurchased()
    {
        adsRemoved = true;
        SaveState();
        Debug.Log("Remove Ads purchased — ads disabled.");

        OnAdsRemoved?.Invoke(true); // ✅ Notify listeners

        try { IronSource.Agent.hideBanner(); } catch { }
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt("MM_AdsRemoved", adsRemoved ? 1 : 0);
        PlayerPrefs.SetInt("MM_GamesSinceInterstitial", gamesSinceInterstitial);
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        adsRemoved = PlayerPrefs.GetInt("MM_AdsRemoved", 0) == 1;
        gamesSinceInterstitial = PlayerPrefs.GetInt("MM_GamesSinceInterstitial", 0);
    }

    // ---------------- Legacy events kept ----------------
    public static event Action OnContinueUsed;
}

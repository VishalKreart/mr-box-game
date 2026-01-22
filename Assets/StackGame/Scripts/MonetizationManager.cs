using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

[System.Serializable]
public class PurchaseCallbackWrapper
{
    public readonly System.Action<bool> Callback;
    public PurchaseCallbackWrapper(System.Action<bool> callback) => Callback = callback;
}

public class MonetizationManager : MonoBehaviour, IStoreListener
{
    public static MonetizationManager Instance;

    [Header("IronSource App Keys")]
    [SerializeField] private string androidAppKey = "YOUR_ANDROID_APP_KEY";
    [SerializeField] private string iosAppKey = "YOUR_IOS_APP_KEY";

    [Header("Ads")]
    [SerializeField] private int interstitialEveryXGames = 3;

    // IAP
    private const string NO_ADS_WEEKLY = "no_ads_weekly"; // Subscription

    private const string SAVE_TOWER = "save_tower"; // Consumable
    private IStoreController storeController;
    private bool iapInitialized;
    private Dictionary<string, PurchaseCallbackWrapper> purchaseCallbacks = new Dictionary<string, PurchaseCallbackWrapper>();

    // State
    private bool noAdsSubscriptionActive;
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

    /// <summary>
    /// Show rewarded ad specifically for saving the tower with rebalancing.
    /// </summary>
    public void ShowSaveTowerAd(System.Action onTowerSaved)
    {
        ShowRewardedAd(() => {
            Debug.Log("[MonetizationManager] Tower save ad completed successfully!");
            onTowerSaved?.Invoke();
            
            // Apply tower rebalancing effect
            TowerRebalancer rebalancer = FindObjectOfType<TowerRebalancer>();
            if (rebalancer != null)
            {
                rebalancer.RebalanceTower();
            }
            else
            {
                Debug.LogWarning("TowerRebalancer not found! Tower won't be rebalanced.");
            }
        });
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
        builder.AddProduct(NO_ADS_WEEKLY, ProductType.Subscription);
        UnityPurchasing.Initialize(this, builder);
    }

    public void PurchaseRemoveAds()
    {
        if (iapInitialized && storeController != null)
        {
            storeController.InitiatePurchase(NO_ADS_WEEKLY);
        }
        else
        {
            Debug.LogError("[MonetizationManager] IAP not initialized.");
        }
    }
    public void PurchaseNoAdsWeekly()
    {
        if (iapInitialized && storeController != null)
        {
            storeController.InitiatePurchase(NO_ADS_WEEKLY);
        }
    }


    public bool IsAdsRemoved() => noAdsSubscriptionActive;

    private void UpdateSubscriptionState(Product product)
    {
        if (product == null || !product.hasReceipt)
        {
            noAdsSubscriptionActive = false;
            return;
        }

#if UNITY_ANDROID || UNITY_IOS
    var subscriptionManager = new SubscriptionManager(product, null);
    var info = subscriptionManager.getSubscriptionInfo();

    noAdsSubscriptionActive =
        info.isSubscribed() == Result.True &&
        info.isExpired() == Result.False;
#else
        noAdsSubscriptionActive = false;
#endif
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        iapInitialized = true;

        var product = controller.products.WithID(NO_ADS_WEEKLY);
        UpdateSubscriptionState(product);

        if (noAdsSubscriptionActive)
        {
            Debug.Log("[MonetizationManager] No-Ads subscription active");
            OnAdsRemoved?.Invoke(true);
            try { IronSource.Agent.hideBanner(); } catch { }
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

    public void PurchaseProduct(string productId, Action<bool> onComplete)
    {
        if (!iapInitialized)
        {
            Debug.LogError("IAP not initialized");
            onComplete?.Invoke(false);
            return;
        }

        var product = storeController.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            var callback = new System.Action<bool>((success) =>
            {
                onComplete?.Invoke(success);
            });

            var wrapper = new PurchaseCallbackWrapper(callback);
            purchaseCallbacks[productId] = wrapper;
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.LogError($"Product {productId} not available for purchase");
            onComplete?.Invoke(false);
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var productId = args.purchasedProduct.definition.id;

        if (purchaseCallbacks.TryGetValue(productId, out var wrapper))
        {
            wrapper.Callback(true);
            purchaseCallbacks.Remove(productId);
        }

        if (productId == NO_ADS_WEEKLY)
        {
            UpdateSubscriptionState(args.purchasedProduct);
            OnAdsRemoved?.Invoke(noAdsSubscriptionActive);

            if (noAdsSubscriptionActive)
            {
                try { IronSource.Agent.hideBanner(); } catch { }
            }
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError($"[MonetizationManager] IAP Purchase Failed: {product?.definition?.id} - {reason}");
        if (product != null && purchaseCallbacks.TryGetValue(product.definition.id, out var wrapper))
        {
            wrapper.Callback(false);
            purchaseCallbacks.Remove(product.definition.id);
        }
    }
    public void RestorePurchases()
    {
#if UNITY_IOS
    if (!iapInitialized)
    {
        Debug.LogWarning("[RestorePurchases] IAP not initialized.");
        return;
    }

    var apple = storeController.extensions.GetExtension<IAppleExtensions>();
    Debug.Log("[RestorePurchases] Restoring purchases...");
    apple.RestoreTransactions(OnRestoreCompleted);
#else
        Debug.Log("[RestorePurchases] Restore not supported on this platform.");
#endif
    }
    private void OnRestoreCompleted(bool success, string message)
    {
//#if UNITY_IOS
    Debug.Log($"[RestorePurchases] Completed: {success}, {message}");

    if (success)
    {
            var product = storeController.products.WithID(NO_ADS_WEEKLY);
            UpdateSubscriptionState(product);
            if (noAdsSubscriptionActive)
            {
                Debug.Log("[MonetizationManager] No-Ads subscription active");
                OnAdsRemoved?.Invoke(true);
                try { IronSource.Agent.hideBanner(); } catch { }
            }
        }
//#endif
    }



    private void HandleRemoveAdsPurchased()
    {
        noAdsSubscriptionActive = true;
        SaveState();
        Debug.Log("Remove Ads purchased — ads disabled.");
        OnAdsRemoved?.Invoke(true);
        try { IronSource.Agent.hideBanner(); } catch { }
    }

    private void SaveState()
    {
        //PlayerPrefs.SetInt("MM_AdsRemoved", adsRemoved ? 1 : 0);
        PlayerPrefs.SetInt("MM_GamesSinceInterstitial", gamesSinceInterstitial);
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        //adsRemoved = PlayerPrefs.GetInt("MM_AdsRemoved", 0) == 1;
        gamesSinceInterstitial = PlayerPrefs.GetInt("MM_GamesSinceInterstitial", 0);
    }

    // Legacy events
    public static event Action OnContinueUsed;
}

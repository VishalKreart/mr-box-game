using Firebase.Analytics;
using System.Collections;
using Firebase;
using UnityEngine;

public class AnalyticsManager:MonoBehaviour
{
    public static AnalyticsManager Instance;

    private bool firebaseReady = false;
    private void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        StartCoroutine(InitFirebase());
#endif
    }

    private IEnumerator InitFirebase()
    {
        var task = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result == DependencyStatus.Available)
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            firebaseReady = true;

            Debug.Log("Firebase initialized");
        }
        else
        {
            Debug.LogError("Firebase dependency error: " + task.Result);
        }
    }

    public void LogEvent(string eventName)
    {
        #if UNITY_ANDROID || UNITY_IOS
        if (!firebaseReady) return;
        FirebaseAnalytics.LogEvent(eventName);
        #endif
    }

    public void LogEvent(string eventName, params Parameter[] parameters)
    {
        #if UNITY_ANDROID || UNITY_IOS
        if (!firebaseReady) return;

        FirebaseAnalytics.LogEvent(eventName, parameters);
        #endif

    }
}

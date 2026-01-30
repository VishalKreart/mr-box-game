using Firebase;
using Firebase.Analytics;
using UnityEngine;
using System.Collections;

public class FirebaseInit : MonoBehaviour
{
    private static bool initialized = false;

    private void Awake()
    {
        if (initialized)
        {
            Destroy(gameObject);
            return;
        }

        initialized = true;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(InitFirebase());
    }

    private IEnumerator InitFirebase()
    {
        var task = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result == DependencyStatus.Available)
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Log("✅ Firebase Analytics Initialized");
#endif
        }
        else
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogError($"❌ Firebase init failed: {task.Result}");
#endif
        }
    }
}

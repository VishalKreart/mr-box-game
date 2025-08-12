using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaManager : MonoBehaviour
{
    private RectTransform rectTransform;
    private ScreenOrientation lastOrientation;
    private Rect lastSafeArea;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        lastOrientation = Screen.orientation;
        lastSafeArea = Screen.safeArea;
        ApplySafeArea();
    }

    void Update()
    {
        if (Screen.orientation != lastOrientation || Screen.safeArea != lastSafeArea)
        {
            lastOrientation = Screen.orientation;
            lastSafeArea = Screen.safeArea;
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
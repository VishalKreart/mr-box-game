using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use this only if you use TextMeshPro for label

public class CustomToggleSwitch : MonoBehaviour
{
    public Toggle toggle;
    public Image handleImage;      // The colored slider image
    public TextMeshProUGUI label;  // Or use Text if not TMP
    public Sprite onSprite;
    public Sprite offSprite;
    public RectTransform handleTransform; // To move left/right

    public Vector2 onPosition;     // X position for ON
    public Vector2 offPosition;    // X position for OFF

    void Start()
    {
        toggle.onValueChanged.AddListener(OnToggleChanged);
        UpdateVisual(toggle.isOn);
    }

    void OnToggleChanged(bool isOn)
    {
        UpdateVisual(isOn);
    }

    void UpdateVisual(bool isOn)
    {
        handleImage.sprite = isOn ? onSprite : offSprite;
        handleImage.SetNativeSize();
        label.text = isOn ? "ON" : "OFF";
        handleTransform.anchoredPosition = isOn ? onPosition : offPosition;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class SwitchSlider : MonoBehaviour
{
    public Slider slider;
    public Image handleImage;
    public Sprite onSprite;   // orange
    public Sprite offSprite;  // blue

    public RectTransform handleRect;
    public Vector2 onPos;
    public Vector2 offPos;

    void Start()
    {
        slider.onValueChanged.AddListener(OnValueChanged);
        UpdateVisual(slider.value);
    }

    void OnValueChanged(float value)
    {
        bool isOn = value > 0.5f;
        slider.SetValueWithoutNotify(isOn ? 1 : 0);
        UpdateVisual(isOn ? 1 : 0);
    }

    void UpdateVisual(float value)
    {
        bool isOn = value == 1;
        handleImage.sprite = isOn ? onSprite : offSprite;
        handleImage.SetNativeSize();
        handleRect.anchoredPosition = isOn ? onPos : offPos;
    }
}

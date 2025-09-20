using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SliderConttroller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("VisualSlider")]
    [SerializeField] RectTransform visual;
    [SerializeField] float widthVisual;

    Slider slider;
    float speedReturn = 5f;

    public static bool IsHeld { get; set; } = false;

    public void OnPointerDown(PointerEventData eventData) => IsHeld = true;
    public void OnPointerUp(PointerEventData eventData) => IsHeld = false;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        ReturnToValueZero();
    }

    public void Visual()
    {
        float xCoord = widthVisual * slider.value / 2;
        visual.sizeDelta = new Vector2(Mathf.Abs(xCoord) * 2, visual.sizeDelta.y);
        visual.anchoredPosition = new Vector2(xCoord, visual.anchoredPosition.y);
    }

    void ReturnToValueZero()
    {
        if (IsHeld || slider.value == 0) return;

        slider.value = Mathf.Lerp(slider.value, 0, speedReturn * Time.deltaTime);
        if (Mathf.Abs(slider.value) < 0.002f)
        {
            slider.value = 0;
            return;
        }
    }
}

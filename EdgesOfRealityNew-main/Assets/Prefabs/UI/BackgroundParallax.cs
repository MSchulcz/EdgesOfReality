using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BackgroundParallax : MonoBehaviour
{
    public float parallaxAmount = 10f; // Максимальное смещение фона
    private RectTransform rectTransform;
    private Vector2 initialPosition;
    private Vector2 screenCenter;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 offset = (mousePosition - screenCenter) / screenCenter; // Нормализованное смещение мыши от центра экрана (-1..1)
        Vector2 parallaxOffset = -offset * parallaxAmount; // Смещение в противоположном направлении

        rectTransform.anchoredPosition = initialPosition + parallaxOffset;
    }
}

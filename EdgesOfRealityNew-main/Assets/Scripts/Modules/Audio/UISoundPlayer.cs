using UnityEngine;
using UnityEngine.EventSystems;
using Metroidvania.Audio;

public class UISoundPlayer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [System.Serializable]
    public struct UIEventSound
    {
        public string eventName; // Например, "ButtonClick", "ButtonHover"
        public AudioObject audioObject;
    }

    [SerializeField] private UIEventSound[] uiEventSounds;

    // Воспроизводит звук по имени события
    public void PlaySound(string eventName)
    {
        foreach (var sound in uiEventSounds)
        {
            if (sound.eventName == eventName)
            {
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlaySFX(sound.audioObject); // 2D звук для UI
                }
                else
                {
                    Debug.LogWarning("AudioManager не найден! Убедись, что он есть в сцене.");
                }
                return;
            }
        }
        Debug.LogWarning($"UI звук с именем {eventName} не найден на {gameObject.name}");
    }

    // Вызывается при наведении курсора на элемент UI
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound("ButtonHover");
    }

    // Вызывается при клике на элемент UI
    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound("ButtonClick");
    }
}
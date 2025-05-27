using UnityEngine;
using UnityEngine.EventSystems;
using Metroidvania.Audio;

public class UISoundPlayer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [System.Serializable]
    public struct UIEventSound
    {
        public string eventName; // событие, "ButtonClick", "ButtonHover"
        public AudioObject audioObject;
    }

    [SerializeField] private UIEventSound[] uiEventSounds;

    // воспроизведение звука по имени события
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
                    Debug.LogWarning("AudioManager не найден! Звук не будет воспроизведен.");
                }
                return;
            }
        }
        Debug.LogWarning($"UI звук с событием {eventName} не найден на {gameObject.name}");
    }

    // воспроизведение звука открытия меню
    public void PlayMenuOpenSound()
    {
        PlaySound("MenuOpen");
    }

    // воспроизведение звука закрытия меню
    public void PlayMenuCloseSound()
    {
        PlaySound("MenuClose");
    }

    // воспроизведение звука при наведении на UI элемент
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound("ButtonHover");
    }

    // воспроизведение звука при клике на UI элемент
    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound("ButtonClick");
    }
}

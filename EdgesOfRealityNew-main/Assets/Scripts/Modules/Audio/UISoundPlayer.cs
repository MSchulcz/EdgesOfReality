using UnityEngine;
using UnityEngine.EventSystems;
using Metroidvania.Audio;

public class UISoundPlayer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [System.Serializable]
    public struct UIEventSound
    {
        public string eventName; // ��������, "ButtonClick", "ButtonHover"
        public AudioObject audioObject;
    }

    [SerializeField] private UIEventSound[] uiEventSounds;

    // ������������� ���� �� ����� �������
    public void PlaySound(string eventName)
    {
        foreach (var sound in uiEventSounds)
        {
            if (sound.eventName == eventName)
            {
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlaySFX(sound.audioObject); // 2D ���� ��� UI
                }
                else
                {
                    Debug.LogWarning("AudioManager �� ������! �������, ��� �� ���� � �����.");
                }
                return;
            }
        }
        Debug.LogWarning($"UI ���� � ������ {eventName} �� ������ �� {gameObject.name}");
    }

    // ���������� ��� ��������� ������� �� ������� UI
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound("ButtonHover");
    }

    // ���������� ��� ����� �� ������� UI
    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound("ButtonClick");
    }
}
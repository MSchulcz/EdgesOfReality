using UnityEngine;

namespace Metroidvania.Audio
{
    public class SFXPlayer : MonoBehaviour
    {
        [System.Serializable]
        public struct SFXEvent
        {
            public string eventName; // Название события
            public AudioObject audioObject; // Связанный звук
        }

        [SerializeField] private SFXEvent[] sfxEvents; // Массив событий и звуков
        [SerializeField] private bool use3DSound = true; // Использовать 3D-звуки?

        // Проигрывает звук по имени события
        public void PlaySFX(string eventName)
        {
            foreach (var sfx in sfxEvents)
            {
                if (sfx.eventName == eventName)
                {
                    if (use3DSound)
                        AudioManager.instance.PlaySFXAtPosition(sfx.audioObject, transform.position);
                    else
                        AudioManager.instance.PlaySFX(sfx.audioObject);
                    return;
                }
            }
            Debug.LogWarning($"SFX с именем {eventName} не найден на {gameObject.name}");
        }
    }
}
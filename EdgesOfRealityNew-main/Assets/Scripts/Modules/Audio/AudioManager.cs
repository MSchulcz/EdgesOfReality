using UnityEngine;
using System.Collections.Generic;

namespace Metroidvania.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance; // Синглтон для легкого доступа
        [SerializeField] private int audioSourcePoolSize = 10; // Количество AudioSource в пуле
        private List<AudioSource> audioSources = new List<AudioSource>();
        private int currentSourceIndex = 0;

        private void Awake()
        {
            // Настраиваем синглтон
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Создаем пул AudioSource
            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                audioSources.Add(source);
            }
        }

        // Проигрывает звук в 2D
        public void PlaySFX(AudioObject audioObject)
        {
            if (audioObject == null || audioObject.clip == null)
            {
                Debug.LogWarning("AudioObject или AudioClip не указаны!");
                return;
            }

            AudioSource source = GetFreeAudioSource();
            audioObject.CloneToSource(source);
            source.spatialBlend = 0f; // 2D звук
            source.Play();
        }

        // Проигрывает звук в 3D на указанной позиции
        public void PlaySFXAtPosition(AudioObject audioObject, Vector3 position)
        {
            if (audioObject == null || audioObject.clip == null)
            {
                Debug.LogWarning("AudioObject или AudioClip не указаны!");
                return;
            }

            AudioSource source = GetFreeAudioSource();
            source.transform.position = position;
            audioObject.CloneToSource(source);
            source.Play();
        }

        private AudioSource GetFreeAudioSource()
        {
            // Ищем свободный AudioSource
            for (int i = 0; i < audioSources.Count; i++)
            {
                int index = (currentSourceIndex + i) % audioSources.Count;
                if (!audioSources[index].isPlaying)
                {
                    currentSourceIndex = (index + 1) % audioSources.Count;
                    return audioSources[index];
                }
            }

            // Если все заняты, создаем новый
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            audioSources.Add(newSource);
            currentSourceIndex = 0;
            return newSource;
        }
    }
}
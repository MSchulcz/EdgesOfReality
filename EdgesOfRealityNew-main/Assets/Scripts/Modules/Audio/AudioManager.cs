using UnityEngine;
using System.Collections.Generic;

namespace Metroidvania.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        [SerializeField] private int audioSourcePoolSize = 10;

        private List<AudioSource> audioSources = new List<AudioSource>();
        private int currentSourceIndex = 0;

        private void Awake()
        {
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

            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                audioSources.Add(source);
            }
        }

        public void PlaySFX(AudioObject audioObject)
        {
            if (audioObject == null || audioObject.clip == null)
            {
                Debug.LogWarning("AudioObject или AudioClip не назначены!");
                return;
            }

            AudioSource source = GetFreeAudioSource();
            audioObject.CloneToSource(source);
            source.spatialBlend = 0f;
            source.Play();
        }

        public void PlaySFXAtPosition(AudioObject audioObject, Vector3 position)
        {
            if (audioObject == null || audioObject.clip == null)
            {
                Debug.LogWarning("AudioObject или AudioClip не назначены!");
                return;
            }

            AudioSource source = GetFreeAudioSource();
            source.transform.position = position;
            audioObject.CloneToSource(source);
            source.Play();
        }

        private AudioSource GetFreeAudioSource()
        {
            for (int i = 0; i < audioSources.Count; i++)
            {
                int index = (currentSourceIndex + i) % audioSources.Count;
                if (!audioSources[index].isPlaying)
                {
                    currentSourceIndex = (index + 1) % audioSources.Count;
                    return audioSources[index];
                }
            }

            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            audioSources.Add(newSource);
            currentSourceIndex = 0;
            return newSource;
        }
    }
}
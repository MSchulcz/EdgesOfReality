using UnityEngine;
using System.Collections.Generic;

namespace Metroidvania.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance; // �������� ��� ������� �������
        [SerializeField] private int audioSourcePoolSize = 10; // ���������� AudioSource � ����
        private List<AudioSource> audioSources = new List<AudioSource>();
        private int currentSourceIndex = 0;

        private void Awake()
        {
            // ����������� ��������
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

            // ������� ��� AudioSource
            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                audioSources.Add(source);
            }
        }

        // ����������� ���� � 2D
        public void PlaySFX(AudioObject audioObject)
        {
            if (audioObject == null || audioObject.clip == null)
            {
                Debug.LogWarning("AudioObject ��� AudioClip �� �������!");
                return;
            }

            AudioSource source = GetFreeAudioSource();
            audioObject.CloneToSource(source);
            source.spatialBlend = 0f; // 2D ����
            source.Play();
        }

        // ����������� ���� � 3D �� ��������� �������
        public void PlaySFXAtPosition(AudioObject audioObject, Vector3 position)
        {
            if (audioObject == null || audioObject.clip == null)
            {
                Debug.LogWarning("AudioObject ��� AudioClip �� �������!");
                return;
            }

            AudioSource source = GetFreeAudioSource();
            source.transform.position = position;
            audioObject.CloneToSource(source);
            source.Play();
        }

        private AudioSource GetFreeAudioSource()
        {
            // ���� ��������� AudioSource
            for (int i = 0; i < audioSources.Count; i++)
            {
                int index = (currentSourceIndex + i) % audioSources.Count;
                if (!audioSources[index].isPlaying)
                {
                    currentSourceIndex = (index + 1) % audioSources.Count;
                    return audioSources[index];
                }
            }

            // ���� ��� ������, ������� �����
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            audioSources.Add(newSource);
            currentSourceIndex = 0;
            return newSource;
        }
    }
}
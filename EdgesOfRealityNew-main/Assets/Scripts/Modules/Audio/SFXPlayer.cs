using UnityEngine;

namespace Metroidvania.Audio
{
    public class SFXPlayer : MonoBehaviour
    {
        [System.Serializable]
        public struct SFXEvent
        {
            public string eventName; // �������� �������
            public AudioObject audioObject; // ��������� ����
        }

        [SerializeField] private SFXEvent[] sfxEvents; // ������ ������� � ������
        [SerializeField] private bool use3DSound = true; // ������������ 3D-�����?

        // ����������� ���� �� ����� �������
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
            Debug.LogWarning($"SFX � ������ {eventName} �� ������ �� {gameObject.name}");
        }
    }
}
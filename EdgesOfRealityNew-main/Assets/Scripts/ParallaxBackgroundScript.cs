using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Tooltip("������, ������������ ������� ����� ����������� ���������")]
    public Transform cameraTransform;

    [Tooltip("����������� ����������. ������ 1 - ��� �������� ��������� ������")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    private Vector3 previousCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        previousCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;
        Vector3 newPosition = transform.position + new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * parallaxFactor, 0);
        transform.position = newPosition;
        previousCameraPosition = cameraTransform.position;
    }
}

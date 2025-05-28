using UnityEngine;
using Metroidvania.Characters.Knight;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private float healthRestoreAmount = 20f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        KnightCharacterController player = collision.GetComponent<KnightCharacterController>();
        if (player != null)
        {
            player.RestoreHealth(healthRestoreAmount);
            Destroy(gameObject);
        }
    }
}

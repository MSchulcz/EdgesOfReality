using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    private float maxHealth;

    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void SetHealth(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class SpriteToggleOnKeyPress : MonoBehaviour
{
    [Header("Sprites or GameObjects to toggle")]
    public UnityEngine.Object sprite1;
    public UnityEngine.Object sprite2;

    [Header("Toggle settings")]
    public float toggleSpeed = 0.5f; // seconds per sprite
    public Key triggerKey = Key.E; // Key to hide the object

    [Header("Trigger settings")]
    public Collider2D triggerArea;
    public string playerTag = "Player";

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private bool showingFirstSprite = true;
    private bool playerInTrigger = false;

    private Sprite GetSpriteFromObject(UnityEngine.Object obj)
    {
        if (obj == null)
            return null;

        if (obj is Sprite sprite)
            return sprite;

        if (obj is GameObject go)
        {
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
                return sr.sprite;
        }

        Debug.LogWarning("Object is not a Sprite or GameObject with SpriteRenderer: " + obj.name);
        return null;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteToggleOnKeyPress requires a SpriteRenderer component on the same GameObject.");
            enabled = false;
            return;
        }

        if (GetSpriteFromObject(sprite1) == null || GetSpriteFromObject(sprite2) == null)
        {
            Debug.LogError("Please assign both sprite1 and sprite2 as Sprite or GameObject with SpriteRenderer in the inspector.");
            enabled = false;
            return;
        }

        spriteRenderer.sprite = GetSpriteFromObject(sprite1);

        // Проверяем, находится ли игрок уже в триггере при старте
        if (triggerArea != null)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(triggerArea.bounds.center, triggerArea.bounds.size, 0f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag(playerTag))
                {
                    playerInTrigger = true;
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (triggerArea != null && !playerInTrigger)
            return;

        // Toggle sprites based on timer
        timer += Time.deltaTime;
        if (timer >= toggleSpeed)
        {
            timer = 0f;
            showingFirstSprite = !showingFirstSprite;
            spriteRenderer.sprite = showingFirstSprite ? GetSpriteFromObject(sprite1) : GetSpriteFromObject(sprite2);
        }

        // Check for key press to hide object
        if (Keyboard.current != null && Keyboard.current[triggerKey].wasPressedThisFrame && playerInTrigger)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerArea == null)
            return;

        if (other == triggerArea)
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (triggerArea == null)
            return;

        if (other == triggerArea)
        {
            playerInTrigger = false;
        }
    }
}

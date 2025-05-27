using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class AdvancedDialogTrigger : MonoBehaviour
{
    [Header("Диалоговая система")]
    public GameObject dialogUI; // Панель диалога
    public Text dialogText;
    public Button nextButton;
    public string[] dialogLines;
    public float typingSpeed = 0.05f;

    [Header("Настройки триггера")]
    public Transform player;
    public float triggerDistance = 3f;
    public bool showOnlyOnce = true;
    public bool requireManualActivation = false; // Нужно ли нажимать кнопку для старта
    public Key activationKey = Key.E; // Клавиша активации (новая система ввода)

    [Header("UI для подсказки")]
    public TextMeshProUGUI promptTextUI; // UI элемент для отображения подсказки

    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool wasShown = false;
    private bool playerInRange = false;

    private CanvasGroup dialogCanvasGroup;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (dialogUI != null)
        {
            dialogCanvasGroup = dialogUI.GetComponent<CanvasGroup>();
            if (dialogCanvasGroup == null)
            {
                dialogCanvasGroup = dialogUI.AddComponent<CanvasGroup>();
            }
            dialogUI.SetActive(false);
            dialogCanvasGroup.alpha = 0f;
        }

        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);

        if (promptTextUI != null)
            promptTextUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= triggerDistance;

        // Автоматический или ручной запуск
        if (playerInRange)
        {
            if (!requireManualActivation && !dialogUI.activeSelf && !(wasShown && showOnlyOnce))
            {
                StartDialog();
            }
            else if (requireManualActivation && Keyboard.current != null && Keyboard.current[activationKey].wasPressedThisFrame && !dialogUI.activeSelf && !(wasShown && showOnlyOnce))
            {
                StartDialog();
            }
        }
        else if (dialogUI.activeSelf)
        {
            CloseDialog();
        }

        // Обработка нажатия пробела для перехода к следующей строке диалога
        if (dialogUI.activeSelf && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextLine();
        }

        // Обновление текста подсказки и видимости
        if (promptTextUI != null)
        {
            if (playerInRange && requireManualActivation && !dialogUI.activeSelf && !(wasShown && showOnlyOnce))
            {
                promptTextUI.text = $"Нажмите {activationKey} для разговора";
                promptTextUI.gameObject.SetActive(true);
            }
            else
            {
                promptTextUI.gameObject.SetActive(false);
            }
        }
    }

    void StartDialog()
    {
        if (dialogLines == null || dialogLines.Length == 0)
        {
            Debug.LogError("Нет строк диалога!");
            return;
        }

        wasShown = true;
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        dialogUI.SetActive(true);
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(dialogCanvasGroup, 0f, 1f, 0.5f));
        currentLine = 0;
        typingCoroutine = StartCoroutine(TypeText(dialogLines[currentLine]));
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = dialogLines[currentLine];
            isTyping = false;
            return;
        }

        if (currentLine < dialogLines.Length - 1)
        {
            currentLine++;
            typingCoroutine = StartCoroutine(TypeText(dialogLines[currentLine]));
        }
        else
        {
            CloseDialog();
        }
    }

    void CloseDialog()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutAndDeactivate());
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        cg.alpha = start;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    IEnumerator FadeOutAndDeactivate()
    {
        yield return FadeCanvasGroup(dialogCanvasGroup, 1f, 0f, 0.5f);
        dialogUI.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}

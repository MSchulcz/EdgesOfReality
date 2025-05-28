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

    void Start()
    {
        if (dialogUI != null)
            dialogUI.SetActive(false);

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

        // Обработка пропуска диалога по пробелу
        if (dialogUI.activeSelf && Keyboard.current != null && Keyboard.current[Key.Space].wasPressedThisFrame)
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

    public void StartDialog()
    {
        if (dialogLines == null || dialogLines.Length == 0)
        {
            Debug.LogError("Нет строк диалога!");
            return;
        }

        wasShown = true;
        dialogUI.SetActive(true);
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
        dialogUI.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}
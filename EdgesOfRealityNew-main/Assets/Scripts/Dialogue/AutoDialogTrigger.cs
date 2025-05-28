using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;

public class AutoDialogTrigger : MonoBehaviour
{
    [Header("Диалоговая система")]
    public GameObject dialogUI; // Панель диалога
    public TextMeshProUGUI dialogText;
    public string[] dialogLines;
    public float typingSpeed = 0.05f;
    public float delayAfterLine = 3f; // Задержка после полного отображения строки

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

    public bool isDialogLocked = false; // Новый флаг для блокировки закрытия диалога

    void Start()
    {
        if (dialogUI != null)
            dialogUI.SetActive(false);

        if (promptTextUI != null)
            promptTextUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= triggerDistance;

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
        else if (dialogUI.activeSelf && !isDialogLocked)
        {
            CloseDialog();
        }

        if (promptTextUI != null)
        {
            if (playerInRange && requireManualActivation && !dialogUI.activeSelf && !(wasShown && showOnlyOnce))
            {
                promptTextUI.text = $"Нажмите {activationKey} для продолжения";
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

        Debug.Log("AutoDialogTrigger: StartDialog called, showing dialog UI.");
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

        // Ждём delayAfterLine секунд, затем переключаем строку
        yield return new WaitForSeconds(delayAfterLine);

        NextLine();
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
}

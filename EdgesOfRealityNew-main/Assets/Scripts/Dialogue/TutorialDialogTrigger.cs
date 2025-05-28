using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using TMPro;
using Metroidvania.Characters.Knight;

public class TutorialDialogTrigger : MonoBehaviour
{
    [Header("Диалоговая система")]
    public GameObject dialogUI; // Панель диалога
    public Text dialogText;
    public Button nextButton;
    public string[] dialogLines = new string[]
    {
        "Добро пожаловать в игру!",
        "Используйте клавиши WASD или стрелки для перемещения.",
        "Нажмите пробел для прыжка.",
        "Нажмите E для взаимодействия с объектами.",
        "Удачи в игре!"
    };
    public float typingSpeed = 0.05f;

    [Header("Настройки триггера")]
    public Transform player;
    public float triggerDistance = 3f;
    public bool showOnlyOnce = true;
    public bool requireManualActivation = false; // Автоматический старт диалога
    public Key activationKey = Key.E; // Клавиша активации (если requireManualActivation = true)

    [Header("UI для подсказки")]
    public TextMeshProUGUI promptTextUI; // UI элемент для отображения подсказки

    private int currentLine = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private bool wasShown = false;
    private bool playerInRange = false;

    [Header("Блокировка управления игроком")]
    public KnightCharacterController playerController; // Ссылка на скрипт управления игроком

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

        if (dialogUI.activeSelf && Keyboard.current != null && Keyboard.current[Key.Space].wasPressedThisFrame)
        {
            NextLine();
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

    void StartDialog()
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

        if (playerController != null)
        {
            playerController.enabled = false; // Блокируем управление игроком
        }
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

        if (playerController != null)
        {
            playerController.enabled = true; // Разблокируем управление игроком
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}

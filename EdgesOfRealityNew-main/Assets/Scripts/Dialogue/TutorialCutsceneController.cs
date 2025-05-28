using System.Collections;
using UnityEngine;
using Metroidvania.Characters.Knight;
using TMPro;
using UnityEngine.UI;

public class TutorialCutsceneController : MonoBehaviour
{
    public KnightCharacterController playerController;
    public Transform playerSpawnPoint;
    public Transform girlTransform;
    public AutoDialogTrigger advancedDialogTrigger;

    public float lookAroundDuration = 2f;
    public float moveSpeed = 2f;

    private bool isCutscenePlaying = false;

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        isCutscenePlaying = true;

        // Заблокировать управление игроком
        if (playerController != null)
            playerController.enabled = false;

        // Появление игрока в позиции
        if (playerController != null && playerSpawnPoint != null)
        {
            playerController.transform.position = playerSpawnPoint.position;
            playerController.FlipFacingDirection(1); // Вправо по умолчанию
        }

        // Разворот влево-вправо (осмотр)
        yield return StartCoroutine(LookAround());

        // Движение игрока к девушке
        yield return StartCoroutine(MoveToGirl());

        // Запуск диалога без проверки расстояния (убираем возможный конфликт с AutoDialogTrigger)
        if (advancedDialogTrigger != null)
        {
            advancedDialogTrigger.isDialogLocked = true; // Блокируем закрытие диалога
            advancedDialogTrigger.StartDialog();
        }

        // Ждём пока диалог не закончится
        while (advancedDialogTrigger != null && advancedDialogTrigger.dialogUI.activeSelf)
        {
            yield return null;
        }

        if (advancedDialogTrigger != null)
        {
            advancedDialogTrigger.isDialogLocked = false; // Разблокируем закрытие диалога
        }

        // Разблокировать управление игроком
        if (playerController != null)
            playerController.enabled = true;

        isCutscenePlaying = false;
    }

    private IEnumerator LookAround()
    {
        if (playerController == null)
            yield break;

        // Взгляд влево
        playerController.FlipFacingDirection(-1);
        yield return new WaitForSeconds(lookAroundDuration / 2f);

        // Взгляд вправо
        playerController.FlipFacingDirection(1);
        yield return new WaitForSeconds(lookAroundDuration / 2f);
    }

    private IEnumerator MoveToGirl()
    {
        if (playerController == null || girlTransform == null)
            yield break;

        Vector3 startPosition = playerController.transform.position;
        Vector3 targetPosition = girlTransform.position;

        // Оставляем небольшой отступ (например, 0.5f) по X, чтобы не доходить до девушки полностью
        float stopDistance = 0.5f;
        Vector3 direction = (targetPosition - startPosition).normalized;

        // Корректируем целевую позицию с отступом по X, фиксируем Y на уровне игрока
        Vector3 adjustedTarget = new Vector3(targetPosition.x + stopDistance, startPosition.y, startPosition.z);

        // Направление движения влево
        playerController.FlipFacingDirection(-1);

        // Включаем анимацию бега
        playerController.SwitchAnimation(KnightCharacterController.RunAnimHash, true);

        float distance = Vector3.Distance(startPosition, adjustedTarget);
        float elapsed = 0f;

        while (elapsed < distance / moveSpeed)
        {
            elapsed += Time.deltaTime;
            // Линейно интерполируем позицию по X, фиксируем Y
            float newX = Mathf.Lerp(startPosition.x, adjustedTarget.x, elapsed * moveSpeed / distance);
            playerController.transform.position = new Vector3(newX, startPosition.y, startPosition.z);
            yield return null;
        }

        // Останавливаемся на позиции с отступом
        playerController.transform.position = adjustedTarget;

        // Включаем анимацию Idle
        playerController.SwitchAnimation(KnightCharacterController.IdleAnimHash, true);
    }
}

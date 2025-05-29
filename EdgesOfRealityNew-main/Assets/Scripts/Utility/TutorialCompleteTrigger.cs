using UnityEngine;
using Metroidvania.SceneManagement;
using Metroidvania.Serialization;

public class TutorialCompleteTrigger : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // Устанавливаем флажок, что туториал пройден
            DataManager.instance.gameData.tutorialCompleted = true;

            // Переходим в главное меню
            SceneLoader.instance.LoadMainMenu();
        }
    }
}

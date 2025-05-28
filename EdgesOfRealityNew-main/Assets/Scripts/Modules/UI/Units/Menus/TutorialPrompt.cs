using Metroidvania.SceneManagement;
using Metroidvania.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Metroidvania.UI.Menus
{
    public class TutorialPrompt : MonoBehaviour
    {
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        [SerializeField] private AssetReferenceSceneChannel tutorialScene;
        [SerializeField] private AssetReferenceSceneChannel firstLevelScene;

        private Action onYesCallback;
        private Action onNoCallback;
        private Action onTutorialCompleteCallback;

        public AssetReferenceSceneChannel TutorialScene => tutorialScene;
        public AssetReferenceSceneChannel FirstLevelScene => firstLevelScene;

        private void Awake()
        {
            promptPanel.SetActive(false);
            yesButton.onClick.AddListener(OnYesClicked);
            noButton.onClick.AddListener(OnNoClicked);
        }

        public void SetCallbacks(Action yesCallback, Action noCallback)
        {
            onYesCallback = yesCallback;
            onNoCallback = noCallback;
        }

        public void SetTutorialCompleteCallback(Action tutorialCompleteCallback)
        {
            onTutorialCompleteCallback = tutorialCompleteCallback;
        }

        public void ShowPrompt()
        {
            Debug.Log("TutorialPrompt ShowPrompt called");
            if (!DataManager.instance.gameData.tutorialCompleted)
            {
                Debug.Log("Showing tutorial prompt panel");
                promptPanel.SetActive(true);
                Debug.Log($"promptPanel active state: {promptPanel.activeSelf}");
            }
            else
            {
                Debug.Log("Tutorial already completed, loading first level");
                SceneLoader.instance.LoadScene(firstLevelScene, SceneLoader.SceneTransitionData.UseGameData);
            }
        }

        private void OnYesClicked()
        {
            promptPanel.SetActive(false);
            onYesCallback?.Invoke();
        }

        private void OnNoClicked()
        {
            promptPanel.SetActive(false);
            onNoCallback?.Invoke();
        }

        public void NotifyTutorialComplete()
        {
            onTutorialCompleteCallback?.Invoke();
        }
    }
}

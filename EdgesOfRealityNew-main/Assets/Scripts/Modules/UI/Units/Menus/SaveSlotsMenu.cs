using Metroidvania.InputSystem;
using Metroidvania.SceneManagement;
using Metroidvania.UI;
using Metroidvania.UI.Menus;
using UnityEngine;

namespace Metroidvania.Serialization.Menus
{
    public class SaveSlotsMenu : CanvasMenuBase, IMenuScreen
    {
        [SerializeField] private CanvasGroup m_canvasGroup;

        [SerializeField] private AssetReferenceSceneChannel m_sceneLevel0;

        [SerializeField] private TutorialPrompt tutorialPrompt;

        public event System.Action OnMenuDisable;
        private SaveSlot[] _saveSlots;

        private int _selectedUserId;
        private GameData _selectedGameData;

        private void Start()
        {
            _saveSlots = GetComponentsInChildren<SaveSlot>(true);

            foreach (SaveSlot saveSlot in _saveSlots)
            {
                GameData saveUserData = DataManager.instance.dataHandler.Deserialize(saveSlot.GetUserId());
                saveSlot.SetData(saveUserData);
                saveSlot.button.onClick.AddListener(() => OnSaveSlotClick(saveSlot));
            }

            if (tutorialPrompt != null)
            {
                tutorialPrompt.SetCallbacks(OnTutorialYes, OnTutorialNo);
                tutorialPrompt.SetTutorialCompleteCallback(OnTutorialComplete);
            }
        }

        public void OnSaveSlotClick(SaveSlot saveSlot)
        {
            _selectedUserId = saveSlot.GetUserId();
            _selectedGameData = saveSlot.GetData();

            if (tutorialPrompt != null && !_selectedGameData.tutorialCompleted)
            {
                tutorialPrompt.ShowPrompt();
            }
            else
            {
                StartGame();
            }
        }

        private void OnTutorialYes()
        {
            DataManager.instance.ChangeSelectedUser(_selectedUserId);
            // Do not set tutorialCompleted here; set after tutorial completion
            // DataManager.instance.gameData.tutorialCompleted = true;
            // DataManager.instance.SerializeData();

            SceneLoader.instance.LoadScene(tutorialPrompt.TutorialScene, SceneLoader.SceneTransitionData.UseGameData);
            InputReader.instance.EnableGameplayInput();
        }

        private void OnTutorialNo()
        {
            DataManager.instance.ChangeSelectedUser(_selectedUserId);
            DataManager.instance.gameData.tutorialCompleted = true;
            DataManager.instance.SerializeData();

            if (_selectedGameData != null)
            {
                _selectedGameData.LoadCurrentScene();
            }
            else
            {
                SceneLoader.instance.LoadScene(tutorialPrompt.FirstLevelScene, SceneLoader.SceneTransitionData.UseGameData);
            }
            InputReader.instance.EnableGameplayInput();
        }

        private void OnTutorialComplete()
        {
            DataManager.instance.gameData.tutorialCompleted = true;
            DataManager.instance.SerializeData();
        }

        private void StartGame()
        {
            if (_selectedGameData != null)
            {
                _selectedGameData.LoadCurrentScene();
            }
            else
            {
                SceneLoader.instance.LoadScene(m_sceneLevel0, SceneLoader.SceneTransitionData.UseGameData);
            }
            InputReader.instance.EnableGameplayInput();
        }

        public void ActiveMenu()
        {
            menuEnabled = true;
            m_canvasGroup.FadeGroup(true, Helpers.TransitionTime, SetFirstSelected);
        }

        public void DesactiveMenu()
        {
            menuEnabled = false;
            m_canvasGroup.FadeGroup(false, Helpers.TransitionTime, () => OnMenuDisable?.Invoke());
        }
    }
}

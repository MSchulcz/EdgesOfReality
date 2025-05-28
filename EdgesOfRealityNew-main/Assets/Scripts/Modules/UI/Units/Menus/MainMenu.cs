using Metroidvania.Credits;
using Metroidvania.InputSystem;
using Metroidvania.Serialization.Menus;
using TMPro;
using UnityEngine;

namespace Metroidvania.UI.Menus
{
    public class MainMenu : CanvasMenuBase
    {
        [Header("Menu Transition")]
        [SerializeField] private CanvasGroup m_mainTitleGroup;
        [SerializeField] private OptionsMenu m_optionsMenu;
        [SerializeField] private SaveSlotsMenu m_saveSlotsMenu;
        [SerializeField] private CreditsMenu m_creditsMenu;

        [SerializeField] private TextMeshProUGUI m_versionText;

        [SerializeField] private TutorialPrompt tutorialPrompt;

        public IMenuScreen activeScreen;

        [SerializeField] private bool autoShowTutorialPromptOnStart = false;

        private void Awake()
        {
            m_versionText.text = Application.version;
            m_optionsMenu.OnMenuDisable += ActiveMenu;
            m_saveSlotsMenu.OnMenuDisable += ActiveMenu;
            m_creditsMenu.OnMenuDisable += ActiveMenu;
            InputReader.instance.MenuCloseEvent += PerformMenuClose;
            InputReader.instance.EnableMenuInput();
        }

        private void Start()
        {
            Debug.Log("MainMenu Start called");
            SetFirstSelected();
            if (autoShowTutorialPromptOnStart)
            {
                StartGame();
            }
        }

        private void OnDestroy()
        {
            InputReader.instance.MenuCloseEvent -= PerformMenuClose;
        }

        public void ShowOptions() => SwitchToScreen(m_optionsMenu);

        public void ShowSaveSlots() => SwitchToScreen(m_saveSlotsMenu);

        public void ShowCredits() => SwitchToScreen(m_creditsMenu);

        public void SwitchToScreen(IMenuScreen screen)
        {
            m_mainTitleGroup.FadeGroup(false, Helpers.TransitionTime, screen.ActiveMenu);
            menuEnabled = false;
            activeScreen = screen;
        }

        public void ActiveMenu()
        {
            activeScreen = null;
            menuEnabled = true;
            m_mainTitleGroup.FadeGroup(true, Helpers.TransitionTime, SetFirstSelected);
        }

        private void PerformMenuClose()
        {
            activeScreen?.DesactiveMenu();
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void StartGame()
        {
            if (tutorialPrompt == null)
            {
                Debug.LogError("tutorialPrompt reference is null in MainMenu");
                return;
            }
            Debug.Log("Calling tutorialPrompt.ShowPrompt()");
            tutorialPrompt.ShowPrompt();
        }
    }
}

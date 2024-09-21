using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private GameObject settingsMenu;
        [SerializeField] private GameObject aboutMenu;

        private static VisualElement _root;

        private static Button _startButton;
        private static Button _settingsButton;
        private static Button _aboutButton;
        private static Button _quitButton;

        private static readonly Color HoverColor = new Color(22f / 255f, 24f / 255f, 49 / 255f, 0.5f);

        public void OnEnable()
        {
            _root = uiDocument.rootVisualElement;

            _startButton = GetButton("Play");
            _settingsButton = GetButton("Settings");
            _aboutButton = GetButton("About");
            _quitButton = GetButton("Quit");

            _startButton.clicked += OnStartButtonClicked;
            _settingsButton.clicked += OnSettingsButtonClicked;
            _aboutButton.clicked += OnAboutButtonClicked;
            _quitButton.clicked += OnQuitButtonClicked;
        }

        private void OnStartButtonClicked()
        {
            LevelManager.Instance.LoadScene("ChargedUp", "CrossFade");
        }

        private void OnSettingsButtonClicked()
        {
            settingsMenu.SetActive(true);
            gameObject.SetActive(false);
        }

        private void OnAboutButtonClicked()
        {
            aboutMenu.SetActive(true);
            gameObject.SetActive(false);
        }

        private static void OnQuitButtonClicked()
        {
            Application.Quit();
        }
        
        public static Button GetButton(string name)
        {
            Button button = _root.Query<Button>(name);
            
            button.RegisterCallback<MouseOverEvent>(evt => button.style.backgroundColor = new StyleColor(HoverColor));
            button.RegisterCallback<MouseOutEvent>(evt => button.style.backgroundColor = new StyleColor(Color.clear));
            
            button.style.borderTopColor = new StyleColor(Color.clear);
            button.style.borderRightColor = new StyleColor(Color.clear);
            button.style.borderBottomColor = new StyleColor(Color.clear);
            button.style.borderLeftColor = new StyleColor(Color.clear);
            
            return button;
        }
    }
}
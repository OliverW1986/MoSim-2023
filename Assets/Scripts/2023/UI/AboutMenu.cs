using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class AboutMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private RobotSelector rs;
        
        private static VisualElement _root;
        
        private static Button _backButton;
        
        public void OnEnable()
        {
            _root = uiDocument.rootVisualElement;

            _backButton = _root.Query<Button>("BackButton");
        }
    }
}
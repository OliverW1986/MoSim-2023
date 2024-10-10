using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.LowLevelPhysics;
using UnityEngine.UIElements;

namespace UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private RobotSelector rs;

        [SerializeField] private Sprite coneImg;
        [SerializeField] private Sprite cubeImg;

        private static VisualElement _root;

        private static Button _backButton;
        private static Button _resetButton;

        private static DropdownField _gamemodeSelect;
        private static DropdownField _cameraSelect;
        private static DropdownField _allianceSelect;

        private VisualElement _singleplayerConfig;
        private VisualElement _2V0Config;
        private VisualElement _1V1Config;

        private static DropdownField _singleRobotSelect;
        private TextField _singleRobotName;
        private static Slider _moveSpeed;
        private static Slider _turnSpeed;
        private Button _gamePieceSelectorBtn;

        private VisualElement _gamePieceSelector;
        private Button _gamePieceSelectorBackBtn;

        private Button _red1Btn;
        private Button _red2Btn;
        private Button _red3Btn;
        private Button _red4Btn;

        private Button _blue1Btn;
        private Button _blue2Btn;
        private Button _blue3Btn;
        private Button _blue4Btn;

        private Dictionary<Button, bool> _buttonStates = new Dictionary<Button, bool>();

        private DropdownField _displaySelect;
        private DropdownField _windowModeSelect;
        private DropdownField _resolutionSelect;
        private DropdownField _fpsLimitSelect;

        private DropdownField _graphicsPreset;
        // private DropdownField _textureQuality;
        // private DropdownField _antiAliasing;
        // private DropdownField _shadowQuality;
        // private DropdownField _fieldOfView;

        private Slider _masterVolume;
        private Toggle _swerveSounds;
        private Toggle _bumpSounds;
        private Toggle _intakeSounds;

        private VisualElement _backgroundImage;
        private GameObject currentRobot;
        [SerializeField] private CinemachineCamera botCam;

        public void OnEnable()
        {
            _root = uiDocument.rootVisualElement;
            _backButton = _root.Query<Button>("BackButton");
            _resetButton = _root.Query<Button>("ResetButton");

            _backButton.clicked += OnBackButtonClicked;
            _resetButton.clicked += ResetPlayerPrefs;

            _gamemodeSelect = _root.Query<DropdownField>("Gamemode");
            _cameraSelect = _root.Query<DropdownField>("Camera");
            _allianceSelect = _root.Query<DropdownField>("Alliance");

            _singleplayerConfig = _root.Query<VisualElement>("SingleRobotConfig");
            _singleRobotSelect = _root.Query<DropdownField>("Robot");
            _singleRobotName = _root.Query<TextField>("BumperNumber");
            _moveSpeed = _root.Query<Slider>("MoveSpeed");
            _turnSpeed = _root.Query<Slider>("TurnSpeed");
            _gamePieceSelectorBtn = _root.Query<Button>("GamePieceSelectorButton");
            _gamePieceSelectorBtn.clicked += () =>
            {
                _gamePieceSelector.style.display = DisplayStyle.Flex;
                _singleplayerConfig.style.display = DisplayStyle.None;
            };

            _1V1Config = _root.Query<VisualElement>("1V1RobotConfig");

            _2V0Config = _root.Query<VisualElement>("2V0RobotConfig");

            _gamePieceSelector = _root.Query<VisualElement>("GamePieceSelector");
            _gamePieceSelectorBackBtn = _root.Query<Button>("GamePieceSelectorBackButton");
            _gamePieceSelectorBackBtn.clicked += () =>
            {
                _gamePieceSelector.style.display = DisplayStyle.None;
                _singleplayerConfig.style.display = DisplayStyle.Flex;
            };

            _red1Btn = _root.Query<Button>("Red1");
            _red2Btn = _root.Query<Button>("Red2");
            _red3Btn = _root.Query<Button>("Red3");
            _red4Btn = _root.Query<Button>("Red4");

            _blue1Btn = _root.Query<Button>("Blue1");
            _blue2Btn = _root.Query<Button>("Blue2");
            _blue3Btn = _root.Query<Button>("Blue3");
            _blue4Btn = _root.Query<Button>("Blue4");

            _backgroundImage = _root.Query<VisualElement>("SettingsContainer");

            InitializeButtonState(_red1Btn, "Red1");
            InitializeButtonState(_red2Btn, "Red2");
            InitializeButtonState(_red3Btn, "Red3");
            InitializeButtonState(_red4Btn, "Red4");

            InitializeButtonState(_blue1Btn, "Blue1");
            InitializeButtonState(_blue2Btn, "Blue2");
            InitializeButtonState(_blue3Btn, "Blue3");
            InitializeButtonState(_blue4Btn, "Blue4");

            _red1Btn.clicked += () => TogglePiece(_red1Btn, "Red1");
            _red2Btn.clicked += () => TogglePiece(_red2Btn, "Red2");
            _red3Btn.clicked += () => TogglePiece(_red3Btn, "Red3");
            _red4Btn.clicked += () => TogglePiece(_red4Btn, "Red4");

            _blue1Btn.clicked += () => TogglePiece(_blue1Btn, "Blue1");
            _blue2Btn.clicked += () => TogglePiece(_blue2Btn, "Blue2");
            _blue3Btn.clicked += () => TogglePiece(_blue3Btn, "Blue3");
            _blue4Btn.clicked += () => TogglePiece(_blue4Btn, "Blue4");

            _gamemodeSelect.index = PlayerPrefs.GetInt("gamemode");
            _gamemodeSelect.RegisterValueChangedCallback(_ =>
            {
                PlayerPrefs.SetInt("gamemode", _gamemodeSelect.index);
                _allianceSelect.style.display = _gamemodeSelect.index == 2 ? DisplayStyle.None : DisplayStyle.Flex;
                _singleplayerConfig.style.display = _gamemodeSelect.index != 0 ? DisplayStyle.None : DisplayStyle.Flex;
                _1V1Config.style.display = _gamemodeSelect.index != 1 ? DisplayStyle.None : DisplayStyle.Flex;
                _2V0Config.style.display = _gamemodeSelect.index != 2 ? DisplayStyle.None : DisplayStyle.Flex;
            });

            _cameraSelect.index = PlayerPrefs.GetInt("cameraMode", 1);
            _cameraSelect.RegisterValueChangedCallback(_ => { PlayerPrefs.SetInt("cameraMode", _cameraSelect.index); });

            _allianceSelect.index = PlayerPrefs.GetString("alliance", "blue") switch
            {
                "blue" => 0,
                "red" => 1,
                _ => 0
            };

            _allianceSelect.RegisterValueChangedCallback(_ =>
            {
                PlayerPrefs.SetString("alliance", _allianceSelect.index == 0 ? "blue" : "red");
            });

            _singleRobotSelect.choices = rs.GetRobotList().Select(robot => robot.robotName).ToList();
            _singleRobotSelect.index = PlayerPrefs.GetInt("blueRobotSettings");

            if(currentRobot != null)
            {
                Destroy(currentRobot);

            }
            try
            {
                currentRobot = SpawnSafeRobot(rs.GetRobotList()[_singleRobotSelect.index].robotPrefab);

            } catch (Exception e)
            {

            }


            _singleRobotSelect.RegisterValueChangedCallback(_ =>
            {
                PlayerPrefs.SetInt("blueRobotSettings", _singleRobotSelect.index);
                PlayerPrefs.SetString(PlayerPrefs.GetString("alliance").Equals("blue") ? "blueName" : "redName", rs.GetRobotList()[_singleRobotSelect.index].robotNumber.ToString());
                _singleRobotName.value =
                    PlayerPrefs.GetString(PlayerPrefs.GetString("alliance").Equals("blue") ? "blueName" : "redName",
                        rs.GetRobotList()[_singleRobotSelect.index].robotNumber.ToString());
                PlayerPrefs.Save();

                Destroy(currentRobot);

                try
                {
                    currentRobot = SpawnSafeRobot(rs.GetRobotList()[_singleRobotSelect.index].robotPrefab);

                }
                catch (Exception e)
                {

                }
            });

            _singleRobotName.value =
                PlayerPrefs.GetString(PlayerPrefs.GetString("alliance").Equals("blue") ? "blueName" : "redName",
                    rs.GetRobotList()[_singleRobotSelect.index].robotNumber.ToString());
            _singleRobotName.RegisterValueChangedCallback(_ =>
            {
                PlayerPrefs.SetString(PlayerPrefs.GetString("alliance").Equals("blue") ? "blueName" : "redName", _singleRobotName.value);
                PlayerPrefs.Save();
            });

            _moveSpeed.value = PlayerPrefs.GetFloat("moveSpeed", 90f);
            _moveSpeed.RegisterValueChangedCallback(_ => { PlayerPrefs.SetFloat("moveSpeed", _moveSpeed.value); });

            _turnSpeed.value = PlayerPrefs.GetFloat("turnSpeed", 100f);
            _turnSpeed.RegisterValueChangedCallback(_ => { PlayerPrefs.SetFloat("turnSpeed", _turnSpeed.value); });

            _displaySelect = _root.Query<DropdownField>("TargetDisplay");
            _windowModeSelect = _root.Query<DropdownField>("WindowMode");
            _resolutionSelect = _root.Query<DropdownField>("Resolution");
            _fpsLimitSelect = _root.Query<DropdownField>("FPSLimit");

            _displaySelect.RegisterCallback<ChangeEvent<int>>(_ =>
            {
                if (Camera.main != null) Camera.main.targetDisplay = _displaySelect.index;
                Display.displays[_displaySelect.index].Activate();
            });

            var savedWindowMode = PlayerPrefs.GetInt("WindowMode", 1);
            _windowModeSelect.index = savedWindowMode;
            ApplyWindowMode(savedWindowMode);
            _windowModeSelect.RegisterValueChangedCallback(_ =>
            {
                ApplyWindowMode(_windowModeSelect.index);
                PlayerPrefs.SetInt("WindowMode", _windowModeSelect.index);
                PlayerPrefs.Save();
            });

            // _resolutionSelect.choices =
            //     Screen.resolutions.Select(resolution => $"{resolution.width}x{resolution.height}").ToList();

            _resolutionSelect.index = PlayerPrefs.GetInt("Resolution", 0);
            _resolutionSelect.RegisterCallback<ChangeEvent<int>>(_ =>
            {
                // var resolutions = Screen.resolutions;
                var resolutions = new[]
                {
                    new Resolution { width = 1920, height = 1080 },
                    new Resolution { width = 1280, height = 720 },
                    new Resolution { width = 1024, height = 576 },
                    new Resolution { width = 800, height = 600 },
                    new Resolution { width = 640, height = 480 }
                };
                var resolution = resolutions[_resolutionSelect.index];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
                PlayerPrefs.SetInt("Resolution", _resolutionSelect.index);
                PlayerPrefs.Save();
            });

            _fpsLimitSelect.index = PlayerPrefs.GetInt("FPSLimit", 0);
            _fpsLimitSelect.RegisterCallback<ChangeEvent<int>>(_ =>
            {
                switch (_fpsLimitSelect.index)
                {
                    case 0:
                        Application.targetFrameRate = -1;
                        QualitySettings.vSyncCount = 0;
                        break;
                    case 1:
                        Application.targetFrameRate = -1;
                        QualitySettings.vSyncCount = 1;
                        break;
                    case 2:
                        Application.targetFrameRate = 30;
                        break;
                    case 3:
                        Application.targetFrameRate = 60;
                        break;
                    case 4:
                        Application.targetFrameRate = 120;
                        break;
                    case 5:
                        Application.targetFrameRate = 240;
                        break;
                }

                PlayerPrefs.SetInt("FPSLimit", _fpsLimitSelect.index);
                PlayerPrefs.Save();
            });

            _graphicsPreset = _root.Query<DropdownField>("GraphicsPreset");
            // _textureQuality = _root.Query<DropdownField>("Textures");
            // _antiAliasing = _root.Query<DropdownField>("Anti-Aliasing");
            // _shadowQuality = _root.Query<DropdownField>("Shadows");
            // _fieldOfView = _root.Query<DropdownField>("FOV");

            _graphicsPreset.index = PlayerPrefs.GetInt("GraphicsPreset", 0);
            QualitySettings.SetQualityLevel(_graphicsPreset.index, true);
            _graphicsPreset.RegisterValueChangedCallback(_ =>
            {
                switch (_graphicsPreset.index)
                {
                    case 0:
                        QualitySettings.SetQualityLevel(3, true);
                        // _textureQuality.index = 4;
                        // _antiAliasing.index = 4;
                        // _shadowQuality.index = 4;
                        break;
                    case 1:
                        QualitySettings.SetQualityLevel(2, true);
                        // _textureQuality.index = 3;
                        // _antiAliasing.index = 3;
                        // _shadowQuality.index = 3;
                        break;
                    case 2:
                        QualitySettings.SetQualityLevel(1, true);
                        // _textureQuality.index = 2;
                        // _antiAliasing.index = 2;
                        // _shadowQuality.index = 2;
                        break;
                    case 3:
                        QualitySettings.SetQualityLevel(0, true);
                        // _textureQuality.index = 2;
                        // _antiAliasing.index = 2;
                        // _shadowQuality.index = 2;
                        break;
                    // case 4:
                    //     QualitySettings.SetQualityLevel(5, true);
                    //     // _textureQuality.index = 0;
                    //     // _antiAliasing.index = 0;
                    //     // _shadowQuality.index = 0;
                    //     break;
                }

                PlayerPrefs.SetInt("GraphicsPreset", _graphicsPreset.index);
            });

            // _textureQuality.index = PlayerPrefs.GetInt("TextureQuality", 2);
            // _textureQuality.RegisterValueChangedCallback(_ =>
            // {
            //     QualitySettings.SetQualityLevel(5, true);
            //     _graphicsPreset.index = 5;
            //     QualitySettings.globalTextureMipmapLimit = _textureQuality.index switch
            //     {
            //         0 => 0,
            //         1 => 1,
            //         2 => 2,
            //         3 => 3,
            //         _ => QualitySettings.globalTextureMipmapLimit
            //     };
            //     PlayerPrefs.SetInt("TextureQuality", _textureQuality.index);
            // });
            //
            // _antiAliasing.index = PlayerPrefs.GetInt("AntiAliasing", 2);
            // _antiAliasing.RegisterValueChangedCallback(_ =>
            // {
            //     QualitySettings.SetQualityLevel(5, true);
            //     _graphicsPreset.index = 5;
            //     QualitySettings.antiAliasing = _antiAliasing.index switch
            //     {
            //         0 => 0,
            //         1 => 2,
            //         2 => 4,
            //         3 => 8,
            //         _ => QualitySettings.antiAliasing
            //     };
            //     PlayerPrefs.SetInt("AntiAliasing", _antiAliasing.index);
            // });
            //
            // _shadowQuality.index = PlayerPrefs.GetInt("ShadowQuality", 2);
            // _shadowQuality.RegisterValueChangedCallback(_ =>
            // {
            //     QualitySettings.SetQualityLevel(5, true);
            //     _graphicsPreset.index = 5;
            //     QualitySettings.shadows = _shadowQuality.index switch
            //     {
            //         0 => ShadowQuality.Disable,
            //         1 => ShadowQuality.HardOnly,
            //         2 => ShadowQuality.All,
            //         _ => QualitySettings.shadows
            //     };
            //     PlayerPrefs.SetInt("ShadowQuality", _shadowQuality.index);
            // });
            //
            // _fieldOfView.index = PlayerPrefs.GetInt("FieldOfView", 3);
            // _fieldOfView.RegisterValueChangedCallback(_ =>
            // {
            //     QualitySettings.SetQualityLevel(5, true);
            //     _graphicsPreset.index = 5;
            //     Camera.main.fieldOfView = _fieldOfView.index switch
            //     {
            //         0 => 60,
            //         1 => 70,
            //         2 => 80,
            //         3 => 90,
            //         4 => 100,
            //         5 => 110,
            //         6 => 120,
            //         _ => Camera.main.fieldOfView
            //     };
            //     PlayerPrefs.SetInt("FieldOfView", _fieldOfView.index);
            // });

            _masterVolume = _root.Query<Slider>("MasterVolume");
            _swerveSounds = _root.Query<Toggle>("SwerveSounds");
            _bumpSounds = _root.Query<Toggle>("BumpSounds");
            _intakeSounds = _root.Query<Toggle>("IntakeSounds");

            _masterVolume.value = PlayerPrefs.GetFloat("MasterVolume", 1);
            _masterVolume.RegisterValueChangedCallback(_ =>
            {
                PlayerPrefs.SetFloat("MasterVolume", _masterVolume.value);
                AudioListener.volume = _masterVolume.value;
                PlayerPrefs.Save();
            });

            _swerveSounds.value = PlayerPrefs.GetInt("SwerveSounds", 1) == 1;
            _swerveSounds.RegisterValueChangedCallback(_ =>
            {
                PlayerPrefs.SetInt("SwerveSounds", _swerveSounds.value ? 1 : 0);
                PlayerPrefs.Save();
            });

            _bumpSounds.value = PlayerPrefs.GetInt("BumpSounds", 1) == 1;
            _bumpSounds.RegisterValueChangedCallback(_ =>
            {
                PlayerPrefs.SetInt("BumpSounds", _bumpSounds.value ? 1 : 0);
                PlayerPrefs.Save();
            });

            _intakeSounds.value = PlayerPrefs.GetInt("IntakeSounds", 1) == 1;
            _intakeSounds.RegisterValueChangedCallback(_ =>
            {
                PlayerPrefs.SetInt("IntakeSounds", _intakeSounds.value ? 1 : 0);
                PlayerPrefs.Save();
            });
        }

        private void FixedUpdate()
        {
            if (currentRobot != null)
            {
                currentRobot.transform.Rotate(Vector3.up, 0.05f);
            }
        }

        public GameObject SpawnSafeRobot(GameObject robot)
        {
            GameObject bot = Instantiate(robot);

            bot.GetComponent<Rigidbody>().isKinematic = true;
            bot.GetComponent<DriveController>().enabled = false;
            bot.GetComponent<GamePieceManager>().enabled = false;
            bot.GetComponent<BackToMenuButton>().enabled = false;
            bot.GetComponent<BackToMenuButton>().enabled = false;

            bot.GetComponentsInChildren<CinemachineBrain>()[0].enabled = false;
            bot.GetComponentsInChildren<CinemachineCamera>()[0].enabled = false;

            if(!GameManager.canRobotMove)
            {
                GameManager.ToggleCanRobotMove();
            }

            botCam.Target.TrackingTarget = bot.transform;

            return bot;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackButtonClicked();
            }
        }

        private void OnBackButtonClicked()
        {
            mainMenu.SetActive(true);
            gameObject.SetActive(false);
        }

        private void InitializeButtonState(Button button, string key)
        {
            int savedState = PlayerPrefs.GetInt(key, 0);
            bool isCone = savedState == 0;

            _buttonStates[button] = isCone;
            button.style.backgroundImage = new StyleBackground(isCone ? coneImg : cubeImg);
        }

        private void TogglePiece(Button target, string key)
        {
            bool isCone = _buttonStates[target];
            if (isCone)
            {
                target.style.backgroundImage = new StyleBackground(cubeImg);
                _buttonStates[target] = false;
                PlayerPrefs.SetInt(key, 1);
            }
            else
            {
                target.style.backgroundImage = new StyleBackground(coneImg);
                _buttonStates[target] = true;
                PlayerPrefs.SetInt(key, 0);
            }

            PlayerPrefs.Save();
        }

        private void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetInt("gamemode", 0);
            PlayerPrefs.SetInt("cameraMode", 1);
            PlayerPrefs.SetString("alliance", "blue");
            PlayerPrefs.SetInt("blueRobotSettings", 0);
            PlayerPrefs.SetFloat("moveSpeed", 90f);
            PlayerPrefs.SetFloat("turnSpeed", 100f);

            PlayerPrefs.SetInt("WindowMode", 1);
            PlayerPrefs.SetInt("FPSLimit", 0);
            PlayerPrefs.SetInt("Resolution", 0);
            PlayerPrefs.SetInt("GraphicsPreset", 0);

            PlayerPrefs.SetFloat("MasterVolume", 1);
            PlayerPrefs.SetInt("SwerveSounds", 1);
            PlayerPrefs.SetInt("BumpSounds", 1);
            PlayerPrefs.SetInt("IntakeSounds", 1);

            PlayerPrefs.SetInt("Red1", 1);
            PlayerPrefs.SetInt("Red2", 0);
            PlayerPrefs.SetInt("Red3", 0);
            PlayerPrefs.SetInt("Red4", 1);

            PlayerPrefs.SetInt("Blue1", 1);
            PlayerPrefs.SetInt("Blue2", 0);
            PlayerPrefs.SetInt("Blue3", 0);
            PlayerPrefs.SetInt("Blue4", 1);

            PlayerPrefs.Save();

            OnEnable();
        }

        private void ApplyWindowMode(int index)
        {
            Screen.fullScreenMode = index switch
            {
                0 => FullScreenMode.ExclusiveFullScreen,
                1 => FullScreenMode.FullScreenWindow,
                2 => FullScreenMode.Windowed,
                _ => Screen.fullScreenMode
            };
        }
    }
}
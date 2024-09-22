using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DriveController : MonoBehaviour
{
    [SerializeField] private TMP_Text[] bumperNumbers;
    public bool reverseBumperAllianceText;

    [SerializeField] private Transform[] rayCastPoints;
    [SerializeField] private float rayCastDistance;
    [SerializeField] private bool flipRayCastDir;

    //Handles climbing logic
    public bool isGrounded = true;

    public AudioSource robotPlayer;
    public AudioSource treadPlayer;
    public AudioSource gearPlayer;
    public AudioResource intakeSound;
    public AudioResource swerveSound;
    public AudioResource gearSound;
    public float moveSpeed = 20f;
    public float rotationSpeed = 15f;
    public bool isRedRobot;

    public static bool isTouchingWallColliderRed;
    public static bool isTouchingWallColliderBlue;
    private Vector3 velocity { get; set; }
    private bool canIntake { get; set; }
    public static bool robotsTouching;
    public static bool isPinningRed;
    public static bool isPinningBlue;
    public bool isIntaking;
    public bool intakeForce;

    private Rigidbody _rb;
    private Vector2 _translateValue;
    private float _rotateValue;
    private Vector3 _startingDirection;
    private Vector3 _startingRotation;
    public float intakeValue;

    private bool _dontPlayDriveSounds;
    private bool _useSwerveSounds;
    private bool _useIntakeSounds;

    public Material materialPrefab;
    public GameObject[] bumpers;
    private Material _bumperMat;
    private Color _defaultBumperColor;

    public float beforeVelocity;
    private bool _dontUpdateBeforeVelocity;

    [SerializeField] private float maxAngularVelocity = 5f;
    private CameraMode _cameraMode = CameraMode.ThirdPerson;

    [SerializeField] private GameObject thirdPersonCamera;
    [SerializeField] private GameObject firstPersonCamera;

    private Vector3 _startingPos;
    private Quaternion _startingRot;

    private InputAction _translateAction;
    private InputAction _rotateAction;
    private InputAction _groundIntakeAction;
    private InputAction _substationIntakeAction;
    private InputAction _restartAction;

    private GameObject _panCamera;

    public bool inCommunity = true;
    private bool _parkScored;

    private ChargeStation _chargeStation;
    private RobotSpawnController _robotSpawnController;
    private GameObject _cineMachine;

    public bool canRotate = true;
    public bool canTranslate = true;

    protected void Start()
    {
        canIntake = true;

        _startingPos = transform.position;
        _startingRot = transform.rotation;

        if (materialPrefab != null)
        {
            _bumperMat = Instantiate(materialPrefab);

            foreach (var bumper in bumpers)
            {
                Material[] materials = bumper.GetComponent<MeshRenderer>().materials;
                var mats = new List<Material>();
                for(int i = 0; i< materials.Length; i++)
                {
                    mats.Add(_bumperMat);
                }

                bumper.GetComponent<MeshRenderer>().SetMaterials(mats);
            }

            _defaultBumperColor = _bumperMat.color;
        }
        else
        {
            Debug.LogError("Material prefab is not assigned!");
        }

        if (!reverseBumperAllianceText)
        {
            if (isRedRobot && PlayerPrefs.GetString("redName") != "")
            {
                foreach (var bumperNumber in bumperNumbers)
                {
                    bumperNumber.text = PlayerPrefs.GetString("redName");
                }
            }
            else if (!isRedRobot && PlayerPrefs.GetString("blueName") != "")
            {
                foreach (var bumperNumber in bumperNumbers)
                {
                    bumperNumber.text = PlayerPrefs.GetString("blueName");
                }
            }
        }
        else
        {
            if (isRedRobot && PlayerPrefs.GetString("blueName") != "")
            {
                foreach (var bumperNumber in bumperNumbers)
                {
                    bumperNumber.text = PlayerPrefs.GetString("blueName");
                }
            }
            else if (!isRedRobot && PlayerPrefs.GetString("redName") != "")
            {
                foreach (var bumperNumber in bumperNumbers)
                {
                    bumperNumber.text = PlayerPrefs.GetString("redName");
                }
            }
        }

        _useSwerveSounds = PlayerPrefs.GetInt("SwerveSounds") == 1;
        _useIntakeSounds = PlayerPrefs.GetInt("IntakeSounds") == 1;

        treadPlayer.resource = swerveSound;
        treadPlayer.loop = true;

        gearPlayer.resource = gearSound;
        gearPlayer.loop = true;

        // moveSpeed -= (moveSpeed * (-PlayerPrefs.GetFloat("moveSpeed") / 100f));
        // rotationSpeed -= (rotationSpeed * (-PlayerPrefs.GetFloat("rotateSpeed") / 100f));

        isTouchingWallColliderRed = false;
        isTouchingWallColliderBlue = false;

        isPinningRed = false;
        isPinningBlue = false;
        robotsTouching = false;
        velocity = new Vector3(0f, 0f, 0f);
        isIntaking = false;

        //Initializing starting transforms
        _rb = GetComponent<Rigidbody>();

        _startingDirection = gameObject.transform.forward;
        _startingRotation = gameObject.transform.right;

        _panCamera = GameObject.FindGameObjectWithTag("PanCam");

        _translateAction = InputSystem.actions.FindAction("Translate");
        _rotateAction = InputSystem.actions.FindAction("Rotate");
        _groundIntakeAction = InputSystem.actions.FindAction("IntakeGround");
        _substationIntakeAction = InputSystem.actions.FindAction("IntakeDoubleSubstation");
        _restartAction = InputSystem.actions.FindAction("Restart");

        _robotSpawnController = FindFirstObjectByType<RobotSpawnController>();
        _cineMachine = GameObject.Find("CinemachineBrain");
        
        _cameraMode = (CameraMode)PlayerPrefs.GetInt("cameraMode");
        switch (_cameraMode)
        {
            case CameraMode.DriverStation:
                SetCameraMode(CameraMode.DriverStation);
                if (isRedRobot)
                {
                    _robotSpawnController.redPanCam.SetActive(true);
                }
                else
                {
                    _robotSpawnController.bluePanCam.SetActive(true);
                }
                _cineMachine.SetActive(false);
                thirdPersonCamera.SetActive(false);
                firstPersonCamera.SetActive(false);
                break;
            case CameraMode.ThirdPerson:
                SetCameraMode(CameraMode.ThirdPerson);
                thirdPersonCamera.SetActive(true);
                firstPersonCamera.SetActive(false);
                break;
            case CameraMode.FirstPerson:
                SetCameraMode(CameraMode.FirstPerson);
                thirdPersonCamera.SetActive(false);
                firstPersonCamera.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _chargeStation = isRedRobot
            ? GameObject.Find("RedChargeStation").GetComponentInChildren<ChargeStation>()
            : GameObject.Find("BlueChargeStation").GetComponentInChildren<ChargeStation>();
    }

    private void Update()
    {
        isGrounded = CheckGround();

        if (!_dontUpdateBeforeVelocity)
        {
            if (!isTouchingWallColliderBlue && !isRedRobot || !isTouchingWallColliderRed && isRedRobot)
            {
                beforeVelocity = _rb.linearVelocity.magnitude;
            }
        }

        if (!isRedRobot)
        {
            if (robotsTouching && isTouchingWallColliderBlue)
            {
                isPinningBlue = true;
            }
            else
            {
                isPinningBlue = false;
            }
        }
        else
        {
            if (robotsTouching && isTouchingWallColliderRed)
            {
                isPinningRed = true;
            }
            else
            {
                isPinningRed = false;
            }
        }

        if ((_groundIntakeAction.ReadValue<float>() > 0.25f || _substationIntakeAction.ReadValue<float>() > 0.25f) &&
            GameManager.canRobotMove && canIntake)
        {
            robotPlayer.resource = intakeSound;
            isIntaking = true;
        }
        else
        {
            isIntaking = false;
        }

        if (_useIntakeSounds)
        {
            switch (isIntaking)
            {
                case true when !robotPlayer.isPlaying:
                    robotPlayer.Play();
                    break;
                case false when robotPlayer.isPlaying:
                    robotPlayer.Stop();
                    break;
            }
        }

        if (_useSwerveSounds)
        {
            var isMovingOrRotating = Math.Abs(Math.Round(velocity.x)) > 0f || Math.Abs(Math.Round(velocity.z)) > 0f ||
                                     Math.Abs(_rotateValue) > 0f;

            if (isMovingOrRotating && !_dontPlayDriveSounds)
            {
                PlaySwerveSounds();
            }
            else
            {
                StopSwerveSounds();
            }
        }

        if (_restartAction.triggered)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (!_parkScored)
        {
            if (inCommunity && GameManager.GameState == GameState.Endgame && !_chargeStation.isScored &&
                !_chargeStation.isDockedScored)
            {
                _parkScored = true;
                Score.AddScore(2, isRedRobot ? Alliance.Red : Alliance.Blue);
            }
        }
        else if (_parkScored && (!inCommunity || _chargeStation.isScored || _chargeStation.isDockedScored))
        {
            _parkScored = false;
            Score.SubScore(2, isRedRobot ? Alliance.Red : Alliance.Blue);
        }
    }

    protected void FixedUpdate()
    {
        if (GameManager.canRobotMove)
        {
            if (isGrounded)
            {
                _dontPlayDriveSounds = false;

                Vector3 moveDirection;
                _translateValue = _translateAction.ReadValue<Vector2>();
                if (_cameraMode == CameraMode.DriverStation)
                {
                    _translateValue.x *= -1f;
                    _translateValue.y *= -1f;
                }
                
                _translateValue.x = Mathf.Sign(_translateValue.x) * Mathf.Pow(Mathf.Abs(_translateValue.x), 2f);
                _translateValue.y = Mathf.Sign(_translateValue.y) * Mathf.Pow(Mathf.Abs(_translateValue.y), 2f);

                _rotateValue = canRotate ? _rotateAction.ReadValue<float>() : 0;

                if (_cameraMode != CameraMode.FirstPerson)
                {
                    moveDirection = _startingDirection * _translateValue.y + _startingRotation * _translateValue.x;
                }
                else
                {
                    moveDirection = transform.forward * _translateValue.y + transform.right * _translateValue.x;
                }

                if(canTranslate)
                {
                    _rb.AddForce(moveDirection * moveSpeed);
                }

                if(canRotate)
                {
                    _rb.AddTorque(new Vector3(0f, _rotateValue * rotationSpeed, 0f));
                    _rb.angularVelocity = Vector3.ClampMagnitude(_rb.angularVelocity, maxAngularVelocity);
                }
                

                velocity = _rb.linearVelocity;
            }
            else
            {
                _dontPlayDriveSounds = true;
            }
        }
        else
        {
            if (_useSwerveSounds)
            {
                _dontPlayDriveSounds = true;
                StopSwerveSounds();
            }
        }
    }

    public void forceIntake(bool value)
    {
        intakeForce = value;
    }

    private void PlaySwerveSounds()
    {
        var velocityFactor = Mathf.Clamp01(velocity.magnitude / 100f);
        var accelerationFactor = Mathf.Clamp(1f + (velocity.magnitude / 100f), 1f, 2f);

        var rotationFactor = Mathf.Clamp01(Mathf.Abs(_rotateValue) / 200f);

        var volume = velocityFactor + (rotationFactor * 10f);

        var pitch = Mathf.Max(accelerationFactor, rotationFactor);

        treadPlayer.volume = volume * 0.8f;
        treadPlayer.pitch = pitch * 0.7f;
        gearPlayer.volume = volume * 0.5f;

        if (!treadPlayer.isPlaying && !gearPlayer.isPlaying)
        {
            gearPlayer.Play();
            treadPlayer.Play();
        }
    }

    private void StopSwerveSounds()
    {
        if (treadPlayer.isPlaying || gearPlayer.isPlaying)
        {
            treadPlayer.Stop();
            gearPlayer.Stop();
        }
    }

    public IEnumerator GrayOutBumpers(float duration)
    {
        _bumperMat.color = Color.gray;

        yield return new WaitForSeconds(duration);
        _bumperMat.color = _defaultBumperColor;
    }

    public void Reset()
    {
        StopAllCoroutines();

        //Reset bumper colors
        _bumperMat.color = _defaultBumperColor;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        //Reset position
        _rb.MovePosition(_startingPos);
        _rb.MoveRotation(_startingRot);
    }

    public void OnTranslate(InputAction.CallbackContext ctx)
    {
        _translateValue = ctx.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        _rotateValue = ctx.ReadValue<float>();
    }

    public void OnIntake(InputAction.CallbackContext ctx)
    {
        intakeValue = ctx.ReadValue<float>();
    }

    public void OnRestart(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator ResetMatch()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
        GameManager.ResetMatch();
    }

    private bool CheckGround()
    {
        var distanceToTheGround = rayCastDistance;
        foreach (var rayCastPoint in rayCastPoints)
        {
            if (!flipRayCastDir)
            {
                if (Physics.Raycast(rayCastPoint.position, -transform.up, distanceToTheGround))
                {
                    return true;
                }
            }
            else
            {
                if (Physics.Raycast(rayCastPoint.position, transform.up, distanceToTheGround))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRedRobot)
        {
            if (other.gameObject.CompareTag("RedPlayer"))
            {
                robotsTouching = true;
            }
            else if (other.gameObject.CompareTag("Field") || other.gameObject.CompareTag("Wall"))
            {
                _dontUpdateBeforeVelocity = true;
                isTouchingWallColliderBlue = true;
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                robotsTouching = true;
            }
            else if (other.gameObject.CompareTag("Field") || other.gameObject.CompareTag("Wall"))
            {
                _dontUpdateBeforeVelocity = true;
                isTouchingWallColliderRed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isRedRobot)
        {
            if (other.gameObject.CompareTag("RedPlayer"))
            {
                robotsTouching = false;
            }
            else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Field"))
            {
                _dontUpdateBeforeVelocity = false;
                if (!isRedRobot)
                {
                    isTouchingWallColliderBlue = false;
                }
                else
                {
                    isTouchingWallColliderRed = false;
                }
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                robotsTouching = false;
            }
            else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Field"))
            {
                _dontUpdateBeforeVelocity = false;
                if (!isRedRobot)
                {
                    isTouchingWallColliderBlue = false;
                }
                else
                {
                    isTouchingWallColliderRed = false;
                }
            }
        }
    }

    public void SetCameraMode(CameraMode cameraMode)
    {
        _cameraMode = cameraMode;
    }

    public enum CameraMode
    {
        DriverStation,
        ThirdPerson,
        FirstPerson
    }
}
using Mechanisms;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class StrikeZoneArm : MonoBehaviour
{
    public HingeJoint lowerArm;
    public HingeJoint upperArm;

    [SerializeField] public float lowerSpeed;
    [SerializeField] private float upperSpeed;

    [SerializeField] private float stowLowerTarget;
    [SerializeField] private float stowUpperTarget;
    [SerializeField] private float shelfLowerTarget;
    [SerializeField] private float shelfUpperTarget;
    [SerializeField] private float groundLowerTarget;
    [SerializeField] private float groundUpperTarget;
    [SerializeField] private float transferLowerTarget;
    [SerializeField] private float transferUpperTarget;
    [SerializeField] private float highConeLowerTarget;
    [SerializeField] private float highConeUpperTarget;
    [SerializeField] private float highCubeLowerTarget;
    [SerializeField] private float highCubeUpperTarget;
    [SerializeField] private float midConeLowerTarget;
    [SerializeField] private float midConeUpperTarget;
    [SerializeField] private float midCubeLowerTarget;
    [SerializeField] private float midCubeUpperTarget;
    [SerializeField] private float lowLowerTarget;
    [SerializeField] private float lowUpperTarget;

    [SerializeField] private float intermediateLower;
    [SerializeField] private float intermediateUpper;

    private JointSpring lowerArmSpring;
    private JointSpring upperArmSpring;

    private float lowerTarget = 0;
    private float upperTarget = 0;

    public RobotState _previousRobotState;
    public RobotState _currentRobotState;

    private GamePieceManager _robotGamePieceManager;

    private InputAction _stowAction;
    private InputAction _highAction;
    private InputAction _middleAction;
    private InputAction _lowAction;
    private InputAction _intakeDoubleSubstationAction;
    private InputAction _intakeGroundAction;
    private InputAction _scoreAction;

    public bool isTransferring = false;
    public bool isPlacing = false;

    public float startAngle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _robotGamePieceManager = GetComponent<GamePieceManager>();

        lowerArmSpring = lowerArm.spring;
        upperArmSpring = upperArm.spring;

        _stowAction = InputSystem.actions.FindAction("Stow");
        _highAction = InputSystem.actions.FindAction("High");
        _middleAction = InputSystem.actions.FindAction("Middle");
        _lowAction = InputSystem.actions.FindAction("Low");
        _intakeDoubleSubstationAction = InputSystem.actions.FindAction("IntakeDoubleSubstation");
        _intakeGroundAction = InputSystem.actions.FindAction("IntakeGround");

        _scoreAction = InputSystem.actions.FindAction("Place");

        startAngle = upperArm.transform.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.canRobotMove && !isTransferring)
        {
            if (_stowAction.triggered && _currentRobotState != RobotState.Stow)
            {
                _currentRobotState = RobotState.Stow;

                StartCoroutine(safeRetract(stowLowerTarget, stowUpperTarget));

            }
            else if(_intakeGroundAction.WasReleasedThisFrame() || _intakeDoubleSubstationAction.WasReleasedThisFrame())
            {
                lowerTarget = stowLowerTarget;
                upperTarget = stowUpperTarget;
            }
            else if (_highAction.WasPressedThisFrame() && _currentRobotState != RobotState.High)
            {
                _currentRobotState = RobotState.High;

                if (_robotGamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                {
                    upperTarget = highCubeUpperTarget;

                    StartCoroutine(safeGoToPos(highCubeLowerTarget, highCubeUpperTarget));
                }
                else
                {
                    StartCoroutine(safeGoToPos(highConeLowerTarget, highConeUpperTarget));
                }

            }
            else if (_middleAction.WasPressedThisFrame() && _currentRobotState != RobotState.Middle)
            {
                _currentRobotState = RobotState.Middle;

                if (_robotGamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                {

                    StartCoroutine(safeGoToPos(midCubeLowerTarget, midCubeUpperTarget));
                }
                else
                {
                    StartCoroutine(safeGoToPos(midConeLowerTarget, midConeUpperTarget));


                }

            }
            else if (_lowAction.triggered && _currentRobotState != RobotState.Low)
            {
                lowerTarget = lowLowerTarget;
                upperTarget = lowUpperTarget;

                _currentRobotState = RobotState.Low;
            }
            else if (_intakeDoubleSubstationAction.triggered)
            {
                if (_robotGamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                {
                    lowerTarget = shelfLowerTarget;
                    upperTarget = shelfUpperTarget;
                }
                else
                {
                    lowerTarget = shelfLowerTarget;
                    upperTarget = shelfUpperTarget;
                }

                _currentRobotState = RobotState.IntakeDoubleSubstation;
            }
            else if (_intakeGroundAction.triggered)
            {
                lowerTarget = groundLowerTarget;
                upperTarget = groundUpperTarget;

                _currentRobotState = RobotState.IntakeGround;
            }
        }

        if(
            _scoreAction.ReadValue<float>() > 0 
            && !isPlacing 
            && _robotGamePieceManager.hasGamePiece 
            && (_currentRobotState == RobotState.Middle || _currentRobotState == RobotState.High) 
            && _robotGamePieceManager.currentGamePiece == GamePieceType.Cone
            )
        {
            StartCoroutine(PlaceSequence());
        }

        lowerArmSpring.targetPosition = Mathf.MoveTowards(lowerArmSpring.targetPosition, lowerTarget, lowerSpeed * Time.deltaTime);
        upperArmSpring.targetPosition = Mathf.MoveTowards(upperArmSpring.targetPosition, upperTarget, upperSpeed * Time.deltaTime);

        lowerArm.spring = lowerArmSpring;
        upperArm.spring = upperArmSpring;

        _previousRobotState = _currentRobotState;

    }

    public IEnumerator PlaceSequence()
    {
        _robotGamePieceManager.canPlace = false;
        isPlacing = true;
        upperTarget -= 15;

        yield return new WaitForSeconds(0.15f);

        _robotGamePieceManager.canPlace = true;

        isPlacing = false;

        _currentRobotState = RobotState.Stow;
        yield return safeRetract(stowLowerTarget, stowUpperTarget);
    }

    private IEnumerator safeGoToPos(float lower, float upper)
    {
        RobotState startState = _currentRobotState;

        lowerTarget = intermediateLower;
        upperTarget = intermediateUpper;

        while (lowerArmSpring.targetPosition != intermediateLower)
        {
            yield return null;
        }

        lowerTarget = lower;
        upperTarget = upper;
    }

    private IEnumerator safeRetract(float lower, float upper)
    {
        RobotState startState = _currentRobotState;

        lowerTarget = intermediateLower;
        upperTarget = Mathf.MoveTowards(upperTarget, upper, Mathf.Abs(upper - upperTarget) / 3);

        while (lowerArmSpring.targetPosition != intermediateLower)
        {
            yield return null;
        }

        if(_currentRobotState == startState)
        {
            lowerTarget = lower;
            upperTarget = upper;
        }
        
    }

    public IEnumerator goToTransfer()
    {
        isTransferring = true;

        lowerTarget = transferLowerTarget;
        upperTarget = transferUpperTarget;

        _currentRobotState = RobotState.Transfer;

        while (lowerArmSpring.targetPosition != transferLowerTarget || upperArmSpring.targetPosition != transferUpperTarget)
        {
            yield return null;
        }

        yield return null;
    }

    public void endTransfer()
    {
        isTransferring = false;

        if(_currentRobotState == RobotState.Transfer)
        {
            lowerTarget = stowLowerTarget;
            upperTarget = stowUpperTarget;

            _currentRobotState = RobotState.Stow;
        }
        
    }

    public bool isScoring()
    {
        return _currentRobotState == RobotState.High || _currentRobotState == RobotState.Middle;
    }

    public enum RobotState
    {
        Stow,
        High,
        Middle,
        Low,
        IntakeDoubleSubstation,
        IntakeGround,
        Transfer

    }
}

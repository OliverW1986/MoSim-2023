using Mechanisms;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class MAArm : MonoBehaviour
{
    public HingeJoint lowerArm;
    public HingeJoint upperArm;
    public HingeJoint endEffector;

    [SerializeField] public float lowerSpeed;
    [SerializeField] private float upperSpeed;
    [SerializeField] private float endEffectorSpeed;

    [SerializeField] private float stowLowerTarget;
    [SerializeField] private float stowUpperTarget;
    [SerializeField] private float stowEETarget;
    [SerializeField] private float shelfLowerTarget;
    [SerializeField] private float shelfUpperTarget;
    [SerializeField] private float shelfEETarget;
    [SerializeField] private float groundLowerTarget;
    [SerializeField] private float groundUpperTarget;
    [SerializeField] private float groundEETarget;
    [SerializeField] private float groundConeLowerTarget;
    [SerializeField] private float groundConeUpperTarget;
    [SerializeField] private float groundConeEETarget;
    [SerializeField] private float highConeLowerTarget;
    [SerializeField] private float highConeUpperTarget;
    [SerializeField] private float highConeEETarget;
    [SerializeField] private float highCubeLowerTarget;
    [SerializeField] private float highCubeUpperTarget;
    [SerializeField] private float highCubeEETarget;
    [SerializeField] private float midConeLowerTarget;
    [SerializeField] private float midConeUpperTarget;
    [SerializeField] private float midConeEETarget;
    [SerializeField] private float midCubeLowerTarget;
    [SerializeField] private float midCubeUpperTarget;
    [SerializeField] private float midCubeEETarget;
    [SerializeField] private float lowLowerTarget;
    [SerializeField] private float lowUpperTarget;
    [SerializeField] private float lowEETarget;

    private JointSpring lowerArmSpring;
    private JointSpring upperArmSpring;
    private JointSpring endEffectorSpring;

    private float lowerTarget = 0;
    private float upperTarget = 0;
    private float eeTarget = 0;

    public RobotState _previousRobotState;
    public RobotState _currentRobotState;

    private GamePieceManager _robotGamePieceManager;

    private InputAction _stowAction;
    private InputAction _highAction;
    private InputAction _middleAction;
    private InputAction _lowAction;
    private InputAction _intakeDoubleSubstationAction;
    private InputAction _intakeGroundAction;

    public bool isTransferring = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _robotGamePieceManager = GetComponent<GamePieceManager>();

        lowerArmSpring = lowerArm.spring;
        upperArmSpring = upperArm.spring;
        endEffectorSpring = endEffector.spring;

        _stowAction = InputSystem.actions.FindAction("Stow");
        _highAction = InputSystem.actions.FindAction("High");
        _middleAction = InputSystem.actions.FindAction("Middle");
        _lowAction = InputSystem.actions.FindAction("Low");
        _intakeDoubleSubstationAction = InputSystem.actions.FindAction("IntakeDoubleSubstation");
        _intakeGroundAction = InputSystem.actions.FindAction("IntakeGround");
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.canRobotMove && !isTransferring)
        {
            if (_stowAction.triggered || _intakeGroundAction.WasReleasedThisFrame() || _intakeDoubleSubstationAction.WasReleasedThisFrame())
            {
                lowerTarget = stowLowerTarget;
                upperTarget = stowUpperTarget;
                eeTarget = stowEETarget;

                _currentRobotState = RobotState.Stow;
            }
            else if (_highAction.triggered)
            {
                if (_robotGamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                {
                    lowerTarget = highCubeLowerTarget;
                    upperTarget = highCubeUpperTarget;
                    eeTarget = highCubeEETarget;
                }
                else
                {
                    lowerTarget = highConeLowerTarget;
                    upperTarget = highConeUpperTarget;
                    eeTarget = highConeEETarget;
                }

                _currentRobotState = RobotState.High;
            }
            else if (_middleAction.triggered)
            {
                if (_robotGamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                {
                    lowerTarget = midCubeLowerTarget;
                    upperTarget = midCubeUpperTarget;
                    eeTarget = midCubeEETarget;
                }
                else
                {
                    lowerTarget = midConeLowerTarget;
                    upperTarget = midConeUpperTarget;
                    eeTarget = midConeEETarget;

                }

                _currentRobotState = RobotState.Middle;
            }
            else if (_lowAction.triggered)
            {

                lowerTarget = lowLowerTarget;
                upperTarget = lowUpperTarget;
                eeTarget = lowEETarget;

                _currentRobotState = RobotState.Low;
            }
            else if (_intakeDoubleSubstationAction.triggered)
            {
                if (_robotGamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                {
                    lowerTarget = shelfLowerTarget;
                    upperTarget = shelfUpperTarget;
                    eeTarget = shelfEETarget;
                }
                else
                {
                    lowerTarget = shelfLowerTarget;
                    upperTarget = shelfUpperTarget;
                    eeTarget = shelfEETarget;
                }

                _currentRobotState = RobotState.IntakeDoubleSubstation;
            }
            else if (_intakeGroundAction.triggered && !_robotGamePieceManager.hasGamePiece)
            {

                if(_robotGamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                {
                    lowerTarget = groundLowerTarget;
                    upperTarget = groundUpperTarget;
                    eeTarget = groundEETarget;
                }
                else
                {
                    lowerTarget = groundConeLowerTarget;
                    upperTarget = groundConeUpperTarget;
                    eeTarget = groundConeEETarget;
                }

                _currentRobotState = RobotState.IntakeGround;
            }
        }

        if(_currentRobotState == RobotState.IntakeGround && _robotGamePieceManager.hasGamePiece)
        {
            lowerTarget = stowLowerTarget;
            upperTarget = stowUpperTarget;
            eeTarget = stowEETarget;

            _currentRobotState = RobotState.Stow;
        }

        lowerArmSpring.targetPosition = Mathf.MoveTowards(lowerArmSpring.targetPosition, lowerTarget, lowerSpeed * Time.deltaTime);
        upperArmSpring.targetPosition = Mathf.MoveTowards(upperArmSpring.targetPosition, upperTarget, upperSpeed * Time.deltaTime);
        endEffectorSpring.targetPosition = Mathf.MoveTowards(endEffectorSpring.targetPosition, eeTarget, endEffectorSpeed * Time.deltaTime);

        lowerArm.spring = lowerArmSpring;
        upperArm.spring = upperArmSpring;
        endEffector.spring = endEffectorSpring;

        _previousRobotState = _currentRobotState;

    }

    public void endTransfer()
    {
        isTransferring = false;
        lowerTarget = stowLowerTarget;
        upperTarget = stowUpperTarget;

        _currentRobotState = RobotState.Stow;
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
        IntakeGround
    }
}

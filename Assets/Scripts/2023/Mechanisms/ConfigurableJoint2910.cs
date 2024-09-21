using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanisms
{
    public class ConfigurableJoint2910 : MonoBehaviour
    {
        [SerializeField] private ConfigurableJoint armPivot;
        [SerializeField] private ConfigurableJoint stage1;
        [SerializeField] private ConfigurableJoint stage2;
        [SerializeField] private ConfigurableJoint wrist;

        [SerializeField] private float armHighAngle;
        [SerializeField] private float stage1HighDistance;
        [SerializeField] private float stage2HighDistance;
        [SerializeField] private float wristHighAngle;
        [SerializeField] private float armMiddleAngle;
        [SerializeField] private float stage1MiddleDistance;
        [SerializeField] private float stage2MiddleDistance;
        [SerializeField] private float wristMiddleAngle;
        [SerializeField] private float armLowAngle;
        [SerializeField] private float stage1LowDistance;
        [SerializeField] private float stage2LowDistance;
        [SerializeField] private float wristLowAngle;
        [SerializeField] private float armSubstationIntakeAngle;
        [SerializeField] private float stage1SubstationIntakeDistance;
        [SerializeField] private float stage2SubstationIntakeDistance;
        [SerializeField] private float wristSubstationIntakeAngle;
        [SerializeField] private float armGroundIntakeAngle;
        [SerializeField] private float stage1GroundIntakeDistance;
        [SerializeField] private float stage2GroundIntakeDistance;
        [SerializeField] private float wristGroundIntakeAngle;

        [SerializeField] private float armHighAngleCube;
        [SerializeField] private float stage1HighDistanceCube;
        [SerializeField] private float stage2HighDistanceCube;
        [SerializeField] private float wristHighAngleCube;
        [SerializeField] private float armMiddleAngleCube;
        [SerializeField] private float stage1MiddleDistanceCube;
        [SerializeField] private float stage2MiddleDistanceCube;
        [SerializeField] private float wristMiddleAngleCube;
        [SerializeField] private float armLowAngleCube;
        [SerializeField] private float stage1LowDistanceCube;
        [SerializeField] private float stage2LowDistanceCube;
        [SerializeField] private float wristLowAngleCube;
        [SerializeField] private float armSubstationIntakeAngleCube;
        [SerializeField] private float stage1SubstationIntakeDistanceCube;
        [SerializeField] private float stage2SubstationIntakeDistanceCube;
        [SerializeField] private float wristSubstationIntakeAngleCube;
        [SerializeField] private float armGroundIntakeAngleCube;
        [SerializeField] private float stage1GroundIntakeDistanceCube;
        [SerializeField] private float stage2GroundIntakeDistanceCube;
        [SerializeField] private float wristGroundIntakeAngleCube;

        [SerializeField] private float armTargetAngle;
        [SerializeField] private float stage1TargetDistance;
        [SerializeField] private float stage2TargetDistance;
        [SerializeField] private float wristTargetAngle;

        private RobotState _previousRobotState;
        private RobotState _currentRobotState;

        private GamePieceManager _gamePieceManager;

        private InputAction _stowAction;
        private InputAction _highAction;
        private InputAction _middleAction;
        private InputAction _lowAction;
        private InputAction _intakeDoubleSubstationAction;
        private InputAction _intakeGroundAction;

        private void Start()
        {
            _gamePieceManager = GetComponent<GamePieceManager>();

            _stowAction = InputSystem.actions.FindAction("Stow");
            _highAction = InputSystem.actions.FindAction("High");
            _middleAction = InputSystem.actions.FindAction("Middle");
            _lowAction = InputSystem.actions.FindAction("Low");
            _intakeDoubleSubstationAction = InputSystem.actions.FindAction("IntakeDoubleSubstation");
            _intakeGroundAction = InputSystem.actions.FindAction("IntakeGround");
        }

        private void Update()
        {
            if (GameManager.canRobotMove)
            {
                if (_stowAction.triggered || _intakeGroundAction.WasReleasedThisFrame() || _intakeDoubleSubstationAction.WasReleasedThisFrame())
                {
                    armTargetAngle = 0;
                    stage1TargetDistance = 0;
                    stage2TargetDistance = 0;
                    wristTargetAngle = 0;

                    _currentRobotState = RobotState.Stow;
                }
                else if (_highAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        armTargetAngle = armHighAngleCube;
                        stage1TargetDistance = stage1HighDistanceCube;
                        stage2TargetDistance = stage2HighDistanceCube;
                        wristTargetAngle = wristHighAngleCube;
                    }
                    else
                    {
                        armTargetAngle = armHighAngle;
                        stage1TargetDistance = stage1HighDistance;
                        stage2TargetDistance = stage2HighDistance;
                        wristTargetAngle = wristHighAngle;
                    }

                    _currentRobotState = RobotState.High;
                }
                else if (_middleAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        armTargetAngle = armMiddleAngleCube;
                        stage1TargetDistance = stage1MiddleDistanceCube;
                        stage2TargetDistance = stage2MiddleDistanceCube;
                        wristTargetAngle = wristMiddleAngleCube;
                    }
                    else
                    {
                        armTargetAngle = armMiddleAngle;
                        stage1TargetDistance = stage1MiddleDistance;
                        stage2TargetDistance = stage2MiddleDistance;
                        wristTargetAngle = wristMiddleAngle;
                    }

                    _currentRobotState = RobotState.Middle;
                }
                else if (_lowAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        armTargetAngle = armLowAngleCube;
                        stage1TargetDistance = stage1LowDistanceCube;
                        stage2TargetDistance = stage2LowDistanceCube;
                        wristTargetAngle = wristLowAngleCube;
                    }
                    else
                    {
                        armTargetAngle = armLowAngle;
                        stage1TargetDistance = stage1LowDistance;
                        stage2TargetDistance = stage2LowDistance;
                        wristTargetAngle = wristLowAngle;
                    }

                    _currentRobotState = RobotState.Low;
                }
                else if (_intakeDoubleSubstationAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        armTargetAngle = armSubstationIntakeAngleCube;
                        stage1TargetDistance = stage1SubstationIntakeDistanceCube;
                        stage2TargetDistance = stage2SubstationIntakeDistanceCube;
                        wristTargetAngle = wristSubstationIntakeAngleCube;
                    }
                    else
                    {
                        armTargetAngle = armSubstationIntakeAngle;
                        stage1TargetDistance = stage1SubstationIntakeDistance;
                        stage2TargetDistance = stage2SubstationIntakeDistance;
                        wristTargetAngle = wristSubstationIntakeAngle;
                    }

                    _currentRobotState = RobotState.IntakeDoubleSubstation;
                }
                else if ((_intakeGroundAction.triggered && !_previousRobotState.Equals(RobotState.Low)) || 
                            (_intakeGroundAction.triggered && !_previousRobotState.Equals(RobotState.IntakeDoubleSubstation)))
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        armTargetAngle = armGroundIntakeAngleCube;
                        stage1TargetDistance = stage1GroundIntakeDistanceCube;
                        stage2TargetDistance = stage2GroundIntakeDistanceCube;
                        wristTargetAngle = wristGroundIntakeAngleCube;
                    }
                    else
                    {
                        armTargetAngle = armGroundIntakeAngle;
                        stage1TargetDistance = stage1GroundIntakeDistance;
                        stage2TargetDistance = stage2GroundIntakeDistance;
                        wristTargetAngle = wristGroundIntakeAngle;
                    }

                    _currentRobotState = RobotState.IntakeGround;
                }
            }
            
            armPivot.targetRotation = Quaternion.Euler(armTargetAngle, 0, 0);
            stage1.targetPosition = new Vector3(0, -stage1TargetDistance, 0);
            stage2.targetPosition = new Vector3(0, -stage2TargetDistance, 0);
            wrist.targetRotation = Quaternion.Euler(-wristTargetAngle, 0, 0);
            
            _previousRobotState = _currentRobotState;
        }

        private IEnumerator MoveArm(float armAngle, float stage1Distance, float stage2Distance, float wristAngle)
        {
            armPivot.targetRotation = Quaternion.Euler(armAngle, 0, 0);
            yield return new WaitForSeconds(0.1f);
            stage1.targetPosition = new Vector3(0, -stage1Distance, 0);
            stage2.targetPosition = new Vector3(0, -stage2Distance, 0);
            wrist.targetRotation = Quaternion.Euler(-wristTargetAngle, 0, 0);
        }
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
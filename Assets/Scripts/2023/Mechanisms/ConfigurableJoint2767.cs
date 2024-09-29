using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanisms
{
    public class ConfigurableJoint2767 : MonoBehaviour
    {
        [SerializeField] private ConfigurableJoint armPivot;
        [SerializeField] private ConfigurableJoint elevator;
        [SerializeField] private ConfigurableJoint wrist;

        [SerializeField] private ConfigurableJoint claw1;
        [SerializeField] private ConfigurableJoint claw2;

        [SerializeField] private float closedConePosition;
        [SerializeField] private float closedCubePosition;
        [SerializeField] private float openClawPosition;

        [SerializeField] private float armHighAngle;
        [SerializeField] private float stage1HighDistance;
        [SerializeField] private float wristHighAngle;
        [SerializeField] private float armMiddleAngle;
        [SerializeField] private float stage1MiddleDistance;
        [SerializeField] private float wristMiddleAngle;
        [SerializeField] private float armLowAngle;
        [SerializeField] private float stage1LowDistance;
        [SerializeField] private float wristLowAngle;
        [SerializeField] private float armSubstationIntakeAngle;
        [SerializeField] private float stage1SubstationIntakeDistance;
        [SerializeField] private float wristSubstationIntakeAngle;
        [SerializeField] private float armGroundIntakeAngle;
        [SerializeField] private float stage1GroundIntakeDistance;
        [SerializeField] private float wristGroundIntakeAngle;

        [SerializeField] private float armHighAngleCube;
        [SerializeField] private float stage1HighDistanceCube;
        [SerializeField] private float wristHighAngleCube;
        [SerializeField] private float armMiddleAngleCube;
        [SerializeField] private float stage1MiddleDistanceCube;
        [SerializeField] private float wristMiddleAngleCube;
        [SerializeField] private float armLowAngleCube;
        [SerializeField] private float stage1LowDistanceCube;
        [SerializeField] private float wristLowAngleCube;
        [SerializeField] private float armSubstationIntakeAngleCube;
        [SerializeField] private float stage1SubstationIntakeDistanceCube;
        [SerializeField] private float wristSubstationIntakeAngleCube;
        [SerializeField] private float armGroundIntakeAngleCube;
        [SerializeField] private float stage1GroundIntakeDistanceCube;
        [SerializeField] private float wristGroundIntakeAngleCube;

        [SerializeField] private float armTargetAngle;
        [SerializeField] private float elevatorTargetDistance;
        [SerializeField] private float wristTargetAngle;
        [SerializeField] private float clawTarget;

        private RobotState _previousRobotState;
        private RobotState _currentRobotState;

        private GamePieceManager _gamePieceManager;

        private InputAction _stowAction;
        private InputAction _highAction;
        private InputAction _middleAction;
        private InputAction _lowAction;
        private InputAction _intakeDoubleSubstationAction;
        private InputAction _intakeGroundAction;

        private Vector3 starting;

        private void Start()
        {
            _gamePieceManager = GetComponent<GamePieceManager>();

            _stowAction = InputSystem.actions.FindAction("Stow");
            _highAction = InputSystem.actions.FindAction("High");
            _middleAction = InputSystem.actions.FindAction("Middle");
            _lowAction = InputSystem.actions.FindAction("Low");
            _intakeDoubleSubstationAction = InputSystem.actions.FindAction("IntakeDoubleSubstation");
            _intakeGroundAction = InputSystem.actions.FindAction("IntakeGround");

            starting = wrist.transform.localEulerAngles;

        }

        private void Update()
        {
            if (GameManager.canRobotMove)
            {
                if (_stowAction.triggered || _intakeGroundAction.WasReleasedThisFrame() || _intakeDoubleSubstationAction.WasReleasedThisFrame())
                {
                    StartCoroutine(Retract(0, 0, 0));

                    _currentRobotState = RobotState.Stow;
                }
                else if (_highAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        StartCoroutine(GoTo(armHighAngleCube, stage1HighDistanceCube, wristHighAngleCube));

                    }
                    else
                    {
                        StartCoroutine(GoTo(armHighAngle, stage1HighDistance, wristHighAngle));
                    }

                    _currentRobotState = RobotState.High;
                }
                else if (_middleAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        StartCoroutine(GoTo(armMiddleAngleCube, stage1MiddleDistanceCube, wristMiddleAngleCube));

                    }
                    else
                    {

                        StartCoroutine(GoTo(armMiddleAngle, stage1MiddleDistance, wristMiddleAngle));
                    }

                    _currentRobotState = RobotState.Middle;
                }
                else if (_lowAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {

                        StartCoroutine(GoTo(armLowAngleCube, stage1LowDistanceCube, wristLowAngleCube));

                    }
                    else
                    {
                        StartCoroutine(GoTo(armLowAngle, stage1LowDistance, wristLowAngle));

                    }

                    _currentRobotState = RobotState.Low;
                }
                else if (_intakeDoubleSubstationAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {

                        StartCoroutine(GoTo(armSubstationIntakeAngleCube, stage1SubstationIntakeDistanceCube, wristSubstationIntakeAngleCube));

                    }
                    else
                    {
                        StartCoroutine(GoTo(armSubstationIntakeAngle, stage1SubstationIntakeDistance, wristSubstationIntakeAngle));

                    }

                    _currentRobotState = RobotState.IntakeDoubleSubstation;
                }
                else if ((_intakeGroundAction.triggered && !_previousRobotState.Equals(RobotState.Low)) || 
                            (_intakeGroundAction.triggered && !_previousRobotState.Equals(RobotState.IntakeDoubleSubstation)))
                {

                    StartCoroutine(Retract(armGroundIntakeAngleCube, stage1GroundIntakeDistanceCube, wristGroundIntakeAngleCube));

                    _currentRobotState = RobotState.IntakeGround;
                }
            }
            
            armPivot.targetRotation = Quaternion.Euler(0, armTargetAngle, 0);
            elevator.targetPosition = new Vector3(0, 0, elevatorTargetDistance);
            wrist.targetRotation = Quaternion.Euler(0, -wristTargetAngle, 0);


            if(_gamePieceManager.hasGamePiece && !_gamePieceManager.isPlacing)
            {
                if(_gamePieceManager.currentGamePiece == GamePieceType.Cube)
                {
                    clawTarget = closedCubePosition;
                } else
                {
                    clawTarget = closedConePosition;
                }
            } else
            {
                    clawTarget = openClawPosition;
            }

            claw1.targetRotation = Quaternion.Euler(0, 0, clawTarget);
            claw2.targetRotation = Quaternion.Euler(0, 0, -clawTarget);
            
            _previousRobotState = _currentRobotState;
        }

        private IEnumerator GoTo(float armTarget, float elevatorTarget, float elbowTarget)
        {
            elevatorTargetDistance = 0;
            while(Mathf.Abs(elevator.transform.localPosition.z) > 0.1)
            {
                yield return null;
            }

            wristTargetAngle = elbowTarget;

            while(Mathf.Abs(getActualAngle(wrist.transform.localEulerAngles.y) - elbowTarget) > 15)
            {
                yield return null;
            }

            elevatorTargetDistance = elevatorTarget;
            armTargetAngle = armTarget;

            yield return null;
        }

        private IEnumerator Retract(float armTarget, float elevatorTarget, float elbowTarget)
        {
            elevatorTargetDistance = 0;
            while (Mathf.Abs(elevator.transform.localPosition.z) > 0.1)
            {
                yield return null;
            }

            armTargetAngle = armTarget;
            yield return new WaitForSeconds(0.25f);

            wristTargetAngle = elbowTarget;

            elevatorTargetDistance = elevatorTarget;

            yield return null;
        }

        private float getActualAngle(float eulerAngleReading)
        {
            //arm starts at like 320, goes to 360, then to up from zero
            if (eulerAngleReading >= 180)
            {
                return eulerAngleReading - starting.y;
            }
            else
            {
                return (360-starting.y) + (eulerAngleReading);
            }
        }
    }

   
}
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanisms
{
    public class ConfigurableJoint2337 : MonoBehaviour
    {
        [SerializeField] private ConfigurableJoint stage1;
        [SerializeField] private ConfigurableJoint stage2;
        [SerializeField] private ConfigurableJoint wrist;

        [SerializeField] private float stage1HighTarget;
        [SerializeField] private float stage2HighTarget;
        [SerializeField] private float wristHighAngle;
        [SerializeField] private float stage1MiddleTarget;
        [SerializeField] private float stage2MiddleTarget;
        [SerializeField] private float wristMiddleAngle;
        [SerializeField] private float stage1LowTarget;
        [SerializeField] private float stage2LowTarget;
        [SerializeField] private float wristLowAngle;
        [SerializeField] private float stage1SubstationIntakeTarget;
        [SerializeField] private float stage2SubstationIntakeTarget;
        [SerializeField] private float wristSubstationIntakeAngle;
        [SerializeField] private float stage1GroundIntakeTarget;
        [SerializeField] private float stage2GroundIntakeTarget;
        [SerializeField] private float wristGroundIntakeAngle;

        [SerializeField] private float stage1HighTargetCube;
        [SerializeField] private float stage2HighTargetCube;
        [SerializeField] private float wristHighAngleCube;
        [SerializeField] private float stage1MiddleTargetCube;
        [SerializeField] private float stage2MiddleTargetCube;
        [SerializeField] private float wristMiddleAngleCube;
        [SerializeField] private float stage1LowTargetCube;
        [SerializeField] private float stage2LowTargetCube;
        [SerializeField] private float wristLowAngleCube;
        [SerializeField] private float stage1SubstationIntakeTargetCube;
        [SerializeField] private float stage2SubstationIntakeTargetCube;
        [SerializeField] private float wristSubstationIntakeAngleCube;
        [SerializeField] private float stage1GroundIntakeTargetCube;
        [SerializeField] private float stage2GroundIntakeTargetCube;
        [SerializeField] private float wristGroundIntakeAngleCube;

        [SerializeField] private float stage1TargetAngle;
        [SerializeField] private float stage2TargetAngle;
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

                    StartCoroutine(retractFrom(0, 0, 0));

                    _currentRobotState = RobotState.Stow;
                }
                else if (_highAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        stage1TargetAngle = stage1HighTargetCube;
                        stage2TargetAngle = stage2HighTargetCube;
                        wristTargetAngle = wristHighAngleCube;
                    }
                    else
                    {
                        stage1TargetAngle = stage1HighTarget;
                        stage2TargetAngle = stage2HighTarget;
                        wristTargetAngle = wristHighAngle;
                    }

                    _currentRobotState = RobotState.High;
                }
                else if (_middleAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        stage1TargetAngle = stage1MiddleTargetCube;
                        stage2TargetAngle = stage2MiddleTargetCube;
                        wristTargetAngle = wristMiddleAngleCube;
                    }
                    else
                    {
                        stage1TargetAngle = stage1MiddleTarget;
                        stage2TargetAngle = stage2MiddleTarget;
                        wristTargetAngle = wristMiddleAngle;
                    }

                    _currentRobotState = RobotState.Middle;
                }
                else if (_lowAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        stage1TargetAngle = stage1LowTargetCube;
                        stage2TargetAngle = stage2LowTargetCube;
                        wristTargetAngle = wristLowAngleCube;
                    }
                    else
                    {
                        stage1TargetAngle = stage1LowTarget;
                        stage2TargetAngle = stage2LowTarget;
                        wristTargetAngle = wristLowAngle;
                    }

                    _currentRobotState = RobotState.Low;
                }
                else if (_intakeDoubleSubstationAction.WasPressedThisFrame())
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {

                        StartCoroutine(extendTo(stage1SubstationIntakeTargetCube, stage2SubstationIntakeTargetCube, wristSubstationIntakeAngleCube));
                    }
                    else
                    {
                        StartCoroutine(extendTo(stage1SubstationIntakeTarget, stage2SubstationIntakeTarget, wristSubstationIntakeAngle));

                    }

                    _currentRobotState = RobotState.IntakeDoubleSubstation;
                }
                else if ((_intakeGroundAction.triggered && !_previousRobotState.Equals(RobotState.Low)) ||
                            (_intakeGroundAction.triggered && !_previousRobotState.Equals(RobotState.IntakeDoubleSubstation)))
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        stage1TargetAngle = stage1GroundIntakeTargetCube;
                        stage2TargetAngle = stage2GroundIntakeTargetCube;
                        wristTargetAngle = wristGroundIntakeAngleCube;
                    }
                    else
                    {
                        stage1TargetAngle = stage1GroundIntakeTarget;
                        stage2TargetAngle = stage2GroundIntakeTarget;
                        wristTargetAngle = wristGroundIntakeAngle;
                    }

                    _currentRobotState = RobotState.IntakeGround;
                }
            }

            stage1.targetRotation = Quaternion.Euler(-stage1TargetAngle, 0, 0);
            stage2.targetRotation = Quaternion.Euler(stage2TargetAngle, 0, 0);
            wrist.targetRotation = Quaternion.Euler(-wristTargetAngle, 0, 0);

            _previousRobotState = _currentRobotState;
        }

        private IEnumerator extendTo(float stage1Target, float stage2Target, float wristAngle)
        {

            stage1TargetAngle = stage1Target;
            wristTargetAngle = wristAngle;

            if (stage2Target > 160)
            {
                stage2TargetAngle = 160;
                yield return new WaitForSeconds(0.25f);
                stage2TargetAngle = stage2Target;
            }
            else
            {
                stage2TargetAngle = stage2Target;
            }

        }

        private IEnumerator retractFrom(float stage1Target, float stage2Target, float wristAngle)
        {

            stage1TargetAngle = stage1Target;
            wristTargetAngle = wristAngle;

            if (stage2TargetAngle > 160)
            {
                stage2TargetAngle = 90;
                yield return new WaitForSeconds(0.25f);
                stage2TargetAngle = stage2Target;
            }
            else
            {
                stage2TargetAngle = stage2Target;
            }

        }

    }


}
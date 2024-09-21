using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanisms
{
    public class ConfigurableJoint5940 : MonoBehaviour
    {
        [SerializeField] private ConfigurableJoint stage1;
        [SerializeField] private ConfigurableJoint stage2;
        [SerializeField] private ConfigurableJoint wrist;
        [SerializeField] private ConfigurableJoint intake;

        [SerializeField] private float stage1HighDistance;
        [SerializeField] private float stage2HighDistance;
        [SerializeField] private float wristHighAngle;
        [SerializeField] private float stage1MiddleDistance;
        [SerializeField] private float stage2MiddleDistance;
        [SerializeField] private float wristMiddleAngle;
        [SerializeField] private float stage1LowDistance;
        [SerializeField] private float stage2LowDistance;
        [SerializeField] private float wristLowAngle;
        [SerializeField] private float stage1SubstationIntakeDistance;
        [SerializeField] private float stage2SubstationIntakeDistance;
        [SerializeField] private float wristSubstationIntakeAngle;

        [SerializeField] private float stage1HighDistanceCube;
        [SerializeField] private float stage2HighDistanceCube;
        [SerializeField] private float wristHighAngleCube;
        [SerializeField] private float stage1MiddleDistanceCube;
        [SerializeField] private float stage2MiddleDistanceCube;
        [SerializeField] private float wristMiddleAngleCube;
        [SerializeField] private float stage1LowDistanceCube;
        [SerializeField] private float stage2LowDistanceCube;
        [SerializeField] private float wristLowAngleCube;
        [SerializeField] private float stage1SubstationIntakeDistanceCube;
        [SerializeField] private float stage2SubstationIntakeDistanceCube;
        [SerializeField] private float wristSubstationIntakeAngleCube;
        [SerializeField] private float stage1GroundIntakeDistanceCube;
        [SerializeField] private float stage2GroundIntakeDistanceCube;
        [SerializeField] private float wristGroundIntakeAngleCube;
        [SerializeField] private float intakeGroundIntakeAngleCube;

        [SerializeField] private float stage1TargetDistance;
        [SerializeField] private float stage2TargetDistance;
        [SerializeField] private float wristTargetAngle;
        [SerializeField] private float intakeTargetAngle;

        private RobotState _previousRobotState;
        private RobotState _currentRobotState;

        private GamePieceManager _gamePieceManager;

        private bool _canMove = true;

        private InputAction _stowAction;
        private InputAction _highAction;
        private InputAction _middleAction;
        private InputAction _lowAction;
        private InputAction _intakeDoubleSubstationAction;
        private InputAction _intakeGroundAction;
        private InputAction _placeGamePieceAction;

        private void Start()
        {
            _gamePieceManager = GetComponent<GamePieceManager>();

            _stowAction = InputSystem.actions.FindAction("Stow");
            _highAction = InputSystem.actions.FindAction("High");
            _middleAction = InputSystem.actions.FindAction("Middle");
            _lowAction = InputSystem.actions.FindAction("Low");
            _intakeDoubleSubstationAction = InputSystem.actions.FindAction("IntakeDoubleSubstation");
            _intakeGroundAction = InputSystem.actions.FindAction("IntakeGround");
            _placeGamePieceAction = InputSystem.actions.FindAction("Place");

            _previousRobotState = RobotState.Stow;
            _currentRobotState = RobotState.Stow;
        }

        private void Update()
        {
            if (GameManager.canRobotMove)
            {
                if (_stowAction.triggered || _intakeGroundAction.WasReleasedThisFrame() ||
                    _intakeDoubleSubstationAction.WasReleasedThisFrame())
                {
                    stage1TargetDistance = 0;
                    stage2TargetDistance = 0;
                    wristTargetAngle = 0;
                    intakeTargetAngle = 0;

                    _currentRobotState = RobotState.Stow;

                    if (_canMove)
                    {
                        StartCoroutine(MoveArmAndIntake(stage1TargetDistance, stage2TargetDistance, wristTargetAngle,
                            intakeTargetAngle));
                    }
                }
                else if (_highAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        stage1TargetDistance = stage1HighDistanceCube;
                        stage2TargetDistance = stage2HighDistanceCube;
                        wristTargetAngle = wristHighAngleCube;
                        intakeTargetAngle = 0;
                    }
                    else
                    {
                        stage1TargetDistance = stage1HighDistance;
                        stage2TargetDistance = stage2HighDistance;
                        wristTargetAngle = wristHighAngle;
                        intakeTargetAngle = 0;
                    }

                    if (_canMove)
                    {
                        StartCoroutine(MoveArmAndIntake(stage1TargetDistance, stage2TargetDistance, wristTargetAngle,
                            intakeTargetAngle, _gamePieceManager.currentGamePieceMode == GamePieceType.Cube));
                    }

                    _currentRobotState = RobotState.High;
                }
                else if (_middleAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        stage1TargetDistance = stage1MiddleDistanceCube;
                        stage2TargetDistance = stage2MiddleDistanceCube;
                        wristTargetAngle = wristMiddleAngleCube;
                        intakeTargetAngle = 0;
                    }
                    else
                    {
                        stage1TargetDistance = stage1MiddleDistance;
                        stage2TargetDistance = stage2MiddleDistance;
                        wristTargetAngle = wristMiddleAngle;
                        intakeTargetAngle = 0;
                    }

                    if (_canMove)
                    {
                        StartCoroutine(MoveArmAndIntake(stage1TargetDistance, stage2TargetDistance, wristTargetAngle,
                            intakeTargetAngle, _gamePieceManager.currentGamePieceMode == GamePieceType.Cube));
                    }

                    _currentRobotState = RobotState.Middle;
                }
                else if (_lowAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        stage1TargetDistance = stage1LowDistanceCube;
                        stage2TargetDistance = stage2LowDistanceCube;
                        wristTargetAngle = wristLowAngleCube;
                        intakeTargetAngle = 0;
                    }
                    else
                    {
                        stage1TargetDistance = stage1LowDistance;
                        stage2TargetDistance = stage2LowDistance;
                        wristTargetAngle = wristLowAngle;
                        intakeTargetAngle = 0;
                    }

                    if (_canMove)
                    {
                        StartCoroutine(MoveArmAndIntake(stage1TargetDistance, stage2TargetDistance, wristTargetAngle,
                            intakeTargetAngle));
                    }

                    _currentRobotState = RobotState.Low;
                }
                else if (_intakeDoubleSubstationAction.triggered)
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        stage1TargetDistance = stage1SubstationIntakeDistanceCube;
                        stage2TargetDistance = stage2SubstationIntakeDistanceCube;
                        wristTargetAngle = wristSubstationIntakeAngleCube;
                        intakeTargetAngle = 0;
                    }
                    else
                    {
                        stage1TargetDistance = stage1SubstationIntakeDistance;
                        stage2TargetDistance = stage2SubstationIntakeDistance;
                        wristTargetAngle = wristSubstationIntakeAngle;
                        intakeTargetAngle = 0;
                    }

                    if (_canMove)
                    {
                        StartCoroutine(MoveArmAndIntake(stage1TargetDistance, stage2TargetDistance, wristTargetAngle,
                            intakeTargetAngle));
                    }

                    _currentRobotState = RobotState.IntakeDoubleSubstation;
                }
                else if ((_intakeGroundAction.triggered && !_previousRobotState.Equals(RobotState.Low)) ||
                         (_intakeGroundAction.triggered &&
                          !_previousRobotState.Equals(RobotState.IntakeDoubleSubstation)))
                {
                    if (!_gamePieceManager.hasGamePiece || (_gamePieceManager.hasGamePiece &&
                                                            _gamePieceManager.currentGamePieceMode ==
                                                            GamePieceType.Cube))
                    {
                        _gamePieceManager.currentGamePieceMode = GamePieceType.Cube;
                        stage1TargetDistance = stage1GroundIntakeDistanceCube;
                        stage2TargetDistance = stage2GroundIntakeDistanceCube;
                        wristTargetAngle = wristGroundIntakeAngleCube;
                        intakeTargetAngle = intakeGroundIntakeAngleCube;
                    }

                    if (_canMove)
                    {
                        StartCoroutine(MoveArmAndIntake(stage1TargetDistance, stage2TargetDistance, wristTargetAngle,
                            intakeTargetAngle));
                    }

                    _currentRobotState = RobotState.IntakeGround;
                }

                if (_placeGamePieceAction.triggered && _gamePieceManager.hasGamePiece && !_gamePieceManager.isPlacing &&
                    _gamePieceManager.canPlace)
                {
                    StartCoroutine(PlaceGamePiece());
                }
            }

            _previousRobotState = _currentRobotState;
        }

        private IEnumerator MoveArmAndIntake(float stage1Distance, float stage2Distance, float wristAngle,
            float intakeAngle, bool moveWrist = true)
        {
            _canMove = false;
            stage1.targetPosition = new Vector3(0, -stage1Distance, 0);
            stage2.targetPosition = new Vector3(0, -stage2Distance, 0);
            intake.targetRotation = Quaternion.Euler(intakeAngle, 0, 0);
            if (moveWrist)
            {
                wrist.targetRotation = Quaternion.Euler(-wristAngle, 0, 0);
            }

            _canMove = true;
            yield return null;
        }

        private IEnumerator PlaceGamePiece()
        {
            _gamePieceManager.isPlacing = true;
            if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
            {
                _gamePieceManager.StartCoroutine(_gamePieceManager.PlaceSequence(GamePieceType.Cube));
                yield return new WaitForSeconds(0.25f);
                _canMove = false;
                stage1.targetPosition = new Vector3(0, 0, 0);
                stage2.targetPosition = new Vector3(0, 0, 0);
                intake.targetRotation = Quaternion.Euler(0, 0, 0);
                wrist.targetRotation = Quaternion.Euler(0, 0, 0);
                yield return new WaitForSeconds(0.1f);
                _canMove = true;
                yield break;
            }

            _canMove = false;
            stage1.targetPosition = new Vector3(0, -stage1TargetDistance, 0);
            stage2.targetPosition = new Vector3(0, -stage2TargetDistance, 0);
            intake.targetRotation = Quaternion.Euler(intakeTargetAngle, 0, 0);
            wrist.targetRotation = Quaternion.Euler(-90, 0, 0);
            _canMove = true;
            yield return new WaitForSeconds(0.25f);
            _gamePieceManager.StartCoroutine(_gamePieceManager.PlaceSequence(GamePieceType.Cone));
            yield return new WaitForSeconds(0.1f);
            _canMove = false;
            stage1.targetPosition = new Vector3(0, 0, 0);
            stage2.targetPosition = new Vector3(0, 0, 0);
            intake.targetRotation = Quaternion.Euler(0, 0, 0);
            wrist.targetRotation = Quaternion.Euler(0, 0, 0);
            yield return new WaitForSeconds(0.1f);
            _canMove = true;
        }
    }
}
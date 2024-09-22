using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanisms
{
    public class ConfigurableJoint3005 : MonoBehaviour
    {
        [SerializeField] private ConfigurableJoint armPivot;
        [SerializeField] private ConfigurableJoint stage1;
        [SerializeField] private ConfigurableJoint wrist;

        private Transform pivotRB;
        private Rigidbody extendRB;
        private Rigidbody wristRB;

        private Vector3 starting;

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
        [SerializeField] private float stage1TargetDistance;
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

            pivotRB = armPivot.GetComponent<Transform>();
            extendRB = stage1.GetComponent<Rigidbody>();
            wristRB = wrist.GetComponent<Rigidbody>();

            starting = pivotRB.localRotation.eulerAngles;

        }

        private void Update()
        {
            if (GameManager.canRobotMove)
            {
                if (_stowAction.triggered || _intakeGroundAction.WasReleasedThisFrame() || _intakeDoubleSubstationAction.WasReleasedThisFrame())
                {
                    StopAllCoroutines();

                    StartCoroutine(retractFrom(0, 0, 0));

                    _currentRobotState = RobotState.Stow;
                }
                else if (_highAction.WasPressedThisFrame())
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        StopAllCoroutines();

                        StartCoroutine(extendTo(armHighAngleCube, stage1HighDistanceCube, wristHighAngleCube));

                    }
                    else
                    {
                        StopAllCoroutines();

                        StartCoroutine(extendTo(armHighAngle, stage1HighDistance, wristHighAngle));
                    }

                    _currentRobotState = RobotState.High;
                }
                else if (_middleAction.WasPressedThisFrame())
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        StopAllCoroutines();
                        StartCoroutine(extendTo(armMiddleAngleCube, stage1MiddleDistanceCube, wristMiddleAngleCube));

                    }
                    else
                    {
                        StopAllCoroutines();

                        StartCoroutine(extendTo(armMiddleAngle, stage1MiddleDistance, wristMiddleAngle));

                    }

                    _currentRobotState = RobotState.Middle;
                }
                else if (_lowAction.WasPressedThisFrame())
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        armTargetAngle = armLowAngleCube;
                        stage1TargetDistance = stage1LowDistanceCube;
                        wristTargetAngle = wristLowAngleCube;
                    }
                    else
                    {
                        armTargetAngle = armLowAngle;
                        stage1TargetDistance = stage1LowDistance;
                        wristTargetAngle = wristLowAngle;

                    }

                    _currentRobotState = RobotState.Low;
                }
                else if (_intakeDoubleSubstationAction.WasPressedThisFrame())
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {
                        StopAllCoroutines();

                        StartCoroutine(extendTo(armSubstationIntakeAngleCube, stage1SubstationIntakeDistanceCube, wristSubstationIntakeAngleCube));

                    }
                    else
                    {
                        StopAllCoroutines();

                        StartCoroutine(extendTo(armSubstationIntakeAngle, stage1SubstationIntakeDistance, wristSubstationIntakeAngle));

                    }

                    _currentRobotState = RobotState.IntakeDoubleSubstation;
                }
                else if ((_intakeGroundAction.WasPressedThisFrame() && !_previousRobotState.Equals(RobotState.Low)) || 
                            (_intakeGroundAction.WasPressedThisFrame() && !_previousRobotState.Equals(RobotState.IntakeDoubleSubstation)))
                {
                    if (_gamePieceManager.currentGamePieceMode == GamePieceType.Cube)
                    {

                        if (_currentRobotState == RobotState.Stow)
                        {
                            StopAllCoroutines();

                            StartCoroutine(extendTo(armGroundIntakeAngleCube, stage1GroundIntakeDistanceCube, wristGroundIntakeAngleCube));
                        }
                        else
                        {
                            StopAllCoroutines();

                            StartCoroutine(retractFrom(armGroundIntakeAngleCube, stage1GroundIntakeDistanceCube, wristGroundIntakeAngleCube));

                        }
                    }
                    else
                    {

                        if(_currentRobotState == RobotState.Stow)
                        {
                            StopAllCoroutines();

                            StartCoroutine(extendTo(armGroundIntakeAngle, stage1GroundIntakeDistance, wristGroundIntakeAngle));
                        }
                        else
                        {
                            StopAllCoroutines();

                            StartCoroutine(retractFrom(armGroundIntakeAngle, stage1GroundIntakeDistance, wristGroundIntakeAngle));

                        }
                    }

                    _currentRobotState = RobotState.IntakeGround;
                }
            }
            
            armPivot.targetRotation = Quaternion.Euler(0, 0, armTargetAngle);
            stage1.targetPosition = new Vector3(stage1TargetDistance, 0, 0);
            wrist.targetRotation = Quaternion.Euler(-wristTargetAngle, 0, 0);



            _previousRobotState = _currentRobotState;
        }

        private float getActualAngle(float eulerAngleReading)
        {
            //in testing, should be about 55, progressively decreases to zero and then up
            if(eulerAngleReading <= starting.z)
            {
                return starting.z - eulerAngleReading;
            } else
            {
                return starting.z + (360 - eulerAngleReading);
            }
        }

        private IEnumerator extendTo(float armAngle, float stage1Distance, float wristAngle)
        {

            if(armAngle > 160)
            {
                armTargetAngle = 160;
                yield return new WaitForSeconds(0.2f);
                armTargetAngle = armAngle;
            } else
            {
                armTargetAngle = armAngle;
            }

            while (Mathf.Abs(getActualAngle(pivotRB.localEulerAngles.z) - armTargetAngle) > 10f)
            {
                
                yield return null;
            }

            stage1TargetDistance = stage1Distance;
            wristTargetAngle = wristAngle;
        }

        private IEnumerator retractFrom(float armAngle, float stage1Distance, float wristAngle)
        {

            stage1TargetDistance = stage1Distance;
            wristTargetAngle = wristAngle;

            yield return new WaitForSeconds(0.2f);

            if (armAngle < 90 && pivotRB.localRotation.eulerAngles.z > 90)
            {
                armTargetAngle = 80;
                yield return new WaitForSeconds(0.2f);
                armTargetAngle = armAngle;
            }
            else
            {
                armTargetAngle = armAngle;
            }
        }

        private IEnumerator MoveArm(float armAngle, float stage1Distance, float stage2Distance, float wristAngle)
        {
            armPivot.targetRotation = Quaternion.Euler(0, 0, armAngle);
            yield return new WaitForSeconds(0.1f);
            stage1.targetPosition = new Vector3(stage1Distance, 0, 0);
            wrist.targetRotation = Quaternion.Euler(-wristTargetAngle, 0, 0);
        }
    }
}
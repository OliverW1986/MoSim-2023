using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mechanisms
{
    public class Climber5940 : MonoBehaviour
    {
        [SerializeField] private ConfigurableJoint elevator;
        [SerializeField] private ConfigurableJoint pivot;

        [SerializeField] private float elevatorDeployDistance;
        [SerializeField] private float pivotDeployRotation;

        [SerializeField] private float elevatorClimbDistance;
        [SerializeField] private float pivotClimbRotation;

        private InputAction _deployAction;
        private InputAction _climbAction;

        private void Start()
        {
            _deployAction = InputSystem.actions.FindAction("DeployClimber");
            _climbAction = InputSystem.actions.FindAction("Climb");
        }
        
        private void Update()
        {
            if (_deployAction.triggered)
            {
                StartCoroutine(Deploy());
            }

            if (_climbAction.triggered)
            {
                StartCoroutine(Climb());
            }
        }
        
        private IEnumerator Deploy()
        {
            elevator.targetPosition = new Vector3(0, -elevatorDeployDistance, 0);
            yield return new WaitForSeconds(0.25f);
            pivot.targetRotation = Quaternion.Euler(pivotDeployRotation, 0, 0);

            yield return new WaitForSeconds(0.5f);
        }
        
        private IEnumerator Climb()
        {
            elevator.targetPosition = new Vector3(0, -elevatorClimbDistance, 0);
            pivot.targetRotation = Quaternion.Euler(pivotClimbRotation, 0, 0);            
            yield return new WaitForSeconds(0.5f);
        }
    }
}
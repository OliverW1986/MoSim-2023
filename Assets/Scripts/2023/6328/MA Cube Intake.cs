using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MACubeIntake : MonoBehaviour
{
    public HingeJoint joint;

    public BoxCollider intakeZone;

    private StrikeZoneArm arm;
    private GamePieceManager pieceManager;
    private LedStripController lights;

    public float transferSpeed;

    public float speed;

    public float stowedTarget;
    public float loweredTarget;

    private float target = 0;

    private InputAction _intakeGroundAction;

    private JointSpring spring;

    private bool intaking = false;

    private bool transferring = false;
    private bool handlingCone = false;

    void Start()
    {
        _intakeGroundAction = InputSystem.actions.FindAction("IntakeGround");

        spring = joint.spring;
        
        arm = GetComponent<StrikeZoneArm>();
        pieceManager = GetComponent<GamePieceManager>();
        lights = GetComponent<LedStripController>();
    }

    void Update()
    {

        if (!pieceManager.hasGamePiece && GameManager.canRobotMove)
        {
            if (_intakeGroundAction.IsPressed() && pieceManager.currentGamePieceMode == GamePieceType.Cube)
            {
                target = loweredTarget;


                intaking = true;
            }
            else
            {
                target = stowedTarget;

                intaking = false;

            }
        }
        else
        {
            target = stowedTarget;
            intaking = false;
        }

        if(GameManager.canRobotMove)
        {
            spring.targetPosition = Mathf.MoveTowards(spring.targetPosition, target, speed * Time.deltaTime);
            joint.spring = spring;
        }
        

    }
}

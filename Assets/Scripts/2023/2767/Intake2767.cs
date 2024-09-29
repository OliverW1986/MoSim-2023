using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Intake2767 : MonoBehaviour
{
    public ConfigurableJoint lowerArm;
    public ConfigurableJoint upperArm;

    public float extendedTarget;

    public float currentTarget;

    public bool running = false;

    private InputAction _intakeGroundAction;
    private GamePieceManager pieceManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _intakeGroundAction = InputSystem.actions.FindAction("IntakeGround");
        pieceManager = GetComponent<GamePieceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_intakeGroundAction.WasPressedThisFrame() && GameManager.canRobotMove)
        {
            currentTarget = extendedTarget;
            running = true;

            pieceManager.doCubePath = true;

        } else if(_intakeGroundAction.WasReleasedThisFrame() && GameManager.canRobotMove)
        {
            running = false;
            pieceManager.doCubePath = false;
            StartCoroutine(DelayedRetract());
        }

        if (running)
        {
            pieceManager.currentGamePieceMode = GamePieceType.Cube;
        }

        upperArm.targetRotation = Quaternion.Euler(0, -currentTarget, 0);

    }

    private IEnumerator DelayedRetract()
    {
        yield return new WaitForSeconds(0.5f);

        currentTarget = 0;
    }
}

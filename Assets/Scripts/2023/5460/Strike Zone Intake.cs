using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class StrikeZoneIntake : MonoBehaviour
{
    public HingeJoint joint;

    public BoxCollider intakeZone;

    public GameObject limelight;

    public GameObject hiddenCone;
    public GameObject hiddenCube;

    public GameObject conePrefab;
    public GameObject cubePrefab;

    private StrikeZoneArm arm;
    private GamePieceManager pieceManager;
    private LedStripController lights;

    public float transferSpeed;

    public float speed;

    public float stowedTarget;
    public float limelightTarget;
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

        if(!transferring && ! pieceManager.hasGamePiece && GameManager.canRobotMove)
        {
            if (_intakeGroundAction.IsPressed())
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

        
        
        if(arm.isScoring() && pieceManager.hasGamePiece)
        {
            target = limelightTarget;
            limelight.SetActive(true);
            intaking = false;
        } else
        {
            limelight.SetActive(false);

        }


        if (intaking)
        {
            var collisions = Physics.OverlapBox(intakeZone.bounds.center, intakeZone.bounds.extents,
            Quaternion.identity);

            foreach (var otherCollider in collisions)
            {

                if (otherCollider.CompareTag("Cone"))
                {
                    handlingCone = true;
                    hiddenCone.SetActive(true);

                    pieceManager.currentGamePieceMode = GamePieceType.Cone; 

                    intaking = false;
                }
                else if(otherCollider.CompareTag("Cube"))
                {
                    handlingCone = false;
                    hiddenCube.SetActive(true);

                    pieceManager.currentGamePieceMode = GamePieceType.Cube;

                    intaking = false;
                }
                else
                {
                    continue;
                }


                GameObject _gamePiece = otherCollider.gameObject;
                Destroy(_gamePiece);

                transferring = true;


                StartCoroutine(TransferSequence());

                break;
            }
        }

        spring.targetPosition = Mathf.MoveTowards(spring.targetPosition, target, speed * Time.deltaTime);
        joint.spring = spring;

    }


    public IEnumerator TransferSequence()
    {
        GetComponent<DriveController>().forceIntake(true);

        target = stowedTarget;

        yield return new WaitForSeconds(0.1f);

        lights.Flash();

        while (spring.targetPosition != target)
        {
            yield return null;
        }

        yield return arm.goToTransfer();


        if(handlingCone)
        {
            yield return SpawnElement(conePrefab, hiddenCone);
        } else
        {
            yield return SpawnElement(cubePrefab, hiddenCube);

        }


        arm.endTransfer();

        transferring = false;

        GetComponent<DriveController>().forceIntake(false);

        yield return null;
    }

    IEnumerator SpawnElement(GameObject prefab, GameObject hidden)
    {
        var element = Instantiate(prefab, hidden.transform.position, hidden.transform.rotation);

        // cone.layer = 12;

        var elementChildren = element.GetComponentsInChildren<BoxCollider>();
        foreach (var child in elementChildren)
        {
            child.gameObject.layer = 12;
        }

        var rb = element.GetComponent<Rigidbody>();
        rb.linearVelocity = GetComponent<Rigidbody>().linearVelocity +
                            (hidden.transform.up.normalized * transferSpeed);
        hidden.SetActive(false);


        yield return new WaitForSeconds(0.2f);



        if(element != null)
        {
        element.layer = 0;

        }


        foreach (var child in elementChildren)
        {
            if(child != null)
            {
                child.gameObject.layer = 0;

            }
        }
    }
}

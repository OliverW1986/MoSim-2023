using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEditor;

public class MADrive : DriveController
{

    public float alignTurnSpeed;
    public float alignSpeed;
    public float alignAccel;
    public float alignDistance;

    public float maxAlignDistance;

    private MAArm arm;
    private GamePieceManager pieceManager;
    private Rigidbody rb;

    private float currentAlignSpeeed = 0;

    private bool isAligning;
    private bool waitingForAlign;
    private Vector2 moveDirection;

    private bool initialPositiveAlign = false;
    private bool stopAlignmentAxis = false;
    private float hardTarget = 0;

    public bool autoAlign = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();

        arm = GetComponent<MAArm>();
        rb = GetComponent<Rigidbody>();
        pieceManager = GetComponent<GamePieceManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();

        if (arm.isScoring() && pieceManager.hasGamePiece && autoAlign)
        {
            canRotate = false;
            rb.angularVelocity = Vector3.zero;
            if (isRedRobot)
            {
                rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, Quaternion.Euler(new Vector3(0, 90, 0)), alignTurnSpeed * Time.deltaTime));

                if (!isAligning && !waitingForAlign)
                {
                    StartCoroutine(Align(GameManager.redHigh, GameManager.redMid, GameManager.redLow, GameManager.redHigh[0].getNode1().center().x));
                }
            }
            else
            {
                rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, Quaternion.Euler(new Vector3(0, -90, 0)), alignTurnSpeed * Time.deltaTime));

                if (!isAligning && !waitingForAlign)
                {
                    StartCoroutine(Align(GameManager.blueHigh, GameManager.blueMid, GameManager.blueLow, GameManager.blueHigh[0].getNode1().center().x));
                }

            }
        }
        else
        {
            canRotate = true;
            canTranslate = true;
        }

        if (isAligning)
        {

            Vector3 direction = Vector3.zero;
            direction =
                transform.right * moveDirection.y
                +
                transform.forward * moveDirection.x
                ;

            direction = ConditionalNormalize(direction);

            if (stopAlignmentAxis)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, 0);
                rb.position = new Vector3(rb.position.x, rb.position.y, hardTarget);
            }

            rb.AddForce(direction * moveSpeed);
        }
    }

    private IEnumerator Align(Link[] high, Link[] mid, Link[] low, float x)
    {

        waitingForAlign = true;

        Link[] toUse;

        switch (arm._currentRobotState)
        {
            case MAArm.RobotState.High:
                toUse = high;
                break;
            case MAArm.RobotState.Middle:
                toUse = mid;
                break;
            case MAArm.RobotState.Low:
                toUse = low;
                break;
            default:
                toUse = new Link[0];
                break;
        }

        List<Node> availableNodes = new List<Node>();
        foreach (Link link in toUse)
        {
            if(!link.node1Scored() && pieceManager.currentGamePieceMode == GamePieceType.Cone) { availableNodes.Add(link.getNode1());  }
            if(!link.node2Scored() && pieceManager.currentGamePieceMode == GamePieceType.Cube) { availableNodes.Add(link.getNode2());  }
            if(!link.node3Scored() && pieceManager.currentGamePieceMode == GamePieceType.Cone) { availableNodes.Add(link.getNode3());  }
        }

        float closestDistance= float.MaxValue;
        Node closest = null;


        while (closestDistance > maxAlignDistance && arm.isScoring() && pieceManager.hasGamePiece)
        {

            foreach (Node node in availableNodes)
            {
                float thisDistance = Vector3.Distance(rb.position, node.center());

                if (thisDistance < closestDistance)
                {
                    closest = node;
                    closestDistance = thisDistance;
                }
            }

            yield return null;
        }

        if (closest != null)
        {
            canTranslate = false;
            isAligning = true;
            waitingForAlign = false;

            initialPositiveAlign = -(rb.position.z - (closest.center().z)) > 0;

            while (arm.isScoring() && pieceManager.hasGamePiece && GameManager.canRobotMove)
            {

                moveDirection.x = (rb.position.x - (x - alignDistance));
                moveDirection.y = - (rb.position.z - (closest.center().z));

                if (initialPositiveAlign != (moveDirection.y > 0))
                {

                    stopAlignmentAxis = true;

                    moveDirection.y = 0;
                    hardTarget  = closest.center().z;
                }

                moveDirection = moveDirection / 2;

                yield return null;
            }

            isAligning = false;
            stopAlignmentAxis = false;
            canTranslate = true;

        }


        yield return null;
    }

    public static Vector3 ConditionalNormalize(Vector3 vector)
    {
        // Check if any component is greater than 1
        if (vector.magnitude > 1)
        {
            // Normalize the vector
            return vector.normalized;
        }
        return vector; // Return the original vector if no component is above 1
    }
}

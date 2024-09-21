using System.Collections;
using UnityEngine;

public class HingeSlapDownIntake : MonoBehaviour, IResettable
{
    [SerializeField] private HingeJoint intake;

    [SerializeField] private float intakeAngle;

    private float _ampRotation;

    private bool _atTarget;
    public bool isAmping;

    private Vector3 _intakeStartingPos;
    private Quaternion _intakeStartingRot;

    private int _startingLayer;

    private DriveController _controller;

    private void Start() 
    {
        _intakeStartingPos = intake.gameObject.transform.localPosition;
        _intakeStartingRot = intake.gameObject.transform.localRotation;

        _startingLayer = intake.gameObject.layer;

        _controller = GetComponent<DriveController>();

        _ampRotation = intake.gameObject.transform.localEulerAngles.x + 20f;
    }

    private void Update()
    {
        if (!isAmping) 
        {
            if (_controller.isIntaking) 
            {
                RotateIntake(intakeAngle);
            }
            else
            {
                StowIntake();
            }
        }
    }

    private void RotateIntake(float targetAngle)
    {
        if (!_atTarget)
        {
            var intakeSpring = intake.spring;
            intakeSpring.targetPosition = intakeAngle;
            intake.spring = intakeSpring;
        }
    }

    public void StowIntake()
    {
        _atTarget = false;
        var intakeSpring = intake.spring;
        intakeSpring.targetPosition = 0;
        intake.spring = intakeSpring;
    }

    public void MoveForAmp()
    {
        RotateToAmpRotation();
    }

    private void RotateToAmpRotation()
    {
        isAmping = true;
        var intakeSpring = intake.spring;
        intakeSpring.targetPosition = _ampRotation;
        intake.spring = intakeSpring;
    }

    private IEnumerator WaitToEnable() 
    {
        yield return new WaitForSeconds(0.01f);
        intake.gameObject.layer = _startingLayer;
    }

    public void Reset() 
    {   
        StopAllCoroutines();

        intake.gameObject.layer = 17;

        intake.gameObject.transform.localPosition = _intakeStartingPos;
        intake.gameObject.transform.localRotation = _intakeStartingRot;

        var intakeSpring = intake.spring;
        intakeSpring.targetPosition = 0;
        intake.spring = intakeSpring;

        _atTarget = false;
        isAmping = false;
        
        StartCoroutine(WaitToEnable());
    }
}

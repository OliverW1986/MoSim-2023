using System.Collections;
using UnityEngine;

public class SlapDownIntake : MonoBehaviour, IResettable
{
    [SerializeField] private Transform intakePivot;

    private Quaternion _stowRotation;
    private Quaternion _ampRotation;

    [SerializeField] private float stowSpeed = 2f;
    [SerializeField] private float rotationSpeed = 45f;

    private bool _atTarget;
    private bool _isRotating;
    private bool _isStowing;
    public bool isAmping { get; set; }

    private DriveController _controller;

    private void Start() 
    {
        _controller = GetComponent<DriveController>();

        _stowRotation = intakePivot.localRotation;
        _ampRotation = Quaternion.Euler(intakePivot.localEulerAngles.x + 20f, intakePivot.localEulerAngles.y, intakePivot.localEulerAngles.z);
    }

    private void Update()
    {
        if (_controller.isIntaking && !_isStowing && !isAmping) 
        {
            RotateIntake(110f);
        }
        else if (!_isRotating && !isAmping)
        {
            StowIntake();
        }
        else if (!isAmping)
        {
            RotateIntake(110f);
        }
    }

    private void RotateIntake(float targetAngle)
    {
        if (!_atTarget)
        {
            _isRotating = true;
            var targetRotation = Quaternion.Euler(targetAngle, 0, 0);
            intakePivot.localRotation = Quaternion.RotateTowards(intakePivot.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (Quaternion.Angle(intakePivot.localRotation, targetRotation) < 0.1f)
            {
                _isRotating = false;
                _atTarget = true;
            }
        }
    }

    public void StowIntake()
    {
        _isStowing = true;
        _atTarget = false;
        intakePivot.localRotation = Quaternion.RotateTowards(intakePivot.localRotation, _stowRotation, stowSpeed * Time.deltaTime);
        if (intakePivot.localRotation == _stowRotation)
        {
            _isStowing = false;
        }
    }

    public void MoveForAmp()
    {
        isAmping = true;
        if (_isRotating) { _isRotating = false; }
        StartCoroutine(RotateToAmpRotation());
    }

    private IEnumerator RotateToAmpRotation()
    {
        var t = 0f;
        var startRotation = intakePivot.localRotation;
        
        while (t < 1f)
        {
            t += Time.deltaTime * 8f;
            intakePivot.localRotation = Quaternion.Slerp(startRotation, _ampRotation, t);
            yield return null;
        }
        
        intakePivot.localRotation = _ampRotation;
    }

    public void Reset() 
    {   
        StopAllCoroutines();
        intakePivot.localRotation = _stowRotation;
        _atTarget = false;
        _isRotating = false;
        _isStowing = false;
        isAmping = false;
    }
}

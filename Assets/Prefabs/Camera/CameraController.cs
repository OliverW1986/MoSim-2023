using System;
using Prefabs.Camera;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed;

    private Vector2 _translateValue;
    private Vector3 _startingDirection;
    private Vector3 _startingRotation;

    private InputAction _moveAction;

    private void Start()
    {
        _startingDirection = transform.forward;
        _startingRotation = transform.right;

        _moveAction = InputSystem.actions.FindAction("PanCamMove");
    }

    private void FixedUpdate()
    {
        _translateValue = _moveAction.ReadValue<Vector2>();

        CameraPan.parentMoving = Math.Abs(_translateValue.magnitude) > 0f;
        
        var moveDirection = _startingDirection * _translateValue.y + _startingRotation * _translateValue.x;

        rb.AddForce(moveDirection * moveSpeed);
    }
}

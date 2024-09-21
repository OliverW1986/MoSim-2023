using UnityEngine;
using UnityEngine.Audio;

public class BumperSounds : MonoBehaviour
{
    [SerializeField] private Alliance alliance;

    [SerializeField] private AudioSource player;
    [SerializeField] private AudioResource[] hitSounds;

    private DriveController _controller;

    private bool _triggerSound;
    private bool _useSounds;

    private void OnEnable()
    {
        _useSounds = PlayerPrefs.GetInt("BumpSounds") == 1;
        _controller = GetComponent<DriveController>();
    }

    private void Update() 
    {
        if (_useSounds) 
        {
            var touchingWall = (alliance == Alliance.Red && DriveController.isTouchingWallColliderRed) || (alliance == Alliance.Blue && DriveController.isTouchingWallColliderBlue);
        
            if (touchingWall && !player.isPlaying && _triggerSound)
            {
                player.volume = _controller.beforeVelocity * 0.02f;
                player.resource = hitSounds[Random.Range(0, hitSounds.Length)];
                player.Play();
            }

            if (!touchingWall)
            {
                _triggerSound = true;
            }
            else 
            {
                _triggerSound = false;
            }
        }
    }
} 

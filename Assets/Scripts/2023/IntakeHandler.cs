using UnityEngine;

public class IntakeHandler : MonoBehaviour
{
    [SerializeField] private RobotSettings robot;
    [SerializeField] private GamePieceManager collisionsScript;
    [SerializeField] private Collider intakeConeCollider;
    [SerializeField] private Collider intakeCubeCollider;

    private GameObject _gamePiece;

    private void Start()
    {
        collisionsScript.coneWithinIntakeConeCollider = false;
        collisionsScript.cubeWithinIntakeCubeCollider = false;
        collisionsScript.touchedGamePiece = null;
    }

    private void Update()
    {
        var coneDetected = false;
        var cubeDetected = false;

        var coneColliders = Physics.OverlapBox(intakeConeCollider.bounds.center, intakeConeCollider.bounds.extents,
            Quaternion.identity);
        var cubeColliders = Physics.OverlapBox(intakeCubeCollider.bounds.center, intakeCubeCollider.bounds.extents,
            Quaternion.identity);

        foreach (var otherCollider in coneColliders)
        {
            if (robot == RobotSettings.Bread)
            {
                if (otherCollider.CompareTag("Cone"))
                {
                    _gamePiece = otherCollider.gameObject;
                    collisionsScript.coneWithinIntakeConeCollider = true;
                    collisionsScript.touchedGamePiece = otherCollider.gameObject;
                    coneDetected = true;
                    break;
                }
                
                if (otherCollider.CompareTag("Cube"))
                {
                    _gamePiece = otherCollider.gameObject;
                    collisionsScript.cubeWithinIntakeCubeCollider = true;
                    collisionsScript.touchedGamePiece = otherCollider.gameObject;
                    cubeDetected = true;
                    break;
                }
            }
            else
            {
                if (otherCollider.CompareTag("Cone") &&
                    collisionsScript.currentGamePieceMode == GamePieceType.Cone)
                {
                    _gamePiece = otherCollider.gameObject;
                    collisionsScript.coneWithinIntakeConeCollider = true;
                    collisionsScript.touchedGamePiece = otherCollider.gameObject;
                    coneDetected = true;
                    break;
                }
            }
        }

        foreach (var otherCollider in cubeColliders)
        {
            if (!otherCollider.CompareTag("Cube") ||
                collisionsScript.currentGamePieceMode != GamePieceType.Cube) continue;
            _gamePiece = otherCollider.gameObject;
            collisionsScript.cubeWithinIntakeCubeCollider = true;
            collisionsScript.touchedGamePiece = otherCollider.gameObject;
            cubeDetected = true;
            break;
        }

        if (!coneDetected)
        {
            collisionsScript.coneWithinIntakeConeCollider = false;
        }

        if (!cubeDetected)
        {
            collisionsScript.cubeWithinIntakeCubeCollider = false;
        }

        if (coneDetected || cubeDetected) return;
        _gamePiece = null;
        collisionsScript.touchedGamePiece = null;
    }
}
using UnityEngine;

public class IntakeCollider2767 : MonoBehaviour
{
    public Intake2767 intake;
    public GamePieceManager pieceManager;

    void OnTriggerEnter(Collider other)
    {
        //TODO: Spline animation or something
        if(intake.running && GameManager.canRobotMove && !pieceManager.hasGamePiece)
        {
            if (other.CompareTag("Cube"))
            {
                pieceManager.cubeWithinIntakeCubeCollider = true;
                pieceManager.touchedGamePiece = other.gameObject;

            }

        }
    }
}

using UnityEngine;

public class G414Penalty : MonoBehaviour
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private AudioSource alliancePlayer;

    private bool _gavePenalty;

    private const int G414PenaltyWorth = 2;

    private void PenalizeScore() 
    {
        _gavePenalty = true;

        alliancePlayer.Play();

        var matchEnded = GameManager.GameState == GameState.End;
        if (matchEnded) return;
        if (GameManager.GameState == GameState.Auto)
        {
            if (alliance == Alliance.Red) 
            {
                GameScoreTracker.BlueAutoPenaltyPoints += G414PenaltyWorth;
            }
            else 
            {
                GameScoreTracker.RedAutoPenaltyPoints += G414PenaltyWorth;
            }
        }
        else
        {
            if (alliance == Alliance.Red)
            {
                GameScoreTracker.BlueTeleopPenaltyPoints += G414PenaltyWorth;
            }
            else 
            {
                GameScoreTracker.RedTeleopPenaltyPoints += G414PenaltyWorth;
            }
                
        }
            
        if (alliance == Alliance.Blue) { Score.redScore += G414PenaltyWorth; }
        else { Score.blueScore += G414PenaltyWorth; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_gavePenalty) 
        {
            if (alliance == Alliance.Blue) 
            {
                // if (other.gameObject.CompareTag("noteShotByBlue") && ZoneControl.blueRobotInRedZone || other.gameObject.CompareTag("noteShotByBlue2") && ZoneControl.blueOtherRobotInRedZone)
                // {
                //     other.tag = "Ring";
                //     PenalizeScore();
                // }
            }
            // if (other.gameObject.CompareTag("noteShotByRed") && ZoneControl.redRobotInBlueZone || other.gameObject.CompareTag("noteShotByRed2") && ZoneControl.redOtherRobotInBlueZone) 
            // {
            //     other.tag = "Ring";
            //     PenalizeScore();
            // }
        }
        _gavePenalty = false;
    }
}

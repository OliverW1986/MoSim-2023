using UnityEngine;

public class Tape : MonoBehaviour, IResettable
{
    private bool _triggeredMobilityScore;
    private bool _triggeredMobilityScoreForSecondaryPlayer;
    private bool _isThereASecondaryPlayer;
    public bool isRedTape;
    
    private void Start()
    {
        Reset();

        if (RobotSpawnController.sameAlliance) 
        {
            _isThereASecondaryPlayer = true;
        }
    }

    public void Reset() 
    {
        _triggeredMobilityScore = false;
        _triggeredMobilityScoreForSecondaryPlayer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRedTape) 
        {
            if (other.gameObject.CompareTag("Player") && GameManager.GameState == GameState.Auto && !_triggeredMobilityScore)
            {
                _triggeredMobilityScore = true;
                Score.AddScore(3, Alliance.Blue);
                GameScoreTracker.BlueAutoLeavePoints += 3;
            }
            else if (_isThereASecondaryPlayer && other.gameObject.CompareTag("Player2") && GameManager.GameState == GameState.Auto && !_triggeredMobilityScoreForSecondaryPlayer) 
            {
                _triggeredMobilityScoreForSecondaryPlayer = true;
                Score.AddScore(3, Alliance.Blue);

                GameScoreTracker.BlueAutoLeavePoints += 3;
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player") && GameManager.GameState == GameState.Auto && !_triggeredMobilityScore)
            {
                _triggeredMobilityScore = true;
                Score.AddScore(3, Alliance.Red);

                GameScoreTracker.RedAutoLeavePoints += 3;
            }
            else if (_isThereASecondaryPlayer && other.gameObject.CompareTag("Player2") && GameManager.GameState == GameState.Auto && !_triggeredMobilityScoreForSecondaryPlayer) 
            {
                _triggeredMobilityScoreForSecondaryPlayer = true;
                Score.AddScore(3, Alliance.Red);

                GameScoreTracker.RedAutoLeavePoints += 3;
            }
        }
    }
}

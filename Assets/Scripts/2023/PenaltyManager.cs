using System.Collections;
using UnityEngine;

public class PenaltyManager : MonoBehaviour
{
    [SerializeField] private Collider[] colliders;
    [SerializeField] private PenaltyCollisions[] sourceCollisions;

    [SerializeField] private Collider autoMiddleLineCollider;
    [SerializeField] private AudioSource errorSound;
    [SerializeField] private bool redAlliance;
    [SerializeField] private float penaltyCooldown;
    [SerializeField] private Alliance alliance;

    [SerializeField] private bool playerInsidePenaltyZone;
    [SerializeField] private bool playerInsideStage;
    [SerializeField] private bool playerPastAutoLine;

    private DriveController _playerThatGotPenalty;
    private DriveController _opponentThatGotPenalty;


    private bool _scoreUpdated;
    private bool _isTimerCounting;

    private const int PenaltyWorth = 5;

    private static GameObject _blueRobot;
    private static GameObject _otherBlueRobot;
    private static GameObject _redRobot;
    private static GameObject _otherRedRobot;

    private void Start()
    {
        _blueRobot = GameObject.FindGameObjectWithTag("Player");
        _otherBlueRobot = GameObject.FindGameObjectWithTag("Player2");
        _redRobot = GameObject.FindGameObjectWithTag("RedPlayer");
        _otherRedRobot = GameObject.FindGameObjectWithTag("RedPlayer2");
    }

    public static void ResetPenaltyManager()
    {
        _blueRobot = GameObject.FindGameObjectWithTag("Player");
        _otherBlueRobot = GameObject.FindGameObjectWithTag("Player2");
        _redRobot = GameObject.FindGameObjectWithTag("RedPlayer");
        _otherRedRobot = GameObject.FindGameObjectWithTag("RedPlayer2");
    }

    private void Update()
    {
        if (!_scoreUpdated && !_isTimerCounting)
        {
            UpdateScore();
        }
        else if (!DriveController.robotsTouching && !_isTimerCounting)
        {
            _scoreUpdated = false;
            _isTimerCounting = false;
        }

        CheckForCollisions();
    }

    private void CheckForCollisions()
    {
        playerInsidePenaltyZone = false;

        foreach (var col in colliders)
        {
            if (redAlliance)
            {
                if (_redRobot is null || _otherRedRobot is null)
                {
                    continue;
                }

                if (col.bounds.Intersects(_redRobot.GetComponent<Collider>().bounds) &&
                    col.bounds.Intersects(_blueRobot.GetComponent<Collider>().bounds))
                {
                    _playerThatGotPenalty = _blueRobot.GetComponent<DriveController>();
                    _opponentThatGotPenalty = _redRobot.GetComponent<DriveController>();
                    playerInsidePenaltyZone = true;
                    break;
                }

                if (col.bounds.Intersects(_redRobot.GetComponent<Collider>().bounds) &&
                    col.bounds.Intersects(_otherBlueRobot.GetComponent<Collider>().bounds))
                {
                    _playerThatGotPenalty = _otherBlueRobot.GetComponent<DriveController>();
                    _opponentThatGotPenalty = _redRobot.GetComponent<DriveController>();
                    playerInsidePenaltyZone = true;
                    break;
                }

                if (col.bounds.Intersects(_otherRedRobot.GetComponent<Collider>().bounds) &&
                    col.bounds.Intersects(_blueRobot.GetComponent<Collider>().bounds))
                {
                    _playerThatGotPenalty = _blueRobot.GetComponent<DriveController>();
                    _opponentThatGotPenalty = _otherRedRobot.GetComponent<DriveController>();
                    playerInsidePenaltyZone = true;
                    break;
                }

                if (col.bounds.Intersects(_otherRedRobot.GetComponent<Collider>().bounds) &&
                    col.bounds.Intersects(_otherBlueRobot.GetComponent<Collider>().bounds))
                {
                    _playerThatGotPenalty = _otherBlueRobot.GetComponent<DriveController>();
                    _opponentThatGotPenalty = _otherRedRobot.GetComponent<DriveController>();
                    playerInsidePenaltyZone = true;
                    break;
                }
            }
            else
            {
                if (_blueRobot is null || _otherBlueRobot is null)
                {
                    continue;
                }

                if (col.bounds.Intersects(_blueRobot.GetComponent<Collider>().bounds) &&
                    col.bounds.Intersects(_redRobot.GetComponent<Collider>().bounds))
                {
                    _playerThatGotPenalty = _redRobot.GetComponent<DriveController>();
                    _opponentThatGotPenalty = _blueRobot.GetComponent<DriveController>();
                    playerInsidePenaltyZone = true;
                    break;
                }

                if (col.bounds.Intersects(_blueRobot.GetComponent<Collider>().bounds) &&
                    col.bounds.Intersects(_otherRedRobot.GetComponent<Collider>().bounds))
                {
                    _playerThatGotPenalty = _otherRedRobot.GetComponent<DriveController>();
                    _opponentThatGotPenalty = _blueRobot.GetComponent<DriveController>();
                    playerInsidePenaltyZone = true;
                    break;
                }

                if (col.bounds.Intersects(_otherBlueRobot.GetComponent<Collider>().bounds) &&
                    col.bounds.Intersects(_redRobot.GetComponent<Collider>().bounds))
                {
                    _playerThatGotPenalty = _redRobot.GetComponent<DriveController>();
                    _opponentThatGotPenalty = _otherBlueRobot.GetComponent<DriveController>();
                    playerInsidePenaltyZone = true;
                    break;
                }

                if (col.bounds.Intersects(_otherBlueRobot.GetComponent<Collider>().bounds) &&
                    col.bounds.Intersects(_otherRedRobot.GetComponent<Collider>().bounds))
                {
                    _playerThatGotPenalty = _otherRedRobot.GetComponent<DriveController>();
                    _opponentThatGotPenalty = _otherBlueRobot.GetComponent<DriveController>();
                    playerInsidePenaltyZone = true;
                    break;
                }
            }
        }

        foreach (var penaltyCollisions in sourceCollisions)
        {
            if (!penaltyCollisions.inside) continue;
            _playerThatGotPenalty = penaltyCollisions.playerInside;
            _opponentThatGotPenalty = penaltyCollisions.enemyInside;
            playerInsidePenaltyZone = true;
            break;
        }

        if (redAlliance)
        {
            if (_redRobot is null || _otherRedRobot is null)
            {
                return;
            }
            playerPastAutoLine = autoMiddleLineCollider.bounds.Intersects(_redRobot.GetComponent<Collider>().bounds) ||
                                 autoMiddleLineCollider.bounds.Intersects(
                                     _otherRedRobot.GetComponent<Collider>().bounds);
            if (!playerPastAutoLine) return;
            _opponentThatGotPenalty = _redRobot.GetComponent<DriveController>();
        }
        else
        {
            if (_blueRobot is null || _otherBlueRobot is null)
            {
                return;
            }
            playerPastAutoLine = autoMiddleLineCollider.bounds.Intersects(_blueRobot.GetComponent<Collider>().bounds) ||
                                 autoMiddleLineCollider.bounds.Intersects(_otherBlueRobot.GetComponent<Collider>()
                                     .bounds);
            if (!playerPastAutoLine) return;
            _opponentThatGotPenalty = _blueRobot.GetComponent<DriveController>();
        }
    }

    private void UpdateScore()
    {
        if (!DriveController.robotsTouching) return;
        if (playerInsidePenaltyZone)
        {
            if (GameManager.GameState != GameState.End)
            {
                if (GameManager.GameState == GameState.Auto)
                {
                    errorSound.Play();
                    AddScore(true, false);
                }
                else
                {
                    errorSound.Play();
                    AddScore(false, false);
                }
            }
        }

        if (playerInsideStage && GameManager.GameState == GameState.Endgame)
        {
            errorSound.Play();
            AddScore(false, false);
        }

        if (!playerPastAutoLine || GameManager.GameState != GameState.Auto) return;
        errorSound.Play();
        AddScore(true, true);
    }

    private void AddScore(bool isAutoPoints, bool addScoreToOpponent)
    {
        StartCoroutine(addScoreToOpponent
            ? NoPenaltiesWhenThisIsRunning(_opponentThatGotPenalty, penaltyCooldown)
            : NoPenaltiesWhenThisIsRunning(_playerThatGotPenalty, penaltyCooldown));

        if (redAlliance)
        {
            if (isAutoPoints)
            {
                if (addScoreToOpponent)
                {
                    GameScoreTracker.BlueAutoPenaltyPoints += PenaltyWorth;
                }
                else
                {
                    GameScoreTracker.RedAutoPenaltyPoints += PenaltyWorth;
                }
            }
            else
            {
                if (addScoreToOpponent)
                {
                    GameScoreTracker.BlueTeleopPenaltyPoints += PenaltyWorth;
                }
                else
                {
                    GameScoreTracker.RedTeleopPenaltyPoints += PenaltyWorth;
                }
            }

            if (addScoreToOpponent)
            {
                Score.blueScore += PenaltyWorth;
            }
            else
            {
                Score.redScore += PenaltyWorth;
            }
        }
        else
        {
            if (isAutoPoints)
            {
                if (addScoreToOpponent)
                {
                    GameScoreTracker.RedAutoPenaltyPoints += PenaltyWorth;
                }
                else
                {
                    GameScoreTracker.BlueAutoPenaltyPoints += PenaltyWorth;
                }
            }
            else
            {
                if (addScoreToOpponent)
                {
                    GameScoreTracker.RedTeleopPenaltyPoints += PenaltyWorth;
                }
                else
                {
                    GameScoreTracker.BlueTeleopPenaltyPoints += PenaltyWorth;
                }
            }

            if (addScoreToOpponent)
            {
                Score.redScore += PenaltyWorth;
            }
            else
            {
                Score.blueScore += PenaltyWorth;
            }
        }

        _scoreUpdated = true;
    }

    private IEnumerator NoPenaltiesWhenThisIsRunning(DriveController controller, float duration)
    {
        controller.StartCoroutine(controller.GrayOutBumpers(duration));
        _isTimerCounting = true;
        yield return new WaitForSeconds(duration);
        _isTimerCounting = false;
    }
}
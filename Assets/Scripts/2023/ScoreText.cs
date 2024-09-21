using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    [SerializeField] private RobotSelector redRobotSelector;
    [SerializeField] private RobotSelector blueRobotSelector;

    public TextMeshProUGUI redAutoSpeakerScore;
    public TextMeshProUGUI redAutoAmpScore;
    public TextMeshProUGUI redAutoLeaveScore;
    public TextMeshProUGUI redAutoPenalty;

    public TextMeshProUGUI redTeleopSpeakerScore;
    public TextMeshProUGUI redTeleopAmpScore;
    public TextMeshProUGUI redStageScore;
    public TextMeshProUGUI redTeleopPenalty;
    public TextMeshProUGUI redTrapScore;

    public TextMeshProUGUI totalRedScore;

    public TextMeshProUGUI blueAutoConeScore;
    public TextMeshProUGUI blueAutoCubeScore;
    public TextMeshProUGUI blueAutoLeaveScore;
    public TextMeshProUGUI blueAutoPenalty;

    public TextMeshProUGUI blueTeleopConeScore;
    public TextMeshProUGUI blueTeleopCubeScore;
    public TextMeshProUGUI blueChargeStationScore;
    public TextMeshProUGUI blueTeleopPenalty;

    public TextMeshProUGUI totalBlueScore;

    public TextMeshProUGUI redRobot;
    public TextMeshProUGUI blueRobot;

    public TextMeshProUGUI otherRedRobot;
    public TextMeshProUGUI otherBlueRobot;

    public ScoreText(RobotSelector blueRobotSelector)
    {
        this.blueRobotSelector = blueRobotSelector;
    }

    private void OnEnable()
    {
        //red auto stuff
        redAutoAmpScore.text = "Amp: " + GameScoreTracker.RedAutoAmpPoints;
        redAutoSpeakerScore.text = "Speaker: " + GameScoreTracker.RedAutoSpeakerPoints;
        redAutoLeaveScore.text = "Mobility: " + GameScoreTracker.RedAutoLeavePoints;
        redAutoPenalty.text = "Penalty: " + GameScoreTracker.RedAutoPenaltyPoints;

        //red teleop stuff
        redTeleopAmpScore.text = "Amp: " + GameScoreTracker.RedTeleopAmpPoints;
        redTeleopSpeakerScore.text = "Speaker: " + GameScoreTracker.RedTeleopSpeakerPoints;
        redStageScore.text = "Stage: " + GameScoreTracker.RedStagePoints;
        redTrapScore.text = "Trap: " + GameScoreTracker.RedTrapPoints;
        redTeleopPenalty.text = "Penalty: " + GameScoreTracker.RedTeleopPenaltyPoints;

        totalRedScore.text = Score.redScore.ToString();

        //blue auto stuff
        blueAutoConeScore.text = "Cones: " + GameScoreTracker.BlueAutoConePoints;
        blueAutoCubeScore.text = "Cubes: " + GameScoreTracker.BlueAutoCubePoints;
        blueAutoLeaveScore.text = "Mobility: " + GameScoreTracker.BlueAutoLeavePoints;
        blueAutoPenalty.text = "Penalty: " + GameScoreTracker.BlueAutoPenaltyPoints;

        //blue teleop stuff
        blueTeleopConeScore.text = "Cones: " + GameScoreTracker.BlueTeleopConePoints;
        blueTeleopCubeScore.text = "Cubes: " + GameScoreTracker.BlueTeleopCubePoints;
        blueChargeStationScore.text = "Charge Station: " + GameScoreTracker.BlueChargeStationPoints;
        blueTeleopPenalty.text = "Penalty: " + GameScoreTracker.BlueTeleopPenaltyPoints;

        totalBlueScore.text = Score.blueScore.ToString();

        var blue = PlayerPrefs.GetInt("blueRobotSettings");
        var red = PlayerPrefs.GetInt("redRobotSettings");

        if (PlayerPrefs.GetInt("gamemode") == 0)
        {
            otherRedRobot.text = string.Empty;
            otherBlueRobot.text = string.Empty;
            if (PlayerPrefs.GetString("alliance") == "blue")
            {
                blueRobot.text = blueRobotSelector.GetRobotList()[blue].robotNumber.ToString();
                redRobot.text = string.Empty;
            }
            else
            {
                redRobot.text = redRobotSelector.GetRobotList()[red].robotNumber.ToString();
                blueRobot.text = string.Empty;
            }
        }
        else if (PlayerPrefs.GetInt("gamemode") == 1)
        {
            otherRedRobot.text = string.Empty;
            otherBlueRobot.text = string.Empty;

            redRobot.text = redRobotSelector.GetRobotList()[red].robotNumber.ToString();
            blueRobot.text = blueRobotSelector.GetRobotList()[blue].robotNumber.ToString();
        }
        else if (PlayerPrefs.GetInt("gamemode") == 2)
        {
            if (PlayerPrefs.GetString("alliance") == "blue")
            {
                blueRobot.text = blueRobotSelector.GetRobotList()[blue].robotNumber.ToString();
                otherBlueRobot.text = redRobotSelector.GetRobotList()[red].robotNumber.ToString();
                redRobot.text = string.Empty;
                otherRedRobot.text = string.Empty;
            }
            else
            {
                redRobot.text = redRobotSelector.GetRobotList()[red].robotNumber.ToString();
                otherRedRobot.text = blueRobotSelector.GetRobotList()[blue].robotNumber.ToString();
                blueRobot.text = string.Empty;
                otherBlueRobot.text = string.Empty;
            }
        }
    }
}
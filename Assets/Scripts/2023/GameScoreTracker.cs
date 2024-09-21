using UnityEngine;

public class GameScoreTracker : MonoBehaviour
{
    public static int BlueTeleopConePoints { get; set; }
    public static int BlueAutoConePoints { get; set; }
    public static int BlueTeleopCubePoints { get; set; }
    public static int BlueAutoCubePoints { get; set; }
    public static int BlueAutoLeavePoints { get; set; }
    public static int BlueAutoPenaltyPoints { get; set; }
    public static int BlueTeleopPenaltyPoints { get; set; }
    public static int BlueChargeStationPoints { get; set; }

    public static int RedTeleopSpeakerPoints { get; set; }
    public static int RedAutoSpeakerPoints { get; set; }
    public static int RedTeleopAmpPoints { get; set; }
    public static int RedAutoAmpPoints { get; set; }
    public static int RedAutoLeavePoints { get; set; }
    public static int RedAutoPenaltyPoints { get; set; }
    public static int RedTeleopPenaltyPoints { get; set; }
    public static int RedStagePoints { get; set; }
    public static int RedTrapPoints { get; set; }

    private void Start()
    {
        ResetScore();
    }

    private static void ResetScore() 
    {
        BlueTeleopConePoints = 0;
        BlueAutoConePoints = 0;
        BlueTeleopCubePoints = 0;
        BlueAutoCubePoints = 0;
        BlueAutoLeavePoints = 0;
        BlueAutoPenaltyPoints = 0;
        BlueTeleopPenaltyPoints = 0;
        BlueChargeStationPoints = 0;

        RedTeleopSpeakerPoints = 0;
        RedAutoSpeakerPoints = 0;
        RedTeleopAmpPoints = 0;
        RedAutoAmpPoints = 0;
        RedAutoLeavePoints = 0;
        RedAutoPenaltyPoints = 0;
        RedTeleopPenaltyPoints = 0;
        RedStagePoints = 0;
        RedTrapPoints = 0;
    }
}

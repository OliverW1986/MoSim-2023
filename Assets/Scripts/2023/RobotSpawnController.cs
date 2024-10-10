using Unity.Cinemachine;
using UnityEngine;

public class RobotSpawnController : MonoBehaviour
{
    private int _gamemode;
    private int _blueRobotIndex;
    private int _redRobotIndex;

    private static bool _isMultiplayer;
    public static bool sameAlliance;

    [SerializeField] private GameObject[] robotPrefabs;

    [SerializeField] private GameObject cameraBorder;

    [SerializeField] private Transform blueSpawn;
    [SerializeField] private Transform secondaryBlueSpawn;
    [SerializeField] private Transform redSpawn;
    [SerializeField] private Transform secondaryRedSpawn;

    [SerializeField] private Material redBumperMaterial;
    [SerializeField] private Material blueBumperMaterial;
    
    [SerializeField] public GameObject bluePanCam;
    [SerializeField] public GameObject redPanCam;
    
    private void Start()
    {
        cameraBorder.SetActive(false);

        _gamemode = PlayerPrefs.GetInt("gamemode");
        _redRobotIndex = PlayerPrefs.GetInt("redRobotSettings");
        _blueRobotIndex = PlayerPrefs.GetInt("blueRobotSettings");

        switch (_gamemode)
        {
            case 2:
                _isMultiplayer = true;
                break;
            case 3:
                sameAlliance = true;
                break;
            default:
                _isMultiplayer = false;
                sameAlliance = false;
                break;
        }

        if (_isMultiplayer)
        {
            cameraBorder.SetActive(true);

            //Instantiate(redRobotPrefabs[_redRobotIndex], redSpawn.position, redSpawn.rotation);
            
            Instantiate(robotPrefabs[_blueRobotIndex], blueSpawn.position, blueSpawn.rotation);
        }
        else if (sameAlliance)
        {
            cameraBorder.SetActive(true);

            if (PlayerPrefs.GetString("alliance") == "red")
            {
                //Instantiate(redRobotPrefabs[_redRobotIndex], redSpawn.position, redSpawn.rotation);

                //Instantiate(secondaryRedRobotPrefabs[_blueRobotIndex], secondaryRedSpawn.position,
                    //secondaryRedSpawn.rotation);
            }
            else
            {
                Instantiate(robotPrefabs[_blueRobotIndex], blueSpawn.position, blueSpawn.rotation);

                //Instantiate(secondaryBlueRobotPrefabs[_redRobotIndex], secondaryBlueSpawn.position,
                   // secondaryBlueSpawn.rotation);
            }
        }
        else
        {
            if(_blueRobotIndex >= robotPrefabs.Length)
            {
                _blueRobotIndex = robotPrefabs.Length - 1;
            }

            //Set correct robots & cameras active
            if (PlayerPrefs.GetString("alliance") == "red")
            {
                var robot = Instantiate(robotPrefabs[_blueRobotIndex], redSpawn.position, redSpawn.rotation);
                var driveController = robot.GetComponent<DriveController>();
                driveController.isRedRobot = true;
                driveController.materialPrefab = redBumperMaterial;
                //driveController.reverseBumperAllianceText = true;
                var cam = robot.GetComponentInChildren<CinemachineFollow>();
                cam.FollowOffset = new Vector3(-14.5f, 11.9f, 0);
            }
            else
            {
                var robot = Instantiate(robotPrefabs[_blueRobotIndex], blueSpawn.position, blueSpawn.rotation);
                var driveController = robot.GetComponent<DriveController>();
                driveController.isRedRobot = false;
                driveController.materialPrefab = blueBumperMaterial;
                driveController.reverseBumperAllianceText = false;
                var cam = robot.GetComponentInChildren<CinemachineFollow>();
                cam.FollowOffset = new Vector3(14.5f, 11.9f, 0);
            }
        }
    }
}
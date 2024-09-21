using UnityEngine;

[CreateAssetMenu]
public class RobotSelector : ScriptableObject
{
    [SerializeField] private Robot[] robot;
    
    public int RobotCount => robot.Length;

    public Robot GetRobot(int index) 
    {
        return robot[index];
    }
    
    public Robot[] GetRobotList()
    {
        return robot;
    }
}
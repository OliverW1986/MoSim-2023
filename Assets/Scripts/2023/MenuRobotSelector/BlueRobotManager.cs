using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlueRobotManager : MonoBehaviour 
{
    public RobotSelector rs;

    public GameObject shotBlocker;

    public TextMeshProUGUI nameText;
    public Image robotImage;

    private int _selectedOption = 1;
    
    private void Start() 
    {
        if (PlayerPrefs.HasKey("blueRobotSettings")) 
        {
            _selectedOption = PlayerPrefs.GetInt("blueRobotSettings");
        }
        UpdateCharacter(_selectedOption);
        UpdateShotBlockerToggle();
    }

    public void NextOption() 
    {
        _selectedOption++;

        if (_selectedOption >= rs.RobotCount) 
        {
            _selectedOption = 0;
        }

        UpdateCharacter(_selectedOption);
        PlayerPrefs.SetInt("blueRobotSettings", _selectedOption);

        UpdateShotBlockerToggle();
    }

    public void BackOption() 
    {
        _selectedOption--;

        if (_selectedOption < 0) 
        {
            _selectedOption = rs.RobotCount - 1;
        }

        UpdateCharacter(_selectedOption);
        PlayerPrefs.SetInt("blueRobotSettings", _selectedOption);

        UpdateShotBlockerToggle();
    }

    private void UpdateShotBlockerToggle() 
    {
        //Shotblocker only for citrus circuits
        if (_selectedOption == 2)
        {
            shotBlocker.SetActive(true);
        }
        else 
        {
            shotBlocker.SetActive(false);
        }
    }

    private void UpdateCharacter(int selectedOption) 
    {   
        var robot = rs.GetRobot(selectedOption);
        robotImage.sprite = robot.robotImage;
        nameText.text = robot.robotName;
    }
}

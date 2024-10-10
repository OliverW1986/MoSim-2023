using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RedRobotManager : MonoBehaviour 
{
    public RobotSelector rs;

    public GameObject shotBlocker;

    public TextMeshProUGUI nameText;
    public Image robotImage;

    private int _selectedOption;
    
    void Start() 
    {
        if (PlayerPrefs.HasKey("redRobotSettings")) 
        {
            _selectedOption = PlayerPrefs.GetInt("redRobotSettings");
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
        PlayerPrefs.SetInt("redRobotSettings", _selectedOption);

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
        PlayerPrefs.SetInt("redRobotSettings", _selectedOption);

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
        nameText.text = robot.robotName;
    }
}

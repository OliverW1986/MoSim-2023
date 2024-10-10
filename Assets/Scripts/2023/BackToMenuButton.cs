using UnityEngine;

public class BackToMenuButton : MonoBehaviour
{
    private ChargedUp _controls;

    private void Start() 
    {
        _controls = new ChargedUp();
        _controls.Player.Enable();
    }

    private void Update()
    {
        if (!_controls.Player.Menu.IsPressed() && !Input.GetKeyDown(KeyCode.Escape)) return;
        _controls.Player.Disable();
        LoadMenu();
    }

    private void OnDisable()
    {
        try
        {
            _controls.Player.Menu.Disable();
        }
        catch
        {
            // ignored
        }
    }

    public void LoadMenu() 
    {
        LevelManager.Instance.LoadScene("MainMenu", "CrossFade");
    }
}

using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const int MatchDuration = 150;
    public static GameState GameState { get; private set; }

    private float _timer;

    public TextMeshProUGUI timerText;

    private AudioSource _player;
    [SerializeField] private AudioResource auto;
    [SerializeField] private AudioResource teleop;
    [SerializeField] private AudioResource endgame;
    [SerializeField] private AudioResource end;

    private bool _triggerEnd = true;
    private bool _triggerTeleop = true;
    private bool _triggerEndgame = true;

    private bool _isResetting;

    private bool _countdown = true;
    public static bool canRobotMove { get; private set; }

    [SerializeField] private GameObject button;
    [SerializeField] private GameObject videoPlayer;

    public static bool isDisabled;

    public string[] tagsToDestroy;

    [SerializeField] private GameObject scoreCard;

    private IResettable[] _resettables;

    private DriveController[] _swerveControllers;
    
    public RobotSpawnController robotSpawnController;

    public Tape blueTape;
    public Tape redTape;


    private const int ShowScoreDelay = 4;
    private const int AutoToTeleopDelay = 3;

    public static bool endBuzzerPlaying;

    private void Start()
    {
        _swerveControllers = FindObjectsByType<DriveController>(FindObjectsSortMode.None);

        //Find all script instances that implement the IResettable interface
        _resettables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IResettable>().ToArray();

        //Set initial gamestate
        GameState = GameState.Auto;

        //Reset flags and get components
        _player = GetComponent<AudioSource>();

        ResetTimer();
        canRobotMove = true;
        
        robotSpawnController = GameObject.Find("RobotSpawnController").GetComponent<RobotSpawnController>();
    }

    //Methods runs once every update timestep
    private void Update()
    {
        if (!_isResetting) 
        {
            //Key binds to forcefully end match prematurely
            // if (Input.GetKeyDown(KeyCode.T))
            // {
            //     ForceEndMatch();
            // }

            if (_countdown) { _timer -= Time.deltaTime; }

            if (_timer <= 0f && _triggerEnd)
            {
                endBuzzerPlaying = true;
                isDisabled = true;
                _triggerEnd = false;
                _timer = 0f;
                GameState = GameState.End;
                _player.resource = end;
                _player.Play();
                StartCoroutine(EndBuzzerTracker());
                _countdown = false;
                timerText.color = Color.red;
                StartCoroutine(ShowMatchScore());
            }
            else if (_timer < 136f && _triggerTeleop)
            {
                _triggerTeleop = false;
                StartCoroutine(Wait());
            }
            else if (_timer <= 30f && _triggerEndgame)
            {
                _triggerEndgame = false;
                GameState = GameState.Endgame;
                _player.resource = endgame;
                _player.Play();
            }
            UpdateTimerDisplay(_timer);
        }
    }

    private IEnumerator EndBuzzerTracker() 
    {
        while (_player.isPlaying) 
        {
            yield return null;
        } 
        endBuzzerPlaying = false;
    }

    private IEnumerator ShowMatchScore()
    {
        button.SetActive(true);

        //Waits for a constant delay until showing the video and/or score button
        yield return new WaitForSeconds(ShowScoreDelay);

        //Load the player preference for showing the match win video
        if (Mathf.Approximately(PlayerPrefs.GetFloat("endVideo"), 1)) { videoPlayer.SetActive(true); }
    }

    //Creates a pause inbetween auto and teleop, plays sounds, and set the note worths correctly
    IEnumerator Wait()
    {
        _countdown = false;
        _player.resource = end;
        _player.Play();
        canRobotMove = false;
        isDisabled = true;

        yield return new WaitForSeconds(AutoToTeleopDelay);

        isDisabled = false;
        GameState = GameState.Teleop;
        _player.resource = teleop;
        _player.Play();
        canRobotMove = true;
        _countdown = true;
    }

    private void ForceEndMatch() 
    {   
        StopAllCoroutines();
        _triggerEnd = true;
        _triggerEndgame = false;
        _triggerTeleop = false;
        _timer = 0;
    }

    //Resets timer to 0 and sets color back to white
    private void ResetTimer() 
    {
        _player.resource = auto;
        _player.Play();

        isDisabled = false;
        _countdown = true;
        _triggerEnd = true;
        _triggerEndgame = true;
        _triggerTeleop = true;
        endBuzzerPlaying = false;

        _timer = MatchDuration;
        timerText.color = Color.white;
    }

    //Resets game without having to reload entire scene
    public static void ResetMatch()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // StopAllCoroutines();
        // _isResetting = true;
        //
        // //Play fade animation
        // LevelManager.Instance.PlayTransition("CrossFade");
        //
        // //Disable scorecard (if on)
        // scoreCard.SetActive(false);
        //
        // //Stop any sounds
        // _player.Stop();
        //
        // //Reset initial gamestate
        // GameState = GameState.Auto;
        //
        // //Reset stored notes
        // blueSpeaker.ResetSpeaker();
        // redSpeaker.ResetSpeaker();
        // blueAmp.ResetAmp();
        // redAmp.ResetAmp();
        //
        // //Reset speaker lights
        // blueSpeakerLights.ResetLights();
        // redSpeakerLights.ResetLights();
        //
        // //Reset note worth's
        // blueAmp.SetNoteWorth(2);
        // redAmp.SetNoteWorth(2);
        //
        // //Reset things on field
        // foreach (var tagToDestroy in tagsToDestroy)
        // {
        //     if (GameObject.FindGameObjectsWithTag(tagToDestroy) == null) continue;
        //     var notes = GameObject.FindGameObjectsWithTag(tagToDestroy);
        //
        //     foreach (var note in notes)
        //     {
        //         Destroy(note);
        //     }
        // }
        //
        // //Reset default field notes
        // fieldNotes.InstantiateNotes();
        //
        // //Reset auto mobility potential
        // blueTape.Reset();
        // redTape.Reset();
        //
        // //Reset scores and scoring stats
        // Score.ResetScores();
        // GameScoreTracker.ResetScore();
        //
        // foreach (var controller in _swerveControllers) 
        // {
        //     controller.StopAmplifiedSpeaker();
        // }
        //
        // ZoneControl.ResetController();
        // PenaltyManager.ResetPenaltyManager();
        //
        // // foreach (IResettable script in resettables) 
        // // {
        // //     //Reset every script that implements the IResettable interface
        // //     script.Reset();
        // // }
        //
        // canRobotMove = true;
        // button.SetActive(false);
        // _isResetting = false;
        //
        // //Reset the timer to prepare for the match
        // ResetTimer();
    }

    //Updates the timer to correctly display the minutes and seconds left
    private void UpdateTimerDisplay(float time)
    {
        var minutes = Mathf.FloorToInt(time / 60);
        var seconds = Mathf.FloorToInt(time % 60);

        var currentTime = $"{minutes}:{seconds:00}";

        timerText.text = currentTime;
    }

    public static void ToggleCanRobotMove()
    {
        canRobotMove = !canRobotMove;
    }
}

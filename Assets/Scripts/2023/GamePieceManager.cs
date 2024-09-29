using System.Collections;
using PathCreation;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class GamePieceManager : MonoBehaviour
{
    [SerializeField] private RobotSettings robotSettings;
    public Alliance alliance;
    
    [SerializeField] private PathCreator conePath;
    [SerializeField] private Transform conePathEnd;
    [SerializeField] private Transform conePathEndAnchor;

    [SerializeField] private PathCreator cubePath;
    [SerializeField] private Transform cubePathEnd;
    [SerializeField] private Transform cubePathEndAnchor;

    public bool doConePath;
    public bool doCubePath;

    [SerializeField] private GameObject conePrefab;
    [SerializeField] private GameObject cubePrefab;

    [SerializeField] private Transform coneSpawnPoint;
    [SerializeField] private Transform cubeSpawnPoint;
    [SerializeField] private float coneSpeed;
    [SerializeField] private Vector3 coneEjectDirection = new Vector3(0,0,1);
    [SerializeField] private float cubeSpeed;
    [SerializeField] private float placeLatency;
    [SerializeField] private float intakeLatency;

    [SerializeField] private AudioSource player;
    [SerializeField] private AudioResource placeSound;

    [SerializeField] private GameObject hiddenCone;
    [SerializeField] private GameObject hiddenCube;
    public bool hasGamePiece;
    public GamePieceType currentGamePieceMode;
    public GamePieceType currentGamePiece;
    public GameObject touchedGamePiece { get; set; }

    public bool coneWithinIntakeConeCollider { get; set; }
    public bool cubeWithinIntakeCubeCollider { get; set; }

    public bool isPlacing;
    private float _placeValue;
    public bool requestPlace;

    public bool canPlace = true;

    private DriveController _driveController;
    private LedStripController _ledStripController;

    private InputAction _gamePiecePlace;
    private InputAction _gamePieceMode;
    private InputAction _spawnGamePiece;
    
    private SingleSubstation _singleSubstation;

    public bool shouldSpawnCone;

    private void Start()
    {
        _driveController = GetComponent<DriveController>();
        _ledStripController = GetComponent<LedStripController>();

        hiddenCube.SetActive(currentGamePieceMode == GamePieceType.Cube);
        hiddenCone.SetActive(currentGamePieceMode == GamePieceType.Cone);

        hasGamePiece = true;

        if (conePath != null)
        {
            if (conePathEnd != null)
            {
                conePath.bezierPath.MovePoint(3, conePath.transform.InverseTransformPoint(conePathEnd.position));
            }

            if (conePathEndAnchor != null)
            {
                conePath.bezierPath.MovePoint(2, conePath.transform.InverseTransformPoint(conePathEndAnchor.position));
            }
        }
        else
        {
            doConePath = false;
        }

        if (cubePath != null)
        {
            if (cubePathEnd != null)
            {
                cubePath.bezierPath.MovePoint(3, cubePath.transform.InverseTransformPoint(cubePathEnd.position));
            }

            if (cubePathEndAnchor != null)
            {
                cubePath.bezierPath.MovePoint(2, cubePath.transform.InverseTransformPoint(cubePathEndAnchor.position));
            }
        } 
        else
        {
            doCubePath = false;
        }

        _gamePiecePlace = InputSystem.actions.FindAction("Place");
        _gamePieceMode = InputSystem.actions.FindAction("GamePieceMode");
        _spawnGamePiece = InputSystem.actions.FindAction("SpawnSingleSubstation");

        if (robotSettings != RobotSettings.Bread)
        {
            _gamePiecePlace.performed += context => requestPlace = true;
            _gamePiecePlace.canceled += context => requestPlace = false;
        }
        
        alliance = PlayerPrefs.GetString("alliance").Equals("red") ? Alliance.Red : Alliance.Blue;
        
        _singleSubstation = alliance == Alliance.Blue ? GameObject.Find("BlueSingleSubstation").GetComponent<SingleSubstation>() : GameObject.Find("RedSingleSubstation").GetComponent<SingleSubstation>();
    }

    private void Update()
    {
        if (!GameManager.canRobotMove) return;

        if (isPlacing) return;

        if (_gamePieceMode.triggered)
        {
            currentGamePieceMode = currentGamePieceMode == GamePieceType.Cone ? GamePieceType.Cube : GamePieceType.Cone;
        }
        else if (hasGamePiece)
        {
            currentGamePieceMode = currentGamePiece;
        }

        if (requestPlace && hasGamePiece && !isPlacing && canPlace)
        {
            isPlacing = true;
            StartCoroutine(currentGamePiece == GamePieceType.Cone
                ? PlaceSequence(GamePieceType.Cone)
                : PlaceSequence(GamePieceType.Cube));
        }

        if (touchedGamePiece != null && coneWithinIntakeConeCollider && conePath != null)
        {
            // conePath.bezierPath.MovePoint(0,
            //     conePath.transform.InverseTransformPoint(touchedGamePiece.transform.position));
        }
        else if (touchedGamePiece != null && cubeWithinIntakeCubeCollider && cubePath != null)
        {
             cubePath.bezierPath.MovePoint(0,
                 cubePath.transform.InverseTransformPoint(touchedGamePiece.transform.position));
        }

        if (coneWithinIntakeConeCollider && !hasGamePiece && _driveController.isIntaking)
        {
            hasGamePiece = true;
            IntakeSequence(GamePieceType.Cone);
            _ledStripController.Flash();
        }
        else if (cubeWithinIntakeCubeCollider && !hasGamePiece && _driveController.isIntaking)
        {
            hasGamePiece = true;
            IntakeSequence(GamePieceType.Cube);
            _ledStripController.Flash();
        }
        
        if (_spawnGamePiece.triggered)
        {
            _singleSubstation.gamePieceType = currentGamePieceMode;
            _singleSubstation.requestSpawn = true;
        }

        if(shouldSpawnCone)
        {
            shouldSpawnCone = false;

            _singleSubstation.gamePieceType = GamePieceType.Cone;
            _singleSubstation.requestSpawn = true;
        }
    }

    private void IntakeSequence(GamePieceType gamePieceType)
    {
        if (gamePieceType == GamePieceType.Cone)
        {
            coneWithinIntakeConeCollider = false;
        }
        else
        {
            cubeWithinIntakeCubeCollider = false;
        }

        currentGamePiece = gamePieceType;

        if (currentGamePiece == GamePieceType.Cone && doConePath)
        {
            StartCoroutine(GamePieceSplineAnimation(currentGamePiece));
        }
        else if (currentGamePiece == GamePieceType.Cube && doCubePath)
        {
            StartCoroutine(GamePieceSplineAnimation(currentGamePiece));
        }
        else
        {
            Destroy(touchedGamePiece);
            if (gamePieceType == GamePieceType.Cone)
            {
                hiddenCone.SetActive(true);
                hiddenCube.SetActive(false);
            }
            else
            {
                hiddenCube.SetActive(true);
                hiddenCone.SetActive(false);
            }
        }
        
        StartCoroutine(CanNotEjectWhenRunnning());
    }

    private IEnumerator CanNotEjectWhenRunnning()
    {
        canPlace = false;
        yield return new WaitForSeconds(intakeLatency);
        canPlace = true;
    }

    private IEnumerator GamePieceSplineAnimation(GamePieceType gamePieceType)
    {
        if (gamePieceType == GamePieceType.Cone)
        {
            var cone = touchedGamePiece;
            
            var coneChildren = cone.GetComponentsInChildren<BoxCollider>();
            foreach (var child in coneChildren)
            {
                child.gameObject.layer = 12;
            }

            cone.transform.SetParent(hiddenCone.transform.parent);
            var distanceTraveled = 0f;
            const float intakeSpeed = 2.5f;

            while (distanceTraveled < conePath.path.length)
            {
                distanceTraveled += intakeSpeed * Time.deltaTime;
                cone.transform.position = conePath.path.GetPointAtDistance(distanceTraveled, EndOfPathInstruction.Stop);
                // cone.transform.rotation =
                //     conePath.path.GetRotationAtDistance(distanceTraveled, EndOfPathInstruction.Stop);

                var t = distanceTraveled / conePath.path.length;
                cone.transform.localScale = Vector3.Lerp(cone.transform.localScale, hiddenCone.transform.localScale, t);

                if (!hasGamePiece)
                {
                    break;
                }

                yield return null;
            }

            Destroy(cone);

            if (hasGamePiece)
            {
                hiddenCone.SetActive(true);
            }
        }
        else
        {
            var cube = touchedGamePiece;

            cube.tag = "ignore";
            cube.layer = 12;
            Destroy(cube.GetComponent<BoxCollider>());

            cube.transform.SetParent(hiddenCube.transform.parent);
            var distanceTraveled = 0f;
            const float intakeSpeed = 2.5f;

            while (distanceTraveled < cubePath.path.length)
            {
                distanceTraveled += intakeSpeed * Time.deltaTime;
                cube.transform.position = cubePath.path.GetPointAtDistance(distanceTraveled, EndOfPathInstruction.Stop);
                cube.transform.rotation =
                    cubePath.path.GetRotationAtDistance(distanceTraveled, EndOfPathInstruction.Stop);

                var t = distanceTraveled / cubePath.path.length;
                cube.transform.localScale = Vector3.Lerp(cube.transform.localScale, hiddenCube.transform.localScale, t);

                if (!hasGamePiece)
                {
                    break;
                }

                yield return null;
            }

            Destroy(cube);

            if (hasGamePiece)
            {
                hiddenCube.SetActive(true);
            }
        }
    }

    public IEnumerator PlaceSequence(GamePieceType gamePieceType)
    {
        player.resource = placeSound;
        player.Play();

        yield return new WaitForSeconds(placeLatency);

        if (gamePieceType == GamePieceType.Cone)
        {
            var cone = Instantiate(conePrefab, coneSpawnPoint.position, coneSpawnPoint.rotation);

            var coneChildren = cone.GetComponentsInChildren<BoxCollider>();
            foreach (var child in coneChildren)
            {
                child.gameObject.layer = 12;
            }

            var rb = cone.GetComponent<Rigidbody>();
            rb.linearVelocity = GetComponent<Rigidbody>().linearVelocity;
                               

            if(robotSettings == RobotSettings.MechanicalAdvantage)
            {
                rb.AddRelativeForce(coneEjectDirection * coneSpeed, ForceMode.Impulse);
            } else
            {
                rb.linearVelocity += coneSpawnPoint.forward.normalized * coneSpeed;
            }
            hiddenCone.SetActive(false);

            var conePosition = cone.transform.position;

            yield return new WaitForSeconds(0.5f);
            
            cone.layer = 0;

            foreach (var child in coneChildren)
            {
                child.gameObject.layer = 0;
            }
            
            hiddenCone.SetActive(false);
            hiddenCube.SetActive(false);
            hasGamePiece = false;
            isPlacing = false;

            if (!_driveController.inCommunity)
            {
                yield return new WaitForSeconds(1.0f);
            
                if (Vector3.Distance(conePosition, cone.transform.position) > 10f)
                {
                    Score.AddScore(12, alliance == Alliance.Red ? Alliance.Blue : Alliance.Red);
                    Debug.Log("Blue Penalty");
                }
            }
        }
        else
        {
            var cube = Instantiate(cubePrefab, cubeSpawnPoint.position, cubeSpawnPoint.rotation);
            cube.layer = 12;
            var rb = cube.GetComponent<Rigidbody>();
            rb.linearVelocity = GetComponent<Rigidbody>().linearVelocity +
                                (cubeSpawnPoint.forward.normalized * cubeSpeed);
            hiddenCube.SetActive(false);
            
            var cubePosition = cube.transform.position;
            
            yield return new WaitForSeconds(0.25f);
            cube.layer = 0;
            
            hiddenCone.SetActive(false);
            hiddenCube.SetActive(false);
            hasGamePiece = false;
            isPlacing = false;

            if (!_driveController.inCommunity)
            {
                yield return new WaitForSeconds(1.0f);
            
                if (Vector3.Distance(cubePosition, cube.transform.position) > 10f)
                {
                    Score.AddScore(12, alliance == Alliance.Red ? Alliance.Blue : Alliance.Red);
                    Debug.Log("Blue Penalty");
                }
            }
        }

        StartCoroutine(CanNotEjectWhenRunnning());
    }

    public float getPlaceLatency()
    {
        return placeLatency;
    }
}

public enum GamePieceType
{
    Cone,
    Cube
}
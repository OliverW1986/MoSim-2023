using System.Collections;
using System.Linq;
using UnityEngine;

public class SubstationGamePieceLoader : MonoBehaviour
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private GameObject cone;
    [SerializeField] private GameObject cube;
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private ConfigurableJoint shelfJoint;
    [SerializeField] private float shelfDistance;
    [SerializeField] private float spawnDelay;
    [SerializeField] private Collider shelfCollider;

    [SerializeField] private GameObject hiddenCone;
    [SerializeField] private GameObject hiddenCube;

    [SerializeField] private bool isCone;

    private GameObject _spawnedGameObject;
    private Collider _spawnCollider;

    private bool _canSpawn = true;
    private bool _enabled;
    // private bool _requestSpawn;
    
    private void Start()
    {
        hiddenCone.SetActive(false);
        hiddenCube.SetActive(false);
        _spawnCollider = GetComponent<BoxCollider>();

        if (PlayerPrefs.GetString("alliance").Equals("red") && alliance == Alliance.Red)
        {
            _enabled = true;
        }
        else if (PlayerPrefs.GetString("alliance").Equals("blue") && alliance == Alliance.Blue)
        {
            _enabled = true;
        }
    }

    private void Update()
    {
        if (!_canSpawn || !_enabled) return;
        _canSpawn = false;
        // _requestSpawn = false;
        if (isCone)
        {
            hiddenCone.SetActive(true);
        }
        else
        {
            hiddenCube.SetActive(true);
        }
        StartCoroutine(MoveShelf());
    }

    private IEnumerator MoveShelf()
    {
        shelfJoint.targetPosition = new Vector3(0, shelfDistance, 0);
        yield return new WaitForSeconds(spawnDelay);
        _spawnedGameObject = Instantiate(isCone ? cone : cube, spawnLocation.position, spawnLocation.rotation);
        hiddenCone.SetActive(false);
        hiddenCube.SetActive(false);
        while (CheckGamePiece())
        {
            yield return null;
        }

        StartCoroutine(HomeShelf());
    }

    private bool CheckGamePiece()
    {
        var colliders = Physics.OverlapBox(shelfCollider.bounds.center, shelfCollider.bounds.extents,
            shelfCollider.transform.rotation);
        return colliders.Any(collider => collider.CompareTag("Cone") || collider.CompareTag("Cube"));
    }

    private IEnumerator HomeShelf()
    {
        // Destroy(_spawnedGameObject);
        shelfJoint.targetPosition = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(spawnDelay);
        _canSpawn = true;
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     var colliders = Physics.OverlapBox(_spawnCollider.bounds.center, _spawnCollider.bounds.extents,
    //         _spawnCollider.transform.rotation);
    //     foreach (var collider in colliders)
    //         if (collider.CompareTag("Player"))
    //         {
    //             // _requestSpawn = true;
    //             break;
    //         }
    // }
}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SingleSubstation : MonoBehaviour
{
    [SerializeField] private GameObject cone;
    [SerializeField] private GameObject cube;
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private float spawnForce;
    [SerializeField] private float spawnDelay;

    public bool requestSpawn;
    public GamePieceType gamePieceType;
    private bool _canSpawn = true;
    
    private InputAction _spawnGamePiece;
    
    private void Start()
    {
        _spawnGamePiece = InputSystem.actions.FindAction("SpawnSingleSubstation");
    }
    
    private void Update()
    {
        if (requestSpawn && _canSpawn)
        {
            requestSpawn = false;
            _canSpawn = false;
            var spawnedGameObject = Instantiate(cube, spawnLocation.position, Quaternion.identity);
            spawnedGameObject.GetComponent<Rigidbody>().AddForce(spawnLocation.forward * spawnForce, ForceMode.VelocityChange);
            spawnedGameObject.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * spawnForce, ForceMode.VelocityChange);
            StartCoroutine(ResetSpawn());
        }
    }
    
    private IEnumerator ResetSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        _canSpawn = true;
    }
}
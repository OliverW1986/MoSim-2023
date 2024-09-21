using UnityEngine;

public class SpawnGameObjects : MonoBehaviour
{
    [SerializeField] private GameObject cone;
    [SerializeField] private GameObject cube;

    [SerializeField] private Transform[] spawnLocations;

    private void Start()
    {
        SyncAutoPieces();

        foreach (var location in spawnLocations)
        {
            Instantiate(location.gameObject.CompareTag("Cube") ? cube : cone, location.position, location.rotation);
        }
    }

    private void SyncAutoPieces()
    {
        foreach (var location in spawnLocations)
        {
            if (PlayerPrefs.GetInt(location.name) == 1)
            {
                location.tag = "Cube";
            }
            else
            {
                location.tag = "Cone";
            }
        }
    }
}
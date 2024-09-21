using UnityEngine;

public class CommunityZone : MonoBehaviour
{
    [SerializeField] private Collider[] communityZones;
    public bool hasRobot;

    private void Update()
    {
        hasRobot = false;
        foreach (var zone in communityZones)
        {
            var colliders = Physics.OverlapBox(zone.bounds.center, zone.bounds.extents, Quaternion.identity);
            foreach (var otherCollider in colliders)
            {
                if (!otherCollider.CompareTag("Player")) continue;
                hasRobot = true;
            }
        }
    
        GameObject.FindWithTag("Player").GetComponent<DriveController>().inCommunity = hasRobot;
    }
}
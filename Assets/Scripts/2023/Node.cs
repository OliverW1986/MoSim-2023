using UnityEngine;

public enum NodeType
{
    High,
    Middle,
    Hybrid
}

public class Node : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private NodeType nodeType;
    [SerializeField] private bool isCubeNode;
    [SerializeField] private bool isConeNode;

    public bool hasGamePiece;

    private void Update()
    {
        hasGamePiece = false;
        var results = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity);
        foreach (var otherCollider in results)
        {
            if (nodeType.Equals(NodeType.Hybrid))
            {
                if (!otherCollider.CompareTag("Cone") && !otherCollider.CompareTag("Cube")) continue;
                hasGamePiece = true;
                break;
            }

            if (isConeNode)
            {
                if (otherCollider.CompareTag("Cone"))
                {
                    hasGamePiece = true;
                    break;
                }
            }

            if (isCubeNode)
            {
                if (!otherCollider.CompareTag("Cube")) continue;
                hasGamePiece = true;
                break;
            }
        }
    }

    public NodeType GetNodeType => nodeType;

    private static bool IsColliderCompletelyWithin(Collider inner, Collider other)
    {
        return other.bounds.Contains(inner.bounds.min) && other.bounds.Contains(inner.bounds.max);
    }
}
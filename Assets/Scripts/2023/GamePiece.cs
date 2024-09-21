using UnityEngine;

public class GamePiece : MonoBehaviour
{
    [SerializeField] private GamePieceType gamePieceType;

    public bool _isScored;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Node"))
        {
            _isScored = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Node"))
        {
            _isScored = false;
            
            var node = GetComponent<Node>();
        }
    }

    private void OnDestroy()
    {
        _isScored = false;
        
        var node = GetComponent<Node>();
    }

    public bool GetIsScored()
    {
        return _isScored;
    }
    
    public GamePieceType GetGamePieceType()
    {
        return gamePieceType;
    }
}
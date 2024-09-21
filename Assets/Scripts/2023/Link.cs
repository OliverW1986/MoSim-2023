using System;
using UnityEngine;

public class Link : MonoBehaviour
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private Node node1;
    [SerializeField] private Node node2;
    [SerializeField] private Node node3;

    private bool _scoredLink;
    private bool _node1Scored;
    private bool _node2Scored;
    private bool _node3Scored;

    public void Update()
    {
        if (node1.hasGamePiece && node2.hasGamePiece && node3.hasGamePiece && !_scoredLink)
        {
            _scoredLink = true;
            Score.AddScore(5, alliance);
        }
        else if (_scoredLink && (!node1.hasGamePiece || !node2.hasGamePiece || !node3.hasGamePiece))
        {
            _scoredLink = false;
            Score.AddScore(-5, alliance);
        }

        _node1Scored = UpdateNodeValues(node1, _node1Scored);
        _node2Scored = UpdateNodeValues(node2, _node2Scored);
        _node3Scored = UpdateNodeValues(node3, _node3Scored);
    }

    private bool UpdateNodeValues(Node node, bool isScored)
    {
        switch (node.hasGamePiece)
        {
            case true when !isScored:
                isScored = true;
                switch (node1.GetNodeType)
                {
                    case NodeType.High:
                        Score.AddScore(GameManager.GameState.Equals(GameState.Auto) ? 6 : 5, alliance);
                        GameScoreTracker.BlueTeleopConePoints -= GameManager.GameState.Equals(GameState.Auto) ? 6 : 5;
                        break;
                    case NodeType.Middle:
                        Score.AddScore(GameManager.GameState.Equals(GameState.Auto) ? 4 : 3, alliance);
                        GameScoreTracker.BlueTeleopConePoints -= GameManager.GameState.Equals(GameState.Auto) ? 4 : 3;
                        break;
                    case NodeType.Hybrid:
                        Score.AddScore(GameManager.GameState.Equals(GameState.Auto) ? 3 : 2, alliance);
                        GameScoreTracker.BlueTeleopConePoints -= GameManager.GameState.Equals(GameState.Auto) ? 3 : 2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
            case false when isScored:
                isScored = false;
                switch (node1.GetNodeType)
                {
                    case NodeType.High:
                        Score.AddScore(GameManager.GameState.Equals(GameState.Auto) ? -6 : -5, alliance);
                        GameScoreTracker.BlueTeleopConePoints -= GameManager.GameState.Equals(GameState.Auto) ? -6 : -5;
                        break;
                    case NodeType.Middle:
                        Score.AddScore(GameManager.GameState.Equals(GameState.Auto) ? -4 : -3, alliance);
                        GameScoreTracker.BlueTeleopConePoints -= GameManager.GameState.Equals(GameState.Auto) ? -4 : -3;
                        break;
                    case NodeType.Hybrid:
                        Score.AddScore(GameManager.GameState.Equals(GameState.Auto) ? -3 : -2, alliance);
                        GameScoreTracker.BlueTeleopConePoints -= GameManager.GameState.Equals(GameState.Auto) ? -3 : -2;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
        }

        return isScored;
    }
}
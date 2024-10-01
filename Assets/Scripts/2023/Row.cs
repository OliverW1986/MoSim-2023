using System;
using UnityEngine;

public class Row : MonoBehaviour
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private Node[] nodes;

    private bool _scoredLink;

    private bool[] scoredValues;
    private int prevLinkCount;

    public void Start()
    {
        scoredValues = new bool[nodes.Length];
    }

    public void Update()
    {
        if (GameManager.GameState == GameState.End) return;

        //Check for links
        int lastStart = 0;
        bool linkStarted = false;
        int linkCount = 0;

        for(int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].hasGamePiece && !linkStarted)
            {
                linkStarted = true;
                lastStart = i;
            } else if (nodes[i].hasGamePiece && i-lastStart == 2)
            {
                linkStarted = false;
                linkCount++;
            } else if (!nodes[i].hasGamePiece)
            {
                linkStarted = false;
            }
        }

        Score.AddScore((linkCount - prevLinkCount) * 5, alliance);

        for(int i = 0; i<nodes.Length; i++)
        {
            scoredValues[i] = UpdateNodeValues(nodes[i], scoredValues[i]);
        }

        prevLinkCount = linkCount;
    }

    private bool UpdateNodeValues(Node node, bool isScored)
    {
        switch (node.hasGamePiece)
        {
            case true when !isScored:
                isScored = true;
                switch (nodes[0].GetNodeType)
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
                switch (nodes[0].GetNodeType)
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

    public bool IsScored(int index)
    {
        return scoredValues[index];
    }

    public Node getNode(int index)
    {
        return nodes[index];
    }
}
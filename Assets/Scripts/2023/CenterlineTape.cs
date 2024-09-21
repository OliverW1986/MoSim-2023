using System;
using UnityEngine;

public class CenterlineTape : MonoBehaviour
{
    private bool _redPenalized;
    private bool _bluePenalized;
    
    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || GameManager.GameState != GameState.Auto) return;
        var alliance = other.GetComponent<GamePieceManager>().alliance;
        switch (alliance)
        {
            case Alliance.Red when !_redPenalized:
                Score.blueScore += 5;
                _redPenalized = true;
                break;
            case Alliance.Blue when !_bluePenalized:
                Score.redScore += 5;
                _bluePenalized = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

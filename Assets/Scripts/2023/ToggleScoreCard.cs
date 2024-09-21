using UnityEngine;

public class ToggleScoreCard : MonoBehaviour
{
    public GameObject scoreCard;
    private bool _isEnabled;

    public void ToggleCard()
    {
        if (!_isEnabled)
        {
            scoreCard.SetActive(true);
            _isEnabled = true;
        }
        else 
        {
            scoreCard.SetActive(false);
            _isEnabled = false;
        }
    }

    public void LoadEndScoreScene() 
    {
        LevelManager.Instance.LoadScene("ScoreDisplay", "CrossFade");
    }
}

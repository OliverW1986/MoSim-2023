using UnityEngine;

public class ChargeStation : MonoBehaviour
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private Collider collider;
    [SerializeField] private Material scoredMaterial;

    public bool isScored;
    public bool isDockedScored;
    private bool _isAutoScored;
    private bool _isAutoDockedScored;
    private bool _hasRobot;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Start()
    {
        if (alliance == Alliance.Red)
        {
            scoredMaterial.SetColor(EmissionColor, new Color(191, 0, 0) * 0.2f);
        }
        else
        {
            scoredMaterial.SetColor(EmissionColor, new Color(0, 49, 191) * 0.2f);
        }
    }

    private void Update()
    {
        var isLevel = transform.localEulerAngles.y is < 3.5f and > 0 or > 356.5f and < 360;
        if (isLevel)
        {
            if (alliance == Alliance.Red)
            {
                scoredMaterial.SetColor(EmissionColor, new Color(191, 0, 0) * 0.2f);
            }
            else
            {
                scoredMaterial.SetColor(EmissionColor, new Color(0, 49, 191) * 0.2f);
            }
        }
        else
        {
            scoredMaterial.SetColor(EmissionColor, Color.black);
        }

        if (GameManager.GameState != GameState.Auto && GameManager.GameState != GameState.Endgame) return;
        _hasRobot = false;
        var colliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity);
        foreach (var otherCollider in colliders)
        {
            if (!otherCollider.CompareTag("Player") || !IsColliderCompletelyWithin(otherCollider, collider)) continue;
            _hasRobot = true;
            break;
        }

        if (_hasRobot)
        {
            if (GameManager.GameState == GameState.Auto)
            {
                if (isLevel)
                {
                    if (!_isAutoScored && !_isAutoDockedScored)
                    {
                        _isAutoScored = true;
                        Score.AddScore(12, alliance);
                        if (alliance == Alliance.Red)
                        {
                            scoredMaterial.SetColor(EmissionColor, new Color(191, 0, 0) * 10f);
                        }
                        else
                        {
                            scoredMaterial.SetColor(EmissionColor, new Color(0, 49, 191) * 10f);
                        }
                    }

                    if (_isAutoDockedScored)
                    {
                        _isAutoDockedScored = false;
                        Score.AddScore(-8, alliance);
                    }
                }
                else
                {
                    if (_isAutoScored)
                    {
                        _isAutoScored = false;
                        Score.AddScore(-12, alliance);
                        scoredMaterial.SetColor(EmissionColor, Color.black);
                    }
                    if (!_isAutoDockedScored)
                    {
                        _isAutoDockedScored = true;
                        Score.AddScore(8, alliance);
                        scoredMaterial.SetColor(EmissionColor, Color.black);
                    }
                }
            }
            else
            {
                if (isLevel)
                {
                    if (!isScored && !isDockedScored)
                    {
                        isScored = true;
                        Score.AddScore(10, alliance);
                        if (alliance == Alliance.Red)
                        {
                            scoredMaterial.SetColor(EmissionColor, new Color(191, 0, 0) * 10f);
                        }
                        else
                        {
                            scoredMaterial.SetColor(EmissionColor, new Color(0, 49, 191) * 10f);
                        }
                    }

                    if (isDockedScored)
                    {
                        isDockedScored = false;
                        Score.AddScore(-6, alliance);
                    }
                }
                else
                {
                    if (isScored)
                    {
                        isScored = false;
                        Score.AddScore(-10, alliance);
                        scoredMaterial.SetColor(EmissionColor, Color.black);
                    }
                    if (!isDockedScored)
                    {
                        isDockedScored = true;
                        Score.AddScore(6, alliance);
                        scoredMaterial.SetColor(EmissionColor, Color.black);
                    }
                }
            }
        }
        else
        {
            if (isScored)
            {
                isScored = false;
                Score.AddScore(-10, alliance);
                scoredMaterial.SetColor(EmissionColor, Color.black);
            }
            if (isDockedScored)
            {
                isDockedScored = false;
                Score.AddScore(-6, alliance);
                scoredMaterial.SetColor(EmissionColor, Color.black);
            }

            if (GameManager.GameState == GameState.Auto)
            {
                if (_isAutoScored)
                {
                    _isAutoScored = false;
                    Score.AddScore(-12, alliance);
                    scoredMaterial.SetColor(EmissionColor, Color.black);
                }
                if (_isAutoDockedScored)
                {
                    _isAutoDockedScored = false;
                    Score.AddScore(-8, alliance);
                    scoredMaterial.SetColor(EmissionColor, Color.black);
                }
            }
        }
    }

    private static bool IsColliderCompletelyWithin(Collider inner, Collider other)
    {
        return other.bounds.Contains(inner.bounds.min) && other.bounds.Contains(inner.bounds.max);
    }
}
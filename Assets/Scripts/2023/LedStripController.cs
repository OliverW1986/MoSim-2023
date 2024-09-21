using System;
using System.Collections;
using UnityEngine;

public class LedStripController : MonoBehaviour
{
    private Material _mat;
    public GameObject[] leds;

    private const int NumOfFlashes = 3;

    public bool isFlashing;

    public float intensity = 200f;

    [SerializeField] private bool useCustomColors;

    [SerializeField] private Color unlitColor;
    [SerializeField] private Color hasConeColor;
    [SerializeField] private Color hasCubeColor;
    [SerializeField] private Color noGamePieceColorCone;
    [SerializeField] private Color noGamePieceColorCube;

    private GamePieceManager _gamePieceManager;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private GamePieceType _currentGamePiece;

    private void Start()
    {
        _gamePieceManager = GetComponent<GamePieceManager>();

        _mat = new Material(Shader.Find("Standard"));
        _mat.EnableKeyword("_EMISSION");

        foreach (var led in leds)
        {
            led.GetComponent<Renderer>().material = _mat;
        }

        //Use default color scheme
        if (!useCustomColors)
        {
            unlitColor = Color.white;
            hasConeColor = Color.yellow;
            hasCubeColor = Color.magenta;
            noGamePieceColorCone = new Color(148, 62, 1);
            noGamePieceColorCube = new Color(57, 0, 69);
        }

        _mat.color = unlitColor;
    }

    private void Update()
    {
        _currentGamePiece = _gamePieceManager.currentGamePieceMode;

        if (isFlashing) return;
        switch (_currentGamePiece)
        {
            case GamePieceType.Cone:
                _mat.SetColor(EmissionColor, hasConeColor * intensity);
                break;
            case GamePieceType.Cube:
                _mat.SetColor(EmissionColor, hasCubeColor * intensity);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Flash()
    {
        StartCoroutine(FlashSequence());
    }

    private IEnumerator FlashSequence()
    {
        isFlashing = true;
        for (var i = 0; i < NumOfFlashes; i++)
        {
            switch (_currentGamePiece)
            {
                case GamePieceType.Cone:
                    _mat.SetColor(EmissionColor, hasConeColor * 0f);
                    yield return new WaitForSeconds(0.12f);
                    _mat.SetColor(EmissionColor, hasConeColor * intensity);
                    yield return new WaitForSeconds(0.12f);
                    break;
                case GamePieceType.Cube:
                    _mat.SetColor(EmissionColor, hasCubeColor * 0f);
                    yield return new WaitForSeconds(0.12f);
                    _mat.SetColor(EmissionColor, hasCubeColor * intensity);
                    yield return new WaitForSeconds(0.12f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        isFlashing = false;
    }
}
using System.Collections;
using UnityEngine;

public class RslLight : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float minIntensity;

    [SerializeField] private float flashDelay;
    [SerializeField] private Color color;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Start() 
    {
        StartCoroutine(RslLightFlash());
    }

    private IEnumerator RslLightFlash() 
    {
        while (true) 
        {
            while (GameManager.isDisabled)
            {
                material.SetColor(EmissionColor, color * maxIntensity);
                yield return null;
            }

            material.SetColor(EmissionColor, color * maxIntensity);
            yield return new WaitForSeconds(flashDelay);
            material.SetColor(EmissionColor, color * minIntensity);
            yield return new WaitForSeconds(flashDelay);
        }
    }
}

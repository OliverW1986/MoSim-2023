using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private float volume;
    private void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume");
    }
}

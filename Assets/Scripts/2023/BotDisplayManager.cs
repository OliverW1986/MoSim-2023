using Unity.Cinemachine;
using UnityEngine;

public class BotDisplayManager : MonoBehaviour
{
    public Camera camera;


    void Start()
    {
        camera = GetComponent<Camera>();


        //Arrange the bots in a lovely little circle    
    }

    private void OnPostRender()
    {
        Debug.Log("Camera post-processing applied for camera: " + camera.name);
    }
}

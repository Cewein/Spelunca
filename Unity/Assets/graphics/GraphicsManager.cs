using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    [SerializeField] private Color fogColor;
    [SerializeField] private float startFogDistance;
    [SerializeField] private float endFogDistance;
    [SerializeField] private ChunkManager chunkManager;
    [SerializeField] private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        setFog();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        setFog();
    }
    
    void setFog(){
        float distance = chunkManager.chunkSize * chunkManager.viewRange/2;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogEndDistance = distance;
        RenderSettings.fogStartDistance = startFogDistance;
        mainCamera.backgroundColor = fogColor;}
}

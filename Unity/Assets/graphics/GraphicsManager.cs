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
        float distance = chunkManager.chunkSize * chunkManager.viewRange;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogEndDistance = endFogDistance;
        RenderSettings.fogStartDistance = startFogDistance;
        mainCamera.backgroundColor = fogColor;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

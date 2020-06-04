using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    [SerializeField] private Color fogColor =  new Color(0.326f, 0.435f, 0.528f);
    [SerializeField] private float startFogDistance = 50.0f;
    [SerializeField] private float endFogDistance = 70.0f;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogEndDistance = endFogDistance;
        RenderSettings.fogStartDistance = startFogDistance;
        Camera.main.backgroundColor = fogColor;
        Camera.main.depthTextureMode = Camera.main.depthTextureMode | DepthTextureMode.Depth;
    }
}

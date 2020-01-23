using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
    [SerializeField] private Color fogColor;
    [SerializeField] private float startFogDistance;
    [SerializeField] private float endFogDistance;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogEndDistance = endFogDistance;
        RenderSettings.fogStartDistance = startFogDistance;
        Camera.main.backgroundColor = fogColor;
    }
}

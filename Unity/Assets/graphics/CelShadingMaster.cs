using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CelShadingMaster : MonoBehaviour
{
    public Shader celShadingShader;
    
    [Range(0f,1f)] public float intensity = 1.0f;

    [Range(0f,1f)] public float filterCenter = 0.5f;

    [Range(0f,1f)] public float filterSmoothness = 0.5f;
    
    public float spacing = 0.1f;

    [Range(0f,1f)] public float lowPassFilter = 1.0f;

    public bool debugMode = false;

    private Material _mat;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(_mat == null)
        {
            _mat = new Material(celShadingShader);
            _mat.SetVector("iResolution", new Vector2(Screen.width, Screen.height));
            updateShader();
        }


        Graphics.Blit(source, destination, _mat);
    }

    private void OnValidate()
    {
        if(_mat != null)
        {
            updateShader();
        }
    }

    private void updateShader()
    {
        _mat.SetFloat("intensity", intensity);
        _mat.SetFloat("smoothness", filterSmoothness);
        _mat.SetFloat("center", filterCenter);
        _mat.SetFloat("spacing", spacing);
        _mat.SetInt("debugMode", debugMode ? 1:0);
        _mat.SetFloat("lowPassFilter", lowPassFilter);
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CelShadingMaster : MonoBehaviour
{
    public Shader celShadingShader;
    private Material _mat;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(_mat == null)
        {
            _mat = new Material(celShadingShader);
            _mat.SetVector("iResolution", new Vector2(Screen.width, Screen.height));
        }


        Graphics.Blit(source, destination, _mat);
    }
}
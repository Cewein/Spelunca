using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class ScannerMaster : MonoBehaviour
{
    public Shader scannerShader;
    public float width;
    public KeyCode key;
    public float speed;

    private Material _mat;
    [HideInInspector]
    public static float dist = -1.0f;
    private bool isMoving = false;


    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            isMoving = true;
            dist = -1.0f;
        }

        if (dist < 150.0 && isMoving)
            dist += speed;
        else
        {
            dist = -1.0f;
            isMoving = false;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(_mat == null)
        {
            _mat = new Material(scannerShader);
            _mat.SetVector("iResolution", new Vector2(Screen.width, Screen.height));
        }

        
        _mat.SetFloat("distanceFromCam", dist);
        _mat.SetFloat("width", width);
        Graphics.Blit(source, destination, _mat);
    }
}

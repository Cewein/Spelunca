using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uiPopUp : MonoBehaviour
{
    public Material popUpShader;

    private Renderer rend;
    // Update is called once per frame
    private void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = false;
    }
    void Update()
    {
        transform.LookAt(ChunkManager.playerPos);


        float dist = Vector3.Distance(transform.position, ChunkManager.playerPos.position);
        if(dist > ScannerMaster.dist && rend.enabled)
        {
            rend.enabled = false;
        }
        else if(dist < ScannerMaster.dist && !rend.enabled)
        {
            rend.enabled = true;
        }
    }
}

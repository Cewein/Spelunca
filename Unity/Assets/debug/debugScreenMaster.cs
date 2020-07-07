using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class debugScreenMaster : MonoBehaviour
{
    [Header("data ears")]
    public ChunkManager CKM;
    public Pool pl;

    [Header("text area")]
    public Text rigthArea;
    public Text leftArea;

    //data node
    private float  fps;
    private float  msBetweenFrame;
    private int    totalMB;
    private string procType;
    private string GraphName;
    private string GraphVersion;

    // Update is called once per frame

    private void Awake()
    {
        totalMB = SystemInfo.systemMemorySize;
        procType = SystemInfo.processorType;
        GraphName = SystemInfo.graphicsDeviceName;
        GraphVersion = SystemInfo.graphicsDeviceVersion;
    }

    void Update()
    {
        rigthArea.text = "Mem: " + (GC.GetTotalMemory(false) / 1024 / 1024).ToString() + " / " + SystemInfo.systemMemorySize + "MB\n\n"
            + SystemInfo.processorType + "\n\n"
            + "Display: " + Screen.currentResolution + "\n"
            + SystemInfo.graphicsDeviceName + "\n"
            + SystemInfo.graphicsDeviceVersion;

        leftArea.text = "Fps: " + (int)(1f / Time.smoothDeltaTime) + "\n"
            + "Rending: " + (int)(Time.deltaTime * 1000) + "ms\n\n"
            + "Pos: " + ChunkManager.playerPos.position + "\n"
            + "Chunk: " + CKM.playerChunk + "\n"
            + "Cached chunk: " + CKM.chunkDictionary.Count + "\n\n"
            + "Pool size: " + pl.poolSize;
    }
}

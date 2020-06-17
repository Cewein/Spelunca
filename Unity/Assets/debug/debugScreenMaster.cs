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

    private float[] fpsStat;
    private float[] renderStat;

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

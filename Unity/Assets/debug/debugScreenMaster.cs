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

    [Header("config")]
    public KeyCode display;

    private int[] fpsStat;
    private int[] renderStat;
    private bool showing;

    private void Awake()
    {
        fpsStat = new int[3];
        renderStat = new int[3];

        fpsStat[0] = int.MaxValue;
        fpsStat[2] = int.MinValue;

        renderStat[0] = int.MaxValue;
        renderStat[2] = int.MinValue;

        showing = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(display))
        {
            showing = !showing;
            if (showing)
            {
                Debug.Log("Show Debugger");
            }
            else
            {
                Debug.Log("Hide Debugger");
            }
        }
        
        if (showing)
        {

            int[] plSize = pl.GetSize();

            fpsStat[1] = (int)(1f / Time.smoothDeltaTime);
            renderStat[1] = (int)(Time.deltaTime * 1000);

            if (fpsStat[2] < fpsStat[1])
                fpsStat[2] = fpsStat[1];
            if (fpsStat[0] > fpsStat[1])
                fpsStat[0] = fpsStat[1];

            if (renderStat[2] < renderStat[1])
                renderStat[2] = renderStat[1];
            if (renderStat[0] > renderStat[1])
                renderStat[0] = renderStat[1];

            rigthArea.text = "Mem: " + (GC.GetTotalMemory(false) / 1024 / 1024).ToString() + " / " + SystemInfo.systemMemorySize + "MB\n\n"
                + SystemInfo.processorType + "\n\n"
                + "Display: " + Screen.currentResolution + "\n"
                + SystemInfo.graphicsDeviceName + "\n"
                + SystemInfo.graphicsDeviceVersion;

            leftArea.text = "Fps: \n\tcurrent: " + fpsStat[1] + " fps\n"
                + "\tmax: " + fpsStat[2] + " fps\n"
                + "\tmin: " + fpsStat[0] + " fps\n\n"
                + "Rending: \n\tcurrent: " + renderStat[1] + "ms\n"
                + "\tmax: " + renderStat[2] + "ms\n"
                + "\tmin: " + renderStat[0] + "ms\n\n"
                + "Pos: " + ChunkManager.playerPos.position + "\n"
                + "Chunk: " + CKM.playerChunk + "\n"
                + "Cached chunk: " + CKM.chunkDictionary.Count + "\n\n"
                + "Pool size: " + pl.poolSize + "\n"
                + "Attacking: " + plSize[0] + "\n"
                + "Disabled: " + plSize[1] + "\n";
        }
        else
        {
            rigthArea.text = "";
            leftArea.text = "";
        }
    }
}

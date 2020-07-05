using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OptionMangager : MonoBehaviour
{

    public OptionSlider[] worldgenSlider;

    // Start is called before the first frame update 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < worldgenSlider.Length; i++)
        {
            worldgenSlider[i].value.text = worldgenSlider[i].slider.value.ToString();
        }
    }

    public void setDefaultWorldGen()
    {
        for (int i = 0; i < worldgenSlider.Length; i++)
        {
            worldgenSlider[i].slider.value = worldgenSlider[i].defaultValue;
        }

        ChunkManager.useDefaultConfig = true;
    }

    public void setCustomWorldGen()
    {
        Directory.CreateDirectory("C:\\ProgramData\\spelunca\\");
        new XDocument(
            new XElement("config",
                new XAttribute("isoLevel", worldgenSlider[0].slider.value),
                new XAttribute("lacunarity", worldgenSlider[1].slider.value),
                new XAttribute("octave", worldgenSlider[2].slider.value),
                new XAttribute("persistence", worldgenSlider[3].slider.value),
                new XAttribute("precision", worldgenSlider[4].slider.value),
                new XAttribute("size", worldgenSlider[5].slider.value),
                new XAttribute("viewDistance", worldgenSlider[6].slider.value)
            )
        )
        .Save("C:\\ProgramData\\spelunca\\config.xml");
        ChunkManager.useDefaultConfig = false;
    }
}

[System.Serializable]
public struct OptionSlider
{
    public Text name;
    public Text value;
    public Slider slider;
    public float defaultValue;
}

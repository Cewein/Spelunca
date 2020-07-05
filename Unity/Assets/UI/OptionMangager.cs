using System.Collections;
using System.Collections.Generic;
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

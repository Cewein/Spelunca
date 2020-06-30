﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sound : MonoBehaviour
{
    [Header("sources")]
    public AudioSource uiClick;
    public AudioSource[] gameMusic;

    [Header("Config")]
    [Range(0, 1)]
    public float igVolume= 0.2f;

    private int selectedMusic = 0;


    public void playButtonClick()
    {
        uiClick.Play();
    }
    // Start is called before the first frame update
    void Start()
    {
        selectedMusic = Random.Range(0, gameMusic.Length);
        gameMusic[selectedMusic].volume = igVolume;
        gameMusic[selectedMusic].Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameMusic[selectedMusic].isPlaying)
        {
            int temp = selectedMusic;
            while(temp == selectedMusic)
            {
                temp = Random.Range(0, gameMusic.Length);
            }
            selectedMusic = temp;
            
            gameMusic[selectedMusic].Play();
        }
        gameMusic[selectedMusic].volume = igVolume;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class WavesUI : MonoBehaviourPunCallbacks
{
    private TMP_Text text;
    private GameObject waves;
    private void Start()
    {
        text = GetComponent<TMP_Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "WavesData")
        {
            if (GameObject.Find("WaveSpawner") != null)
            {
                waves = GameObject.Find("WaveSpawner");
                text.text = "WAVE: " + waves.GetComponent<WaveSpawner>().currentWave;
            }
        }

        if (gameObject.name == "EnnemiesData")
        {
            if (GameObject.Find("WaveSpawner") != null)
            {
                waves = GameObject.Find("WaveSpawner");
                text.text = "Enemies " + waves.GetComponent<WaveSpawner>().enemiesRemaining + " / " + waves.GetComponent<WaveSpawner>().totalEnemiesWave;
            }
        }

    }
}
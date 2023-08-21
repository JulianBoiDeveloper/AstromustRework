using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] AudioListener audioListener;
    float masterSound = 1f;

    [SerializeField] GameObject buttonMute;
    [SerializeField] GameObject buttonUnMute;

    [SerializeField] GameObject disableVFXButton;
    [SerializeField] GameObject enableVFXButton;

    ParticleSystem[] particleSystemsScene;

    [SerializeField] Slider masterVolumeBar;

    void Start()
    {
        particleSystemsScene = GameObject.FindObjectsOfType<ParticleSystem>();
    }

    public void Mute()
    {
        AudioListener.volume = 0f;
        buttonMute.SetActive(false);
        buttonUnMute.SetActive(true);
    }

    public void Unmute()
    {
        AudioListener.volume = 1f;
        buttonUnMute.SetActive(false);
        buttonMute.SetActive(true);
    }

    public void EnableVFX()
    {
        for(int i = 0; i < particleSystemsScene.Length; i++)
        {
            particleSystemsScene[i].gameObject.SetActive(true);
        }

        disableVFXButton.SetActive(true);
        enableVFXButton.SetActive(false);
    }

    public void DisableVFX()
    {
        for (int i = 0; i < particleSystemsScene.Length; i++)
        {
            particleSystemsScene[i].gameObject.SetActive(false);
        }

        disableVFXButton.SetActive(false);
        enableVFXButton.SetActive(true);
    }

    public void UpdateMasterVolume()
    {
        AudioListener.volume = masterVolumeBar.value;

        if (AudioListener.volume != 0f)
        {
            buttonMute.SetActive(true);
            buttonUnMute.SetActive(false);
        }
        else
        {
            buttonMute.SetActive(false);
            buttonUnMute.SetActive(true);
        }
    }
}

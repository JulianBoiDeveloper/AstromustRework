using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectToServer2 : MonoBehaviourPunCallbacks
{
    public string sceneName;
    
    public Image progressBar;

    public float duration = 0.5f; // Durée totale de la progression de la barre
    private float currentTime = 0f; // Temps actuel écoulé
    private bool isProgressing = false; // Indicateur si la progression est en cours
    
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (isProgressing)
        {
            currentTime += Time.deltaTime;

            // Mettez à jour la valeur du Slider en fonction du temps écoulé
            progressBar.fillAmount = currentTime / duration;

            if (currentTime >= duration)
            {
                isProgressing = false;
                currentTime = 0f;
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        isProgressing = true;
    }

    public override void OnJoinedLobby()
    {
        currentTime = duration;
        StartLevel();
    }

    public void StartLevel()
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

}

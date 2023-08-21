using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMap : MonoBehaviourPunCallbacks{

    
    private List<AsyncOperation> sceneToLoad = new List<AsyncOperation>();

    public Image loadingProgressBar;
    // Start is called before the first frame update

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon!");
        int playerCount = PhotonNetwork.CountOfPlayersOnMaster;
        
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Si aucune salle n'existe, en créer une
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom()
    {
       PhotonNetwork.LoadLevel("MainSceneTest");
       StartCoroutine(LoadingScreen());
    }


    IEnumerator LoadingScreen()
    {
        float totalprogress = 0;
        for (int i = 0; i < sceneToLoad.Count; i++)
        {
            while (!sceneToLoad[i].isDone)
            {
                totalprogress += sceneToLoad[i].progress;
                loadingProgressBar.fillAmount = totalprogress / sceneToLoad.Count;
                yield return null;
            }
        }
    }
}

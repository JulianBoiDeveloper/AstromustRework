using UnityEngine;
using Photon.Pun;

public class ConnectToServer : MonoBehaviour
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
}

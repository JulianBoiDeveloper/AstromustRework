using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkHandler : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
    public override void OnDisconnected(DisconnectCause cause)
    {
        // Handle disconnection event
        Debug.LogWarning("Disconnected from Photon server. Reason: " + cause.ToString());
        PhotonNetwork.LoadLevel("MenuMain");
        // You might want to handle reconnection logic here, show a UI, etc.
    }

}

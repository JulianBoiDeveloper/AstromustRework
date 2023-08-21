using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DisconnectToServer : MonoBehaviourPunCallbacks
{
    public static string deleteNameRoom = "";
    public static int nbrjoueur;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {   
        // Vérifier s'il reste une seule personne dans la salle
        if (nbrjoueur == 1)
        {
            Debug.Log("Last Player and Delete");
            
        }
    }
}


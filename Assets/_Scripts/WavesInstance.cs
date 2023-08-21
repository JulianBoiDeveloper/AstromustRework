using UnityEngine;
using Photon.Pun;

public class WavesInstance : MonoBehaviourPunCallbacks
{
    public GameObject wavesScript;

    public override void OnCreatedRoom()
    {
        if (GameObject.Find("WaveSpawner(Clone)") == null)
        {
            PhotonNetwork.InstantiateRoomObject(wavesScript.name, wavesScript.transform.position,
                wavesScript.transform.rotation);
        }
    }
}

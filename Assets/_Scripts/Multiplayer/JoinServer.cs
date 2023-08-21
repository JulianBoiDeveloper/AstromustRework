using Photon.Pun;
using TMPro;
using UnityEngine;
public class JoinServer : MonoBehaviourPunCallbacks
{
    public string text;
    [SerializeField] TMP_Text playersInfoText;

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(text);
    }

    public void SetPlayersInfo(string roomName, int players, int maxPlayers)
    {
        text = roomName;
        playersInfoText.text = roomName + ": " + players.ToString() + "/" + maxPlayers.ToString();
    }
}

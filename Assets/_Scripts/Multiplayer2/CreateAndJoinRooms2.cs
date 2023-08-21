using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateAndJoinRooms2 : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public Button btn;
    public string sceneName;
    public Toggle toggle;
    
    public GameObject loader;



    public void RoomCreate()
    {
        
        if (btn.interactable && createInput.text != "")
        {
            btn.interactable = false;
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.IsVisible = !toggle.isOn;
            PhotonNetwork.CreateRoom(createInput.text,roomOptions); 
            Debug.Log("Insert Rooms on BDD");
            loader.SetActive(true);
        }
        else
        {
            Debug.Log("Not Use");
        }
    }

    public void JoinRoom()
    {
        
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        DisconnectToServer.deleteNameRoom = PhotonNetwork.CurrentRoom.Name;
        DisconnectToServer.nbrjoueur = PhotonNetwork.CurrentRoom.PlayerCount;
        loader.SetActive(true);
         PhotonNetwork.LoadLevel(sceneName);
        Debug.Log("Update Rooms on BDD Joined Room");
    }
}
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomListCatchingExample : MonoBehaviourPunCallbacks
{
    private TypedLobby customLobby = new TypedLobby("customLobby", LobbyType.Default);

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    [SerializeField] GameObject roomListButton;
    [SerializeField] GameObject contentGO;
    [SerializeField] List<GameObject> roomListGO;

    private void Start()
    {
        roomListGO = new List<GameObject>();
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby(customLobby);
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
                for(int j = 0; j < roomListGO.Count; j++)
                {
                    if (roomListGO[j].GetComponent<JoinServer>().text == info.Name)
                    {
                        Destroy(roomListGO[j], 0.5f);
                        roomListGO.RemoveAt(j);
                    }
                }
                Debug.Log("Room removed: " + info.Name);
            }
            else
            {
            //    if(info.IsVisible) {
                    cachedRoomList[info.Name] = info;
               
                    roomListButton.GetComponentInChildren<TMP_Text>().text = roomList[i].Name +" : "+ roomList[i].PlayerCount + " / " + roomList[i].MaxPlayers;
                    GameObject go = GameObject.Instantiate(roomListButton, contentGO.transform);
                
                    roomListGO.Add(go);
                    go.GetComponent<JoinServer>().SetPlayersInfo(info.Name, info.PlayerCount, info.MaxPlayers);
                
                    Debug.Log("New room: " + info.Name);
            //    }
            }
        }
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
        
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
    }
}

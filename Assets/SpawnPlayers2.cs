using Photon.Pun;
using UnityEngine;

public class SpawnPlayers2 : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = PhotonNetwork.Instantiate(playerPrefab.name, transform.position, Quaternion.identity);
        go.GetComponent<ThirdPersonControllerV2>().defaultPos = transform.position;
    }
    
    

}

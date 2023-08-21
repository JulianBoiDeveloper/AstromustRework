
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefabs;
    public Transform[] Spawn;
    
    public int nbrSpawner;

    public void Start()
    {
        nbrSpawner = Random.Range(0, nbrSpawner);
        PhotonNetwork.Instantiate(playerPrefabs.name,
            new Vector3(Spawn[nbrSpawner].transform.position.x, Spawn[nbrSpawner].transform.position.y,3),
            Quaternion.identity);
    }

}

using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;
using Org.BouncyCastle.Asn1.Crmf;

[RequireComponent(typeof(AudioSource))]
public class WaveSpawner : MonoBehaviourPunCallbacks
{
    public GameObject enemyPrefab;
    public GameObject bigMonster;
    public GameObject wave5boss;


    public int enemiesPerWave = 5;
    public int currentWave = 1;
    public int enemiesRemaining;
    public int totalEnemiesWave = 0;

    public Transform[] spawnPoints;

    public Transform wave5BossSpawner;
    public Transform bigMonsterSpawner;

    public GameObject ambiantObjectSound;
    public GameObject startRoundObjectSound;
    public GameObject stopRoundObjectSound;
    public AudioClip startRound;
    public AudioClip stopRound;
    private AudioSource audioSourceAmbiant;
    private AudioSource audioSourceStartRound;
    private AudioSource audioSourceStopRound;

    public bool gameStarted = false;

    private void Start()
    {
        audioSourceStartRound = GetComponent<AudioSource>();
        audioSourceStopRound = GetComponent<AudioSource>();
        enemiesRemaining = enemiesPerWave;
        totalEnemiesWave = enemiesPerWave;
    //    if (PhotonNetwork.IsMasterClient)
    //        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;


        for (int i = 0; i < totalEnemiesWave; i++)
        {
            int rand = Random.Range(0, spawnPoints.Length);
            Vector3 randomSpawnPosition = spawnPoints[rand].position;
            PhotonNetwork.InstantiateRoomObject(enemyPrefab.name, randomSpawnPosition, Quaternion.identity);
        }
    }

    public void StartWave()
    {
        if(PhotonNetwork.IsMasterClient) {
            SpawnEnemies();
        }
        audioSourceStartRound.clip = startRound;
        audioSourceStartRound.PlayOneShot(startRound);
        photonView.RPC("StartGame", RpcTarget.All, true);
    }

    public void OnEnemyKilled()
    {
   //     if (!PhotonNetwork.IsMasterClient)
    //        return;
        photonView.RPC("IncreaseWave", RpcTarget.All);
    }

    public void SpawnWave5Boss()
    {
        PhotonNetwork.InstantiateRoomObject(wave5boss.name, wave5BossSpawner.position, Quaternion.identity);
    }

    public void SpawnWave30Boss()
    {
        PhotonNetwork.InstantiateRoomObject(bigMonster.name, bigMonsterSpawner.position, Quaternion.identity);
    }

    [PunRPC]
    private void StartGame(bool _gameStarted)
    {
        gameStarted = _gameStarted;
    }

    [PunRPC]
    private void IncreaseWave()
    {
        Debug.Log("IncreaseWave...");
        enemiesRemaining--;

        if(enemiesRemaining <= 0) {
            audioSourceStartRound.clip = startRound;
            audioSourceStopRound.PlayOneShot(stopRound);
            currentWave++;
            enemiesRemaining = enemiesPerWave + (currentWave * 2);
            totalEnemiesWave = enemiesRemaining;


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Spawn enemies");
                SpawnEnemies();
            }

            if (currentWave % 5 == 0)
            {
                enemiesRemaining++;
                totalEnemiesWave++;
            }
            if (currentWave == 30)
            {
                enemiesRemaining++;
                totalEnemiesWave++;
            }

            if (PhotonNetwork.IsMasterClient) {
                Debug.Log("Spawn enemies");
                if (currentWave % 5 == 0) {
                    SpawnWave5Boss();
                }
                if(currentWave == 30)
                {
                    SpawnWave30Boss();
                }
            }
        }
    }


    /*
    [Header("Player Data Spawner")]
    public Transform[] spawnPoints; // Les spawns
    public GameObject enemyPrefab; // Le monstre
    public int enemiesAlive;    // Nombre de monstre en vie

    [Header("Waves Data Spawner")] [Space(10)]
    public int currentWave = 0;
    public int ennemieWaveSpawner = 5;
    public float timeBetweenWave = 10f;
    private bool spawningWave = false;
    public int maxEnemies;
    public int monsterAddedPerRound = 4;
    public float ratioPerRound = 1f;

    [Header("Audio Sound")] 
    public GameObject ambiantObjectSound;
    public GameObject startRoundObjectSound;
    public GameObject stopRoundObjectSound;
    public AudioClip startRound;
    public AudioClip stopRound;
    private AudioSource audioSourceAmbiant;
    private AudioSource audioSourceStartRound;
    private AudioSource audioSourceStopRound;
    
    private GameObject[] spawnedPrefabs;
    
    // - Commencer la waves par l'Hote OK
    // - Permettre de voir les ennemies + la data Syncrhonisé  OK
    // - Permettre de voir tout ça fluidement   =>
    // - Déconnecté l'hote et voir si l'autre joueur vois et garde toute les données
    // - Voir si tout est syncro si l'hote d'avant reviens
    // - Ajout de point
    // - L'autre joueur ne vois pas ce que tu vois

    public void Start()
    {
        if (PhotonNetwork.MasterClient.IsLocal)
        {
            audioSourceAmbiant = ambiantObjectSound.GetComponent<AudioSource>();
            audioSourceStartRound = startRoundObjectSound.GetComponent<AudioSource>();
            audioSourceStopRound = stopRoundObjectSound.GetComponent<AudioSource>();
            photonView.RPC("RPCStartWaves", RpcTarget.All);
        }
    }


    private void Update()
    {
        if (PhotonNetwork.MasterClient.IsLocal)
        {
            ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "currentWave", currentWave },
                { "enemiesAlive", enemiesAlive },
                { "maxEnemies", maxEnemies }
            };

            Debug.Log(playerProperties);

            PhotonNetwork.MasterClient.SetCustomProperties(playerProperties);
            
            currentWave = (int)PhotonNetwork.MasterClient.CustomProperties["currentWave"];
            enemiesAlive = (int)PhotonNetwork.MasterClient.CustomProperties["enemiesAlive"];
            maxEnemies = (int)PhotonNetwork.MasterClient.CustomProperties["maxEnemies"];
        }
        Debug.Log((int)PhotonNetwork.MasterClient.CustomProperties["currentWave"] + ": " + (int)PhotonNetwork.MasterClient.CustomProperties["enemiesAlive"] + "/"+ (int)PhotonNetwork.MasterClient.CustomProperties["maxEnemies"]);
    }

    


    IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(timeBetweenWave);
        if (currentWave <= 1)
        {
            currentWave++;
        }
        maxEnemies = ennemieWaveSpawner + ((currentWave - 1) * (int)(monsterAddedPerRound*ratioPerRound));
        enemiesAlive = maxEnemies;
        
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnMonster();
            yield return new WaitForSeconds(0.1f); // Attendre entre deux spawn
        }
        Debug.Log("START ROUND");
        audioSourceStartRound.clip = startRound;
        audioSourceStartRound.PlayOneShot(startRound);
    }
    
    public void SpawnMonster()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)];
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.InstantiateRoomObject(enemyPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
    }

    [PunRPC]
    public void EnemyDied()
     {
         enemiesAlive--;

         if (enemiesAlive == 0 && currentWave != 0)
         {
             Debug.Log("END ROUND");
             audioSourceStartRound.clip = startRound;
             audioSourceStopRound.PlayOneShot(stopRound);
             currentWave++;
             StartCoroutine(SpawnWave());
         }
     }
    
    // Appelé lorsqu'un joueur quitte la salle (le jeu)
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Si le joueur qui quitte est le joueur local (vous)
        if (otherPlayer == PhotonNetwork.LocalPlayer)
        {
            // Détruire toutes les instances de préfabriqués créées par le joueur local
            foreach (GameObject prefabInstance in spawnedPrefabs)
            {
                if (prefabInstance != null)
                {
                    PhotonNetwork.Destroy(prefabInstance);
                }
            }
        }
    }
    [PunRPC]
    public void RPCStartWaves()
    {
        StartCoroutine(SpawnWave());
        audioSourceAmbiant.Play();
    }
    */
}

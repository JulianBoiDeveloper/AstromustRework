using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class ScoreUIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playersScoreText;


//    List<ThirdPersonControllerV2> playersConnected;


    private void Start()
    {
  //      playersConnected = new List<ThirdPersonControllerV2>();
    }

    private void Update()
    {
        playersScoreText.text = " ";
        foreach (KeyValuePair<int, Photon.Realtime.Player> kvp in PhotonNetwork.CurrentRoom.Players)
        {
            int key = kvp.Key;
            Photon.Realtime.Player value = kvp.Value;
            playersScoreText.text += value.NickName + key + "  -  " + value.GetScore() + "\n";
        }
    }

    private void OnEnable()
    {
        // Subscribe to the event when this script is enabled
    //    intVariable.OnValueChanged += UpdateText;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when this script is disabled
     //   intVariable.OnValueChanged -= UpdateText;
    }

    private void UpdateText(int newValue)
    {
        // Update the text component with the new value
        playersScoreText.text = newValue.ToString();
    }

    /*
    public override void OnPlayerEnteredRoom()
    {
        // Called when a new player enters the room
        if (PhotonNetwork.IsMasterClient)
        {
            // Perform your actions for the new player here
         //   Debug.Log("New player entered the room: " + newPlayer.NickName);
        }
    }*/

    /*
    private void OnEnable()
    {
        // Find all IntVariable objects in the scene and subscribe to their events
        IntVariable[] intVariables = FindObjectsOfType<IntVariable>();
        foreach (IntVariable intVariable in intVariables)
        {
            intVariable.OnValueChanged += UpdateText;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from all IntVariable events when disabled
        IntVariable[] intVariables = FindObjectsOfType<IntVariable>();
        foreach (IntVariable intVariable in intVariables)
        {
            intVariable.OnValueChanged -= UpdateText;
        }
    }
    */
}

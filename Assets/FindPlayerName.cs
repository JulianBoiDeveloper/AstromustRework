using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.SimpleGoogleSignIn;

public class FindPlayerName : MonoBehaviour
{
    [SerializeField] TMP_Text playerNameText;
    void Start()
    {
        if (GameObject.FindObjectOfType<Example>().userName != string.Empty)
            playerNameText.text = "Welcome " + GameObject.FindObjectOfType<Example>().userName;
        else
            playerNameText.text = "";
    }
}

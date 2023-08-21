using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TheGameManager : MonoBehaviour
{
    private static TheGameManager _instance;

    public static TheGameManager Instance { get { return _instance; } }


    public string playerName;
    [SerializeField] TMP_InputField inputField;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void UpdatePlayerName()
    {
        playerName = inputField.text;
    }
}
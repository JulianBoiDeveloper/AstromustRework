using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using System.IO;
using TMPro;

public class PlayGamesScript : MonoBehaviour
{
    public TMP_Text text;
    public string email ="example@test.fr";
    public bool isPlayStoreInAvailable = false;
    public bool isSaving = false;
    public string filePath = "Assets/Data/email.csv";
    
    void Start()
    {
        /*
        #if UNITY_ANDROID
            PlayGamesPlatform.Instance.Authenticate(delegate(SignInStatus status){ });
            Social.localUser.Authenticate((bool succes) =>
            {
                isPlayStoreInAvailable = succes;
                email = Social.localUser.userName;

            });    
        #endif*/
    }

    private void Update()
    {
        /*
        text.text = email; // NULL ERROR HERE
        if (isSaving)
        {

            if (File.Exists(filePath))
            {
                WriteDataToCsvFile(filePath, email);
            }
            else
            {
                CreateCsvFileIfNotExists(filePath);
            }
        }
        */
    } 
    
    void CreateCsvFileIfNotExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                WriteDataToCsvFile(filePath, email);
            }
        }
    }
    
     void WriteDataToCsvFile(string filePath, string data)
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(data);
        }
    }
    
}
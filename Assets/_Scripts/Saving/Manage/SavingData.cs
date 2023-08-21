using System.Collections.Generic;
using UnityEngine;
using Unity.Services.CloudSave;
using System;
using System.IO;
using Unity.Services.Authentication;

public class SavingData : MonoBehaviour
{
    
    private const string jsonFilePath = "players.json";
    public string data;
    public DataGeneralSaving obj;
    public bool saveData = false;
    

    private void Update()
    {
        if (saveData)
        {
            DataUpdate();
        }
    }
    private async void DataUpdate()
    {
        data =  JsonUtility.ToJson(obj);
        var datas = new Dictionary<string, object>
        {
            {"all", data}
        };
        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(datas);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        saveData = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using TMPro;
using Samples.Purchasing.Core.BuyingConsumables;
using System.Threading.Tasks;
using System;

public class CloudSave : MonoBehaviour
{
    private static CloudSave _instance;
    public static CloudSave Instance { get { return _instance; } }

    public string text;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject); // Add this line
        }

        GetMaxWave();
    }

    int level = 0;
    public void IncreaseLevel()
    {
        level++;
        SaveEntry("level", level.ToString());
    //    text += "\nIncrease level: " + level;
    }

    public async void GetLevel()
    {
        Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "level" });
        text += "\nLevel cloud: " + savedData["level"].ToString();
        level = int.Parse(savedData["level"].ToString());
    }
    async public void SaveTest()
    {
        var entry = new Dictionary<string, object> { { "MyKey", "Data saved" } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(entry);
    }
    async public void SaveEntry(string key, string data)
    {
        var entry = new Dictionary<string, object> { { key, (object)data } };
        await CloudSaveService.Instance.Data.ForceSaveAsync(entry);
    }

    public int maxWave;

    public async void GetMaxWave()
    {
        Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "MaxWave" });
        maxWave = int.Parse(savedData["MaxWave"]);
    }

    public async void GetIAPs(BuyingConsumables bc)
    {
        try
        {
            // Load cloud data asynchronously
            Task<Dictionary<string, string>> unlockGameTask = CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "iap_unlockedGame" });
            Task<Dictionary<string, string>> powerup1Task = CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "iap_boost1" });
            Task<Dictionary<string, string>> powerup2Task = CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { "iap_boost2" });

            // Wait for all tasks to complete
            await Task.WhenAll(unlockGameTask, powerup1Task, powerup2Task);

            // Process the loaded data
            IAPsTracker._instance.unlockGame = bool.Parse(unlockGameTask.Result["iap_unlockedGame"]);
            IAPsTracker._instance.boost1amount = int.Parse(powerup1Task.Result["iap_boost1"]);
            IAPsTracker._instance.boost2amount = int.Parse(powerup2Task.Result["iap_boost2"]);

            // Update the UI
            bc.UpdateUI();
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred during IAP data retrieval: " + ex.Message);
        }
    }

    public async void RetrieveKeys()
    {
        List<string> keys = await CloudSaveService.Instance.Data.RetrieveAllKeysAsync();

        for (int i = 0; i < keys.Count; i++)
        {
            Debug.Log(keys[i]);
            text += "\nKEY["+i+"]: " + keys[i];
        }
    }
}

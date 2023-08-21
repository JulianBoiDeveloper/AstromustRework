using System.Collections.Generic;
using Unity.Services.CloudSave;
using UnityEngine;

public class LoadingData : MonoBehaviour
{
   public bool loadData = false; 
   public string loadedData;
   private async void Update()
   {
      if (loadData)
      {
         Dictionary<string, string> savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"all"});
         loadedData = savedData["all"];
         Debug.Log(loadedData);
         loadData = false;
      }
      
   }
   
}

using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
[RequireComponent(typeof(AuthentificationCloudSave))]
public class CloudSaveInit : MonoBehaviour
{
    private string playerId;
    public string environment = "production";

    async void Start()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environment);
            await UnityServices.InitializeAsync(options);
            
            
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
    }
  
}



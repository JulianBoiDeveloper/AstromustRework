using Unity.Services.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using UnityEngine;

[RequireComponent(typeof(DataGeneralSaving))]
public class AuthentificationCloudSave : MonoBehaviour
{

    private List<string> keys;
    internal async Task Awake()
    {
        await UnityServices.InitializeAsync();
        await SignInAnonymously();
        keys = await CloudSaveService.Instance.Data.RetrieveAllKeysAsync();
    }
    
    private async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            var playerId = AuthenticationService.Instance.PlayerId;

            Debug.Log("Signed in as: " + playerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}


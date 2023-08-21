using Unity.Services.Core;
using UnityEngine;
using System;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using TMPro;
using GooglePlayGames;
using UnityEngine.SceneManagement;

public class UGSInitialization : MonoBehaviour
{

    async void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        SetupEvents();
        //     SignInAnonymously();
    }

    [SerializeField] TMP_Text status;

    async void Init()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        SetupEvents();
    }

    // Setup authentication event handlers if desired
    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () => {
            // Shows how to get a playerID
   //         Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
    //        Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
     //       status.text = "Player signed in";
        };

        AuthenticationService.Instance.SignInFailed += (err) => {
  //          Debug.LogError(err);
  //          status.text = "Error: " + err;
        };

        AuthenticationService.Instance.SignedOut += () => {
 //           Debug.Log("Player signed out.");
   //         status.text = "Player signed out";
        };

        AuthenticationService.Instance.Expired += () =>
        {
    //        Debug.Log("Player session could not be refreshed and expired.");
     //       status.text = "Player session could not be refreshed and expired.";

        };
    }
    async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            LoadMenu();
   //         Debug.Log("Sign in anonymously succeeded!");
   // Shows how to get the playerID
   //           Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
   //          Assets.SimpleGoogleSignIn.Example.mydebug += "\nSign in anonymoysly success";
        }
        catch (AuthenticationException ex) 
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
//            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
   //         Debug.LogException(ex);
        }
    }
    public void SignInAnonymously()
    {
    //    Init();
        SignInAnonymouslyAsync();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MenuMain");
    }
}
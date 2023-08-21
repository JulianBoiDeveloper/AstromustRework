using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;
using System.Threading.Tasks;
using UnityEngine.SocialPlatforms;

public class GPGSManager : MonoBehaviour
{
    public string Token;
    public string Error;

    void Awake()
    {
  //      PlayGamesPlatform.Activate();
    }

    /*
    async void Start()
    {
        await UnityServices.InitializeAsync();
        await LoginGooglePlayGames();
        await SignInWithGooglePlayGamesAsync(Token);
    }*/

    public async void InitGoogle()
    {
        await UnityServices.InitializeAsync();
        await LoginGooglePlayGames();
        await SignInWithGooglePlayGamesAsync(Token);
    }

    public void SignOut()
    {
      //  PlayGamesPlatform.Instance.localUser.sign
    }




    //Fetch the Token / Auth code
    public Task LoginGooglePlayGames()
    {
        return null;
        /*
        var tcs = new TaskCompletionSource<object>();
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success.Equals(SignInStatus.Success))
            {
                Debug.Log("Login with Google Play games successful.");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    Debug.Log("Authorization code: " + code);
                    Token = code;
                    // This token serves as an example to be used for SignInWithGooglePlayGames
                    tcs.SetResult(null);
                });
            }
            else
            {
                Error = "Failed to retrieve Google play games authorization code";
                Debug.Log("Login Unsuccessful");
                tcs.SetException(new Exception("Failed"));
            }
        });
        return tcs.Task;
        */
    }


    async Task SignInWithGooglePlayGamesAsync(string authCode)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); //Display the Unity Authentication PlayerID
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    async public void LinkGoogle(string token)
    {
        await AuthenticationService.Instance.LinkWithGooglePlayGamesAsync(token);
    }

}
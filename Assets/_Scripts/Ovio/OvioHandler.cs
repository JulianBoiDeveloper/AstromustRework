using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OvioHandler : MonoBehaviour
{

    // oviogg://astromust/userName/<game ID>/<user ID>  <---- To OviO
    // astromust://dlapi/ovio/<userID>/<wallet address>  <---- To Astromust.
    // Main API key is: X0Y4XkSp6Ou7lOuvnOdW
    // Dev API key is: YdJ6tFGNelXycwcP6DCb
    public string OvioAPIKey = "";

    public bool isOvioLinked {get; private set;}
    public string currentUserID {get; private set;}
    public string currentOvioAddress {get; private set;}

    private string OVIO_ENDPOINT = "https://prod.ovio.gg/api/gameS2SEvent/customWeb3Event";

    void Awake() {

        if(PlayerPrefs.HasKey("ovio_userId") && PlayerPrefs.HasKey("ovio_walletAddress")) {
            currentUserID = PlayerPrefs.GetString("ovio_userId");
            currentOvioAddress = PlayerPrefs.GetString("ovio_walletAddress");
            isOvioLinked = true;
        }
    }

    public void DeleteOvioLink() {
        PlayerPrefs.DeleteKey("ovio_userId");
        PlayerPrefs.DeleteKey("ovio_walletAddress");
        isOvioLinked = false;
    }

    public void InitializeOvio(string userId, string walletAddress) {
        currentOvioAddress = walletAddress;
        currentUserID = userId;
        PlayerPrefs.SetString("ovio_userId", currentUserID);
        PlayerPrefs.SetString("ovio_walletAddress", currentOvioAddress);
        if(!isOvioLinked) {
            isOvioLinked = true;
        }else {
            Debug.LogWarning("Re-Initializing OviO settings!");
        }
    }

    public void SendOvioGetEvent(Action<string> responseCallback) {
        if(!isOvioLinked) {
            Debug.LogError("Can't send Ovio event before calling `InitializeOvio()`!");
            return;
        }
        StartCoroutine(OvioRequestHandler(responseCallback));
    }

    IEnumerator OvioRequestHandler(Action<string> responseCallback) {

        string fullUrl = $"{OVIO_ENDPOINT}/{OvioAPIKey}?userId={currentUserID}&walletAddress={currentOvioAddress}";
        using(UnityWebRequest webRequest = UnityWebRequest.Get(fullUrl)) {
            Debug.Log($"Sending Ovio Request to: '{fullUrl}'");
            yield return webRequest.SendWebRequest();

            switch(webRequest.result) {
                case UnityWebRequest.Result.ConnectionError: {
                    Debug.LogError($"Connection Error: {webRequest.error}");
                    break;
                }
                case UnityWebRequest.Result.ProtocolError: {
                    Debug.LogError($"Protocol Error: {webRequest.error}");
                    break;
                }
                case UnityWebRequest.Result.Success: {
                    responseCallback(webRequest.downloadHandler.text);
                    break;
                }
                default: {
                    Debug.LogWarning($"Unknown response: {webRequest.downloadHandler.text}");
                    responseCallback(webRequest.downloadHandler.text);
                    break;
                }
            }
        }
    }
}

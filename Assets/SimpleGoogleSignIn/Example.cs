using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleGoogleSignIn.Scripts;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SceneManagement;

namespace Assets.SimpleGoogleSignIn
{
    public class Example : MonoBehaviour
    {
        public Text Log;
        public Text Output;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject); // Add this line to ensure the GameObject isn't destroyed on scene load.
        }

        public void Start()
        {
        //    Application.logMessageReceived += (condition, stacktrace, type) => Log.text += condition + "\n";
            GoogleAuth.OnTokenResponse += OnTokenResponse; // Optional. Subscribe to get an access token.

            if (GoogleAuth.SavedAuth != null)
            {
                SignIn();
            }
        }

        public void SignIn()
        {
            if (GoogleAuth.SavedAuth != null)
                GoogleAuth.RefreshToken();

            GoogleAuth.SignIn(OnSignIn);
            SceneManager.LoadScene("MenuMain");
        }

        public void SignOut()
        {
            GoogleAuth.SignOut(revokeAccessToken: true);
            Output.text = "Not signed in";
        }

        public static string idToken = "none";
        public UserInfo info;
        public string userName;

        private async void OnSignIn(bool success, string error, UserInfo userInfo)
        {
            Output.text = success ? $"Hello, {userInfo.name}!" : error;
            userName = userInfo.name;
            info = userInfo;
            if (success)
            {
                await SignInWithGoogleAsync(idToken);
            }
        }

        async Task LinkWithGoogleAsync(string idToken)
        {
            mydebug += "\nInside linking";
            try
            {
                mydebug += "\nLink is successful";
                await AuthenticationService.Instance.LinkWithGoogleAsync(idToken);
                Debug.Log("Link is successful.");
            }
            catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            {
                // Prompt the player with an error message.
                Debug.LogError("This user is already linked with another account. Log in instead.");
                mydebug += "\nThis user is already linked with another account.Log in instead";
            }

            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
                mydebug += "\nError: " + ex;

            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
                mydebug += "\nError: " + ex;
            }
            mydebug += "\nend linking";
        }

        async Task SignInWithGoogleAsync(string idToken)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithGoogleAsync(idToken);
                SceneManager.LoadScene("MenuMain");
     //           Debug.Log("SignIn is successful.");
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

        private async void OnTokenResponse(TokenResponse response)
        {
   //         Debug.Log($"Access token: {response.access_token}");
            idToken = response.id_token;
  //          mydebug += "\nid token: " + idToken;
  //          mydebug += "\naccess token: " + response.access_token;


            var jwt = new JWT(response.id_token);

 //           Debug.Log($"JSON Web Token (JWT) Payload: {jwt.Payload}");

            //jwt.ValidateSignature((success, error) => Debug.Log(success ? "JWT signature validated." : error));
        }

        public void SaveUserInfoCloud()
        {
            GoogleAuth.GetUserInfoCloudSave();
        }

        public void Navigate(string url)
        {
            Application.OpenURL(url);
        }

        public static string mydebug = "";
        private Vector2 scrollPosition = Vector2.zero;
        private GUIStyle greenTextStyle;

        /*
        private void OnGUI()
        {
            // Set up GUI layout
            GUILayout.BeginArea(new Rect(10, 10, 500, 800));
            GUILayout.Box("Debug Info");
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));
            // Create custom GUIStyle with green color and larger font size
            greenTextStyle = new GUIStyle(GUI.skin.label);
            greenTextStyle.normal.textColor = Color.green;
            greenTextStyle.fontSize = 16;
            GUILayout.Label(mydebug);

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }*/
    }
}
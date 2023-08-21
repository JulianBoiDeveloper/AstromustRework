using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Services.Authentication;


#if UNITY_WEBGL

using System.Collections;

#endif

namespace Assets.SimpleGoogleSignIn.Scripts
{
    /// <summary>
    /// API specification: https://developers.google.com/identity/protocols/oauth2/native-app
    /// </summary>
    public class GoogleAuth : MonoBehaviour
    {
        public static event Action<TokenResponse> OnTokenResponse = response => { };

        public static TokenResponse TokenResponse { get; private set; }

        public static SavedAuth SavedAuth
        {
            get { if (PlayerPrefs.HasKey(PlayerPrefsKey)) try { return JsonUtility.FromJson<SavedAuth>(PlayerPrefs.GetString(PlayerPrefsKey)); } catch { return null; } return null; }
            private set => PlayerPrefs.SetString(PlayerPrefsKey, JsonUtility.ToJson(value));
        }

        private const string PlayerPrefsKey = "GoogleAuth.SavedAuth";

        /// <summary>
        /// OpenID configuration: https://accounts.google.com/.well-known/openid-configuration
        /// </summary>
        private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
        private const string UserInfoEndpoint = "https://openidconnect.googleapis.com/v1/userinfo";
        private const string RevocationEndpoint = "https://oauth2.googleapis.com/revoke";
        private const string AccessScope = "openid email profile";

        private static string _redirectUri, _state, _codeVerifier;
        private static Action<bool, string, UserInfo> _callback;
        
        public static void SignIn(Action<bool, string, UserInfo> callback)
        {
            _callback = callback;

            #if UNITY_EDITOR || UNITY_STANDALONE

            _redirectUri = $"http://localhost:{Utils.GetRandomUnusedPort()}/";
            
            #elif UNITY_WSA || UNITY_ANDROID || UNITY_IOS

            _redirectUri = $"{Settings.Instance.CustomUriScheme}:/oauth2callback";

            #elif UNITY_WEBGL

            _redirectUri = "";

            #endif

            if (SavedAuth == null)
            {
                Auth();
            }
            else
            {
                UseSavedToken();
            }
        }

        public static void SignOut(bool revokeAccessToken = false)
        {
            if (revokeAccessToken && SavedAuth != null)
            {
                RevokeAccessToken(SavedAuth.TokenResponse.access_token);
            }

            TokenResponse = null;
            PlayerPrefs.DeleteKey(PlayerPrefsKey);
            AuthenticationService.Instance.SignOut();
        }

#if UNITY_EDITOR || UNITY_STANDALONE

        private static void Listen()
        {
            var httpListener = new System.Net.HttpListener();

            httpListener.Prefixes.Add(_redirectUri);
            httpListener.Start();

            var context = System.Threading.SynchronizationContext.Current;
            var asyncResult = httpListener.BeginGetContext(result => context.Send(HandleHttpListenerCallback, result), httpListener);

            // Block the thread when background mode is not supported to serve HTTP response while the application is not in focus.
            if (!Application.runInBackground) asyncResult.AsyncWaitHandle.WaitOne();
        }

        private static void HandleHttpListenerCallback(object state)
        {
            var result = (IAsyncResult) state;
            var httpListener = (System.Net.HttpListener) result.AsyncState;
            var context = httpListener.EndGetContext(result);

            // Send an HTTP response to the browser to notify the user to close the browser.
            var response = context.Response;
            var buffer = Encoding.UTF8.GetBytes(Resources.Load<TextAsset>("StandaloneTemplate").text.Replace("{0}", Application.productName));

            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "text/html; charset=utf-8";
            response.ContentLength64 = buffer.Length;

            var output = response.OutputStream;

            output.Write(buffer, 0, buffer.Length);
            output.Close();
            httpListener.Close();

            HandleAuthResponse(context.Request.QueryString);
        }

        private static void HandleAuthResponse(NameValueCollection parameters)
        {
            var error = parameters.Get("error");

            if (error != null)
            {
                _callback?.Invoke(false, error, null);
                return;
            }

            var state = parameters.Get("state");
            var code = parameters.Get("code");
            var scope = parameters.Get("scope");

            if (state == null || code == null || scope == null) return;

            if (state == _state)
            {
                PerformCodeExchange(code);
            }
            else
            {
                Log("Unexpected response.");
            }
        }

        #elif UNITY_WSA || UNITY_ANDROID || UNITY_IOS

        static GoogleAuth()
        {
            Application.deepLinkActivated += OnDeepLinkActivated;
        }

        private static void OnDeepLinkActivated(string deepLink)
        {
            Log($"Deep link activated: {deepLink}");

            var parameters = Utils.ParseQueryString(deepLink);
            var error = parameters.Get("error");

            if (error != null)
            {
                _callback?.Invoke(false, error, null);
                return;
            }

            var state = parameters.Get("state");
            var code = parameters.Get("code");
            
            if (state == null || code == null) return;

            if (state == _state)
            {
                PerformCodeExchange(code);
            }
            else
            {
                Log("Unexpected response.");
            }
        }

        #endif

        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WSA || UNITY_ANDROID || UNITY_IOS

        private static void Auth()
        {
            _state = Guid.NewGuid().ToString();
            _codeVerifier = Guid.NewGuid().ToString();

            var codeChallenge = Utils.CreateCodeChallenge(_codeVerifier);
            var authorizationRequest = $"{AuthorizationEndpoint}?response_type=code&scope={Uri.EscapeDataString(AccessScope)}&redirect_uri={Uri.EscapeDataString(_redirectUri)}&client_id={Settings.Instance.ClientId}&state={_state}&code_challenge={codeChallenge}&code_challenge_method=S256";

            Log($"Authorization: {authorizationRequest}");
            Application.OpenURL(authorizationRequest);

            #if UNITY_EDITOR || UNITY_STANDALONE

            Listen();

            #endif
        }

        #elif UNITY_WEBGL

        private const string AuthorizationMiddleware = "https://hippogames.dev/api/oauth";
        private static GoogleAuth _instance;

        private static void Auth()
        {
            _state = Guid.NewGuid().ToString();
            _codeVerifier = Guid.NewGuid().ToString();
            
            var codeChallenge = Utils.CreateCodeChallenge(_codeVerifier);
            var request = UnityWebRequest.Post(AuthorizationMiddleware + "/init", new Dictionary<string, string> { { "state", _state }, { "redirectUri", _redirectUri }, { "clientName", Application.productName } });

            Log($"Initializing auth middleware: {request.url}");

            request.SendWebRequest().completed += obj =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var authorizationRequest = $"{AuthorizationEndpoint}?client_id={Settings.Instance.ClientId}&scope={Uri.EscapeDataString(AccessScope)}&response_type=code&redirect_uri={Uri.EscapeDataString(AuthorizationMiddleware + "/redirect")}&state={_state}&code_challenge={codeChallenge}&code_challenge_method=S256";

                    Log($"Authorization: {authorizationRequest}");
                    Application.OpenURL(authorizationRequest);
                    GetAuthCode();
                }
                else
                {
                    _callback(false, $"{request.error}: {request.downloadHandler.text}", null);
                }

                request.Dispose();
            };
        }

        private static void GetAuthCode()
        {
            if (_instance == null)
            {
                _instance = new GameObject(typeof(GoogleAuth).Name).AddComponent<GoogleAuth>();
                DontDestroyOnLoad(_instance);
            }

            _instance.StopAllCoroutines();
            _instance.StartCoroutine(GetCodeLoopback());
        }

        private static IEnumerator GetCodeLoopback()
        {
            yield return new WaitForSeconds(1);

            for (var i = 0; i < 10; i++)
            {
                using var request = UnityWebRequest.Post(AuthorizationMiddleware + "/getcode", new Dictionary<string, string> { { "state", _state } });

                Log($"Obtaining auth code: {request.url}");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var code = request.downloadHandler.text;

                    Log($"code={code}");

                    PerformCodeExchange(code);

                    yield break;
                }
                
                if (request.responseCode == 704)
                {
                    yield return new WaitForSeconds(5);
                }
                else
                {
                    _callback(false, $"{request.error}: {request.downloadHandler.text}", null);
                }
            }
        }

        #endif

        private static void UseSavedToken()
        {
            if (SavedAuth == null) throw new Exception("Initial authorization is required.");

            if (SavedAuth.ClientId != Settings.Instance.ClientId)
            {
                SignOut();
                SignIn(_callback);
            }
            else if (SavedAuth.ExpirationTime > DateTime.UtcNow)
            {
                Debug.Log("Using saved access token...");
                RequestUserInfo(SavedAuth.TokenResponse.access_token, (success, error, userInfo) =>
                {
                    if (success)
                    {
                        _callback(true, null, userInfo);
                    }
                    else
                    {
                        SignOut();
                        SignIn(_callback);
                    }
                });
            }
            else
            {
                Debug.Log("Refreshing expired access token...");
                RefreshAccessToken((success, error, tokenResponse) =>
                {
                    if (success)
                    {
                        RequestUserInfo(SavedAuth.TokenResponse.access_token, _callback);
                    }
                    else
                    {
                        SignOut();
                        SignIn(_callback);
                    }
                });
            }
        }

        public static void GetUserInfoCloudSave()
        {
            RequestUserInfo(SavedAuth.TokenResponse.access_token, (success, error, userInfo) =>
            {
                if (success)
                {
                    CloudSave.Instance.SaveEntry("Email", userInfo.email);
                    CloudSave.Instance.SaveEntry("Family_Name", userInfo.family_name);
                    CloudSave.Instance.SaveEntry("Given_Name", userInfo.given_name);
                    CloudSave.Instance.SaveEntry("Name", userInfo.name);
                    CloudSave.Instance.SaveEntry("Language", userInfo.locale);
                    CloudSave.Instance.SaveEntry("Picture", userInfo.picture);
                }
            });
        }

        public static void RefreshToken()
        {
            RefreshAccessToken((success, error, tokenResponse) =>
            {
                if (success)
                {
                    RequestUserInfo(SavedAuth.TokenResponse.access_token, _callback);
                }
                else
                {
                    SignOut();
                    SignIn(_callback);
                }
            });
        }

        private static void PerformCodeExchange(string code)
        {
            var request = UnityWebRequest.Post(TokenEndpoint, new Dictionary<string, string>
            {
                { "code", code },
                #if UNITY_WEBGL && !UNITY_EDITOR
                { "redirect_uri", AuthorizationMiddleware + "/redirect" },
                #else
                { "redirect_uri", _redirectUri },
                #endif
                { "client_id", Settings.Instance.ClientId },
                #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
                { "client_secret", Settings.Instance.ClientSecret },
                #endif
                { "code_verifier", _codeVerifier },
                { "scope", AccessScope },
                { "grant_type", "authorization_code" }
            });

            Log($"Exchanging code for access token: {request.url}");

            request.SendWebRequest().completed += _ =>
            {
                if (request.error == null)
                {
      //              Log($"TokenExchangeResponse={request.downloadHandler.text}");

                    TokenResponse = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);
                    SavedAuth = new SavedAuth(Settings.Instance.ClientId, TokenResponse);
                    OnTokenResponse(TokenResponse);
                    RequestUserInfo(TokenResponse.access_token, _callback);
                }
                else
                {
                    _callback(false, $"{request.error}: {request.downloadHandler.text}", null);
                }
            };
        }

        /// <summary>
        /// You can move this function to your backend for more security.
        /// </summary>
        public static void RequestUserInfo(string accessToken, Action<bool, string, UserInfo> callback)
        {
            var request = UnityWebRequest.Get(UserInfoEndpoint);

            Log($"Requesting user info: {request.url}");

            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            request.SendWebRequest().completed += _ =>
            {
                if (request.error == null)
                {
         //           Log($"UserInfo={request.downloadHandler.text}");

                    var userInfo = JsonUtility.FromJson<UserInfo>(request.downloadHandler.text);

                    SavedAuth.UserInfo = userInfo;
                    callback(true, null, userInfo);
                }
                else
                {
                    callback(false, $"{request.error}: {request.downloadHandler.text}", null);
                }
            };
        }

        /// <summary>
        /// https://developers.google.com/identity/protocols/oauth2/native-app#offline
        /// </summary>
        public static void RefreshAccessToken(Action<bool, string, TokenResponse> callback)
        {
            if (SavedAuth == null) throw new Exception("Initial authorization is required.");

            var refreshToken = SavedAuth.TokenResponse.refresh_token;
            var request = UnityWebRequest.Post(TokenEndpoint, new Dictionary<string, string>
            {
                { "client_id", Settings.Instance.ClientId },
                #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
                { "client_secret", Settings.Instance.ClientSecret },
                #endif
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            });

            Log($"Access token refresh: {request.url}");

            request.SendWebRequest().completed += obj =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                //    Log($"TokenExchangeResponse={request.downloadHandler.text}");

                    TokenResponse = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);
                    TokenResponse.refresh_token = refreshToken;
                    SavedAuth = new SavedAuth(Settings.Instance.ClientId, TokenResponse);
                    OnTokenResponse(TokenResponse);
                    callback(true, null, TokenResponse);
                }
                else
                {
                    callback(false, $"{request.error}: {request.downloadHandler.text}", null);
                }

                request.Dispose();
            };
        }

        private static void RevokeAccessToken(string accessToken)
        {
            if (SavedAuth == null) return;

            var request = UnityWebRequest.Post($"{RevocationEndpoint}?token={accessToken}", "");

            Log($"Revoking access token: {request.url}");

            request.SendWebRequest().completed += _ => Log(request.error ?? "Access token revoked!");
        }

        private static void Log(string message)
        {
       //     Debug.Log(message); // TODO: Remove in Release.
        }
    }
}
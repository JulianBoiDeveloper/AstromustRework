using UnityEngine;

namespace Assets.SimpleGoogleSignIn.Scripts
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Simple Google Sign-In/Settings")]
    public class Settings : ScriptableObject
    {
        [Header("Android / iOS / Universal Windows Platform")]
        [SerializeField] private string ClientIdMobile;
        [SerializeField] private string CustomUriSchemeMobile;

        [Header("Windows / macOS / Editor")]
        [SerializeField] private string ClientIdStandalone;
        [SerializeField] private string ClientSecretStandalone;

        [Header("WebGL")]
        [SerializeField] private string ClientIdWebGL;
        [SerializeField] private string ClientSecretWebGL;

        #if UNITY_EDITOR || UNITY_STANDALONE

        public string ClientId => ClientIdStandalone;
        public string ClientSecret => ClientSecretStandalone;

        #elif UNITY_WEBGL

        public string ClientId => ClientIdWebGL;
        public string ClientSecret => ClientSecretWebGL;

        #elif UNITY_WSA || UNITY_ANDROID || UNITY_IOS

        public string ClientId => ClientIdMobile;
        public string CustomUriScheme => CustomUriSchemeMobile;

        #endif

        private static Settings _instance;

        public static Settings Instance => _instance ??= Resources.Load<Settings>("Settings");

        public bool Redefined()
        {
            return ClientIdMobile != "275731233438-nv59vdj6ornprhtppnm8qle2jt3ebjol.apps.googleusercontent.com"
                   && CustomUriSchemeMobile != "google.auth"
                   && ClientIdStandalone != "275731233438-v1anl61611mmer6ohqes9a310mkc2di8.apps.googleusercontent.com"
                   && ClientSecretStandalone != "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"
                   && ClientIdWebGL != "275731233438-3hvi982vbhpcpkjihioa4d3gum074lds.apps.googleusercontent.com"
                   && ClientSecretWebGL != "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";
        }
    }
}
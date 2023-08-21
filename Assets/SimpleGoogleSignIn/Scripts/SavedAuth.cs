using System;
using Newtonsoft.Json.Linq;

namespace Assets.SimpleGoogleSignIn.Scripts
{
    [Serializable]
    public class SavedAuth
    {
        public string ClientId;
        public TokenResponse TokenResponse;
        public long Expiration;
        public UserInfo UserInfo;

        public DateTime ExpirationTime => DateTimeOffset.FromUnixTimeSeconds(Expiration).UtcDateTime;

        public SavedAuth(string clientId, TokenResponse tokenResponse)
        {
            ClientId = clientId;
            TokenResponse = tokenResponse;

            if (string.IsNullOrEmpty(TokenResponse.id_token))
            {
                Expiration = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 10;
            }
            else
            {
                var jwt = new JWT(TokenResponse.id_token);
                var payload = JObject.Parse(jwt.Payload);

                Expiration = (int) payload["exp"] - 10;
            }
        }
    }
}
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Assets.SimpleGoogleSignIn.Scripts
{
    /// <summary>
    /// JWT debugger: https://jwt.io/
    /// </summary>
    public class JWT
    {
        public readonly string Encoded;

        public string Header => Base64UrlEncoder.Decode(Encoded.Split('.')[0]);
        public string Payload => Base64UrlEncoder.Decode(Encoded.Split('.')[1]);
        public string SignedData => Encoded.Split('.')[0] + "." + Encoded.Split('.')[1];
        public string Signature => Encoded.Split('.')[2];

        private const string JwksUri = "https://www.googleapis.com/oauth2/v3/certs";

        public JWT(string encoded)
        {
            Encoded = encoded;
        }

        /// <summary>
        /// More info: https://developers.google.com/identity/openid-connect/openid-connect#validatinganidtoken
        /// Signature validation makes sense on a backend only in most cases.
        /// </summary>
        public void ValidateSignature(Action<bool, string> callback)
        {
            var header = JObject.Parse(Header);

            if ((string) header["typ"] != "JWT")
            {
                callback(false, "Unexpected header (typ).");
                return;
            }

            if ((string) header["alg"] != "RS256")
            {
                callback(false, "Unexpected header (alg).");
                return;
            }

            var payload = JObject.Parse(Payload);

            if ((string) payload["iss"] != "https://accounts.google.com")
            {
                callback(false, "Unexpected payload (iss).");
                return;
            }

            if ((string) payload["aud"] != Settings.Instance.ClientId)
            {
                callback(false, "Unexpected payload (aud).");
                return;
            }

            var exp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds((int) payload["exp"]);

            if (exp < DateTime.UtcNow)
            {
                callback(false, "JWT expired.");
                return;
            }

            var request = UnityWebRequest.Get(JwksUri); // TODO: Cache keys.

            request.SendWebRequest().completed += obj =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var certs = JObject.Parse(request.downloadHandler.text);
                    var keys = certs["keys"].ToDictionary(i => i["kid"], i => i);
                    var kid = header["kid"];

                    if (!keys.ContainsKey(kid))
                    {
                        callback(false, $"Public key not found (kid={kid}).");
                        return;
                    }

                    var modulus = Base64UrlEncoder.DecodeBytes((string) keys[kid]["n"]);
                    var exponent = Base64UrlEncoder.DecodeBytes((string) keys[kid]["e"]);
                    var parameters = new RSAParameters { Modulus = modulus, Exponent = exponent };
                    var provider = new RSACryptoServiceProvider();

                    provider.ImportParameters(parameters);

                    var signature = Base64UrlEncoder.DecodeBytes(Signature);
                    var sha = new SHA256Managed();
                    var data = Encoding.UTF8.GetBytes(SignedData);
                    var verified = provider.VerifyData(data, sha, signature);

                    if (verified)
                    {
                        callback(true, null);
                    }
                    else
                    {
                        callback(false, "Invalid JWT signature.");
                    }
                }
                else
                {
                    callback(false, $"{request.error}: {request.downloadHandler.text}");
                }

                request.Dispose();
            };
        }
    }
}
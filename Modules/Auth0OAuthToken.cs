using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Web.Script.Serialization;

namespace Auth0OAuth.Modules
{
    public class Auth0OAuthToken
    {
        public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public string access_token { get; set; }
        public string token_type { get; set; }
        public string id_token { get; set; }

        private ClaimsPrincipal _principal;

        public static Auth0OAuthToken FromStream(Stream stream)
        {
            var serializer = new JavaScriptSerializer();
            using (var reader = new StreamReader(stream))
            {
                return serializer.Deserialize<Auth0OAuthToken>(reader.ReadToEnd());
            }
        }

        public bool IsValid()
        {
            var principal = GetPrincipal();
            var exp = principal.FindFirst("exp");
            var secs = Int32.Parse(exp.Value);
            return DateTime.UtcNow <= EpochTime.AddSeconds(secs);
        }

        public static Auth0OAuthToken FromBytes(byte[] bytes)
        {
            var serializer = new JavaScriptSerializer();
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new StreamReader(stream))
                {
                    return serializer.Deserialize<Auth0OAuthToken>(reader.ReadToEnd());
                }
            }
        }

        public byte[] ToBytes()
        {
            var serializer = new JavaScriptSerializer();
            return Encoding.UTF8.GetBytes(serializer.Serialize(this));
        }

        public ClaimsPrincipal GetPrincipal()
        {
            if (_principal != null)
            {
                return _principal;
            }

            var base64 = id_token.Split('.')[1];

            // fixup
            int mod4 = base64.Length % 4;
            if (mod4 > 0)
            {
                base64 += new string('=', 4 - mod4);
            }

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            var claims = new List<Claim>();
            var serializer = new JavaScriptSerializer();
            foreach (var pair in serializer.Deserialize<Dictionary<string, object>>(json))
            {
                AddClaim(claims, pair.Key, pair.Value);
            }

            var identity = new ClaimsIdentity(claims, "auth0");
            return _principal = new ClaimsPrincipal(identity);
        }

        static void AddClaim(List<Claim> claims, string key, object value)
        {
            if (value == null)
            {
                //claims.Add(new Claim(key, "<null>"));
            }
            else if (value is ArrayList)
            {
                var array = (ArrayList)value;
                for (int i = 0; i < array.Count; ++i)
                {
                    AddClaim(claims, String.Format("{0}[{1}]", key, i), array[i]);
                }
            }
            else if (value is Dictionary<string, object>)
            {
                var dict = (Dictionary<string, object>)value;
                foreach (var item in dict)
                {
                    AddClaim(claims, String.Format("{0}.{1}", key, item.Key), item.Value);
                }
            }
            else
            {
                claims.Add(new Claim(key, value.ToString(), value.GetType().Name));
            }
        }
    }
}
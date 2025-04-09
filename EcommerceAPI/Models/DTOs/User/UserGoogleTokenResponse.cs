using Newtonsoft.Json;

namespace EcommerceAPI.Models.DTOs.User
{
    public class UserGoogleTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonProperty("id_token")]
        public string IdToken { get; set; } = string.Empty;
    }
}

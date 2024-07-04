using System.Text.Json.Serialization;

namespace Dashboard2.Server.Model.OpenId
{
    public readonly record struct TokenError
    {
        [JsonPropertyName("error")]
        public string Error { get; init; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; init; }
    }
}

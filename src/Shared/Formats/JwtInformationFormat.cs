namespace Shared.Formats
{
    public class JwtInformationFormat
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string ExpirationInMinutes { get; set; }
        public required string SecretKey { get; set; }
    }
}

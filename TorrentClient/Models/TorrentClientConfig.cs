namespace TorrentClient.Models
{
    public record TorrentClientConfig
    {
        public string ApiUrl { get; set; } = "";
        public bool HasAuthentication { get; set; } = false;
        public string? Username { get; set; }
        public string? Password { get; set; }

        public string[] ToDetailsArray()
        {
            
            if (!HasAuthentication)
            {
                return [
                    "Requires Authentication: False",
                    "API Url: " + (string.IsNullOrWhiteSpace(ApiUrl) ? "Not Set" : ApiUrl),
                    ];
            }

            return [
                "Requires Authentication: True",
                "API Url: " + ApiUrl,
                "Username: " + Username,
                "Password: " + (Password == null ? "Not Set" : Password.Replace(@"\w", "*"))
            ];
        }
    }
}

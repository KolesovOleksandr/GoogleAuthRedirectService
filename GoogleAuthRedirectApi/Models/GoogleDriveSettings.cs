namespace GoogleAuthRedirectApi.Models;

public class GoogleDriveSettings: IGoogleDriveSettings
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string RedirectUri { get; set; } = null!;
    public string GrantType { get; set; } = null!;
}

public interface IGoogleDriveSettings
{
    string ClientId { get; set; }
    string ClientSecret { get; set; }
    string RedirectUri { get; set; }   
    string GrantType { get; set; }
}
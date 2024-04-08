namespace GoogleAuthRedirectApi.Models;

public class TokensDbSettings : ITokensDbSettings
{
    public string TokensCollectionName { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}

public interface ITokensDbSettings
{
    string TokensCollectionName { get; set; }
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
}

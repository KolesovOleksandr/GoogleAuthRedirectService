using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleAuthRedirectApi.Models;

public class UserTokens
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime AccessTokenExpiryTime { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public List<string> Scopes { get; set; } = [];
    public DateTime LastUpdated { get; set; }
}

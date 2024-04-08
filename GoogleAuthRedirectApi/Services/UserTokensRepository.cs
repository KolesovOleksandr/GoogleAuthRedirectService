using GoogleAuthRedirectApi.Models;
using MongoDB.Driver;

namespace GoogleAuthRedirectApi.Services;

public class UserTokensRepository: IUserTokensRepository
{
    private readonly IMongoCollection<UserTokens> _users;

    public UserTokensRepository(ITokensDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);

        _users = database.GetCollection<UserTokens>(settings.TokensCollectionName);
    }

    public async Task<bool> AddUserTokensAsync(UserTokens userTokens)
    {
        if (userTokens != null)
        {
            await _users.InsertOneAsync(userTokens);
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<UserTokens> GetUserTokensAsync(string userId)
    {
        var userTokens = await _users.FindAsync(ut => ut.UserId == userId);

        return userTokens.FirstOrDefault();
    }

    public async Task<bool> UpdateUserTokensAsync(
        string userId, 
        string newAccessToken, 
        DateTime newExpiration, 
        string? newRefreshToken = null)
    {
        var filter = Builders<UserTokens>.Filter.Eq(ut => ut.UserId, userId);
        var update = Builders<UserTokens>.Update
            .Set(ut => ut.AccessToken, newAccessToken)
            .Set(ut => ut.AccessTokenExpiryTime, newExpiration);

        if (!string.IsNullOrEmpty(newRefreshToken))
        {
            update = update.Set(ut => ut.RefreshToken, newRefreshToken);
        }

        await _users.UpdateOneAsync(filter, update);

        return true;
    }

    public async Task<bool> DeleteUserTokensAsync(string userId)
    {
        var filter = Builders<UserTokens>.Filter.Eq(ut => ut.UserId, userId);

        await _users.DeleteOneAsync(filter);

        return true;
    }
}

public interface IUserTokensRepository
{
    public Task<bool> AddUserTokensAsync(UserTokens userTokens);

    public Task<UserTokens> GetUserTokensAsync(string userId);

    public Task<bool> UpdateUserTokensAsync(
        string userId,
        string newAccessToken,
        DateTime newExpiration,
        string? newRefreshToken = null);

    public Task<bool> DeleteUserTokensAsync(string userId);
}
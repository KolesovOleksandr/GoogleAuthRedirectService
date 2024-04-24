using GoogleAuthRedirectApi.Models;
using GoogleAuthRedirectApi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GoogleAuthRedirectApi.Controllers;

public class RedirectController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserTokensRepository _userTokensRepository;
    private readonly IGoogleDriveSettings _googleDriveSettings;

    public RedirectController(
        IHttpClientFactory httpClientFactory, 
        IUserTokensRepository userTokensRepository,
        IGoogleDriveSettings googleDriveSettings)
    {
        _httpClientFactory = httpClientFactory;
        _userTokensRepository = userTokensRepository;
        _googleDriveSettings = googleDriveSettings;
    }

    [HttpGet]
    //http://103.45.246.54:8081/Redirect/Index
    public async Task<IActionResult> Index(string code, string state, string error)
    {
        if (!string.IsNullOrEmpty(error)) 
        {
            return View("Error", error);
        }

        if(string.IsNullOrEmpty(code))
        {
            return View("Error", "No authorization code provided");
        }

        dynamic stateData;
        try
        {
            var decodedState = Encoding.UTF8.GetString(Convert.FromBase64String(state));
            stateData = JsonConvert.DeserializeObject<dynamic>(decodedState);
        }
        catch (Exception ex)
        {
            // Log the exception and handle the error appropriately
            return View("Error", "Invalid state parameter.");
        }

        if (stateData == null || string.IsNullOrEmpty(stateData.user_id.ToString()))
        {
            return View("Error", "Telegram user id is missing in the state.");
        }

        var userId = stateData.user_id.ToString();

        var tokens = await ExchangeCodeForTokensAsync(code);
        await StoreTokensAsync(userId, tokens);

        return Content("Authentication successful. You can close this window.");
    }

    private async Task<Dictionary<string, string>> ExchangeCodeForTokensAsync(string code)
    {
        var client = _httpClientFactory.CreateClient();

        var tokenRequest = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = _googleDriveSettings.ClientId,
            ["client_secret"] = _googleDriveSettings.ClientSecret,
            ["redirect_uri"] = _googleDriveSettings.RedirectUri,
            ["grant_type"] = _googleDriveSettings.GrantType
        };

        var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to exchange authorization code: {responseContent}");
        }

        return JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
    }

    private async Task<bool> StoreTokensAsync(string userId, Dictionary<string, string> tokens)
    {
        if (tokens.ContainsKey("access_token"))
        {
            var userTokens = new UserTokens
            {
                UserId = userId,
                AccessToken = tokens["access_token"],
                RefreshToken = tokens.ContainsKey("refresh_token") ? tokens["refresh_token"] : null,
                AccessTokenExpiryTime = DateTime.UtcNow.AddSeconds(int.Parse(tokens["expires_in"])),
                LastUpdated = DateTime.UtcNow,
                Scopes = new List<string> { "https://www.googleapis.com/auth/drive.file" }
            };

            await _userTokensRepository.AddUserTokensAsync(userTokens);
            return true;
        }

        return false;
    }

}


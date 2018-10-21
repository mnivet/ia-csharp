using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class CodingGameClient
{
    /** url of the coding game server */
    private static readonly Uri ApiUrl = new Uri("https://coding-game.swat-sii.fr");
    ////private static readonly Uri API_URL = new Uri("http://localhost/");

    /** change this bool to show json response in console */
    private static readonly bool EnableJsonLog = false;

    private static readonly string CreateGameUrl = "api/fights";
    private static readonly string JoinGetGameUrl = "api/fights/{0}/players/{1}";
    private static readonly string PlayGameUrl = "api/fights/{0}/players/{1}/actions/{2}";

    private static readonly HttpClient Client = new HttpClient { BaseAddress = ApiUrl };
    private static readonly string JsonMediaType = "application/json";
    private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public static async Task<Game> CreateGame(string gameName, bool speedy, bool versus)
    {
        var content = JsonSerialize(new { name = gameName, speedy, versus });
        var response = await Client.PostAsync(CreateGameUrl, content);

        return await ReadResponseAndLog(response, "CREATE");
    }

    public static async Task<Game> JoinGame(string gameToken, string playerKey, string character, string name)
    {

        var content = JsonSerialize(new { character, name });
        var url = string.Format(JoinGetGameUrl, gameToken, playerKey);
        var response = await Client.PostAsync(url, content);

        return await ReadResponseAndLog(response, "JOIN");
    }

    public static async Task<Game> GetGame(string gameToken, string playerKey)
    {
        var url = string.Format(JoinGetGameUrl, gameToken, playerKey);
        var response = await Client.GetAsync(url);

        return await ReadResponseAndLog(response, "GET");
    }

    public static async Task<Game> Play(string gameToken, string playerKey, string actionName)
    {
        Console.Out.WriteLine($"{actionName} !");
        var url = string.Format(PlayGameUrl, gameToken, playerKey, actionName);
        var content = new StringContent(string.Empty, Encoding.UTF8, JsonMediaType);
        var response = await Client.PostAsync(url, content);

        return await ReadResponseAndLog(response, "PLAY");
    }

    public static async Task<Game> PlayAndWaitCoolDown(string gameToken, string playerKey, string actionName)
    {
        var game = await Play(gameToken, playerKey, actionName);
        WaitCoolDown(game, actionName);
        return game;
    }

    private static HttpContent JsonSerialize(object content)
    {
        var jsonContent = JsonConvert.SerializeObject(content, JsonSettings);
        return new StringContent(jsonContent, Encoding.UTF8, JsonMediaType);
    }

    private static async Task<Game> ReadResponseAndLog(HttpResponseMessage response, string requestType)
    {
        var contentString = await response.Content.ReadAsStringAsync();
        try
        {
            var game = JsonConvert.DeserializeObject<Game>(contentString);
            if (EnableJsonLog)
            {
                Console.Out.WriteLine("Response from " + requestType + " game request:");
                Console.Out.WriteLine(contentString);
            }
            return game;
        }
        catch (Exception e)
        {
            throw new SerializationException($"Unable to deserialize the following content as json:{Environment.NewLine}{contentString}", e);
        }
    }

    private static void WaitCoolDown(Game game, string actionName)
    {
        var action = game.Me?.Character.Actions
            .FirstOrDefault(p => p.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase));
        if (action == null || action.CoolDown <= 0) return;

        var waitDuration = (int)Math.Ceiling(action.CoolDown * game.Speed);
        Console.Out.WriteLine($"Waiting cool down during {waitDuration}ms...");
        Thread.Sleep(waitDuration);
    }
}


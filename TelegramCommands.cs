using System.Collections;
using System.Data;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using JikanDotNet;
using JikanDotNet.Config;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace MyAPI;

public class TelegramCommands
{
    public async Task Neko(ITelegramBotClient botClient, Message message)
    {
        var client = new RestClient("https://myanimewebapi.azurewebsites.net");
        var request = new RestRequest("/Anime/NEKO");
        var response = client.Execute(request);
        var json = (string)JsonObject.Parse(response.Content);
        var splittedText = json.Split('"');
        var answer = splittedText[^2];
        InputFile finAnswer = InputFile.FromUri(answer);
        await botClient.SendPhotoAsync(message.Chat.Id, finAnswer);
    }

    public async Task Waifu(ITelegramBotClient botClient, Message message)
    {
        var client = new RestClient("https://myanimewebapi.azurewebsites.net");
        var request = new RestRequest("/Anime/WAIFU");
        var response = client.Execute(request);
        var json = (string)JsonObject.Parse(response.Content);
        var splittedText = json.Split('"');
        var answer = splittedText[^2];
        InputFile finAnswer = InputFile.FromUri(answer);
        await botClient.SendPhotoAsync(message.Chat.Id, finAnswer);
    }
    
    public async Task Bl(ITelegramBotClient botClient, Message message)
    {
        var client = new RestClient("https://myanimewebapi.azurewebsites.net");
        var request = new RestRequest("/Anime/bl");
        var response = client.Execute(request);
        var json = (string)JsonObject.Parse(response.Content);
        var splittedText = json.Split('"');
        var answer = splittedText[^2];
        InputFile finAnswer = InputFile.FromUri(answer);
        await botClient.SendPhotoAsync(message.Chat.Id, finAnswer);
    }
    
    public async Task FindAnimeByName(ITelegramBotClient botClient, Message message)
    {
        var client = new RestClient("https://myanimewebapi.azurewebsites.net");
        var request = new RestRequest($"/Anime/Anime?name={message.Text}");
        var response = client.Execute(request);
        var anilist = response.Content;

        // Convert the anilist response into a list
        var animeList = JsonConvert.DeserializeObject<List<Anime>>(anilist);

        if (animeList != null && animeList.Count > 0)
        {
            var i = 0;
            foreach (var anime in animeList)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"ID: {JsonObject.Parse(anilist)[i]["mal_id"]}{Environment.NewLine}Title: {anime.Title}{Environment.NewLine}Synopsis: {anime.Synopsis}{Environment.NewLine}URL: {anime.Url}");
                i++;
            }

        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "No anime found with that name.");
        }
    }
    
    public async Task RandomAnime(ITelegramBotClient botClient, Message message)
    {
        var client = new RestClient("https://myanimewebapi.azurewebsites.net");
        var request = new RestRequest("/Anime/Random Anime");
        var response = client.Execute(request);
        var mal_id = (int)JsonObject.Parse(response.Content)["mal_id"];
        var title = (string)JsonObject.Parse(response.Content)["title"];
        var synopsis = (string)JsonObject.Parse(response.Content)["synopsis"];
        var url = (string)JsonObject.Parse(response.Content)["url"];


        await botClient.SendTextMessageAsync(message.Chat.Id,
            $"ID: {mal_id}{Environment.NewLine}Title: {title}{Environment.NewLine}Synopsis: {synopsis}" +
            $"{Environment.NewLine}URL: {url}");
    }

    public async Task FindMangaByName(ITelegramBotClient botClient, Message message)
    {
        var client = new RestClient("https://myanimewebapi.azurewebsites.net");
        var request = new RestRequest($"/Anime/Manga?name={message.Text}");
        var response = client.Execute(request);
        var manlist = response.Content;

        // Convert the manlist response into a list
        var mangaList = JsonConvert.DeserializeObject<List<Manga>>(manlist);

        if (mangaList != null && mangaList.Count > 0)
        {
            var i = 0;
            foreach (var manga in mangaList)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"ID: {(int)JsonObject.Parse(response.Content)[i]["mal_id"]}{Environment.NewLine}Title: {manga.Title}{Environment.NewLine}Synopsis: {manga.Synopsis}{Environment.NewLine}URL: {manga.Url}");
                i++;
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "No manga found with that name.");
        }
    }
    
    public async Task RandomManga(ITelegramBotClient botClient, Message message)
    {
        var client = new RestClient("https://myanimewebapi.azurewebsites.net");
        var request = new RestRequest("/Anime/Random Manga");
        var response = client.Execute(request);
        var mal_id = (int)JsonObject.Parse(response.Content)["mal_id"];
        var title = (string)JsonObject.Parse(response.Content)["title"];
        var synopsis = (string)JsonObject.Parse(response.Content)["synopsis"];
        var url = (string)JsonObject.Parse(response.Content)["url"];
            
            
        await botClient.SendTextMessageAsync(message.Chat.Id, $"ID: {mal_id}{Environment.NewLine}Title: {title}{Environment.NewLine}Synopsis: {synopsis}" +
                                                              $"{Environment.NewLine}URL: {url}");
    }
    
    public async Task ShowTop(ITelegramBotClient botClient, Message message)
    {
        //var client = new RestClient("https://localhost:7166");
        //var request = new RestRequest("/Anime/Public Top Anime");
        //var response = client.Execute(request);
        //var data = response.Content;
        //var top = new List<(int, string)>();
        //foreach (var anime in JsonObject.Parse(data).AsArray())
        //{
        //    var id = (int)anime["id"];
        //    var name = (string)anime["name"];
        //    top.Add((id, name));
        //}
        //await botClient.SendTextMessageAsync(message.Chat.Id, top.Select(x => $"ID: {x.Item1}\nTitle: {x.Item2}\n\n----------\n\n").Aggregate((x,y)=>x+y));
        var client = new HttpClient();
        var response = await client.GetAsync("https://myanimewebapi.azurewebsites.net/Anime/Public Top Anime");
        var data = await response.Content.ReadAsStringAsync();
        var top = new List<(int, string)>();
        foreach (var anime in JsonObject.Parse(data).AsArray())
        {
            var id = (int)anime["id"];
            var name = (string)anime["name"];
            top.Add((id, name));
        }
        await botClient.SendTextMessageAsync(message.Chat.Id, top.Select(x => $"ID: {x.Item1}\nTitle: {x.Item2}\n\n----------\n\n").Aggregate((x,y)=>x+y));



    }

    public async Task AddToDataBase(ITelegramBotClient botClient, string message)
    {
        var id = Convert.ToInt32(message);
        var client = new RestClient("https://myanimewebapi.azurewebsites.net");
        var request = new RestRequest($"/Anime/PostAsync?id={id}", Method.Post);
        client.ExecuteAsync(request);
        //var client = new HttpClient();
        //await client.PostAsync("https://localhost:7166/Anime/PostAsync", new StringContent(System.Text.Json.JsonSerializer.Serialize(new {Id = id}), Encoding.Unicode, "application/json"));



    }
    public async Task DeleteFromDataBase(ITelegramBotClient botClient, string message)
    {
        var id = Convert.ToInt32(message);
        var client = new RestClient("https://myanimewebapi.azurewebsites.net");
        var request = new RestRequest($"/Anime/Delete?id={id}", Method.Delete);
        client.Execute(request);
        //var client = new HttpClient();
        //await client.DeleteAsync($"https://localhost:7166/Anime/Delete?id={id}");

    }
}
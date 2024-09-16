using System.Text;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using JikanDotNet;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MyAPI;

public class TelegramBot
{
    TelegramBotClient botClient = new TelegramBotClient("//bot token");
    CancellationTokenSource cancellationToken = new CancellationTokenSource();
    ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
    TelegramCommands TelegramCommands = new TelegramCommands();
    public string currentMenu = "Home";
    string name;

    public async Task Start()
    {
        botClient.StartReceiving(HandlerUpdateAsync, HandlerErrorAsync, receiverOptions, cancellationToken.Token);
        var botMe = await botClient.GetMeAsync();
        Console.WriteLine($"Bot {botMe.Username} started");
        Console.ReadKey();
    }
    
    private async Task HandlerErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Помилка в телеграм бот АПІ:\n {apiRequestException.ErrorCode}" +
                                                       $"\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        Console.WriteLine(ErrorMessage);
        if (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                
                await Task.Delay(1000, cancellationToken);

                botClient.StartReceiving(HandlerUpdateAsync, HandlerErrorAsync, receiverOptions, cancellationToken);
            }
            catch (Exception restartException)
            {
                Console.WriteLine($"An error occurred while restarting the bot: {restartException.Message}");
            }
        }
        
    }

    private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update?.Message?.Text != null)
        {
            await HandlerMessageAsync(botClient, update.Message);
        }
    }
    private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
    {
        switch (currentMenu)
        {
            case "findanime":
                await TelegramCommands.FindAnimeByName(botClient, message);
                currentMenu = "Home";
                break;
            case "findmanga":
                await TelegramCommands.FindMangaByName(botClient, message);
                currentMenu = "Home";
                break;
            case "addtotop":
                await TelegramCommands.AddToDataBase(botClient, message.Text);
                await botClient.SendTextMessageAsync(message.Chat.Id, "Done!");
                currentMenu = "Home";
                break;
            case "deletefromtop":
                await TelegramCommands.DeleteFromDataBase(botClient, message.Text);
                await botClient.SendTextMessageAsync(message.Chat.Id, "Done!");
                currentMenu = "Home";
                break;
            
        }

        switch (message.Text)
        {
            case "/start" or "/start@mI_animes_bot":
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Welcome to the Anime Telegram Bot! This bot uses Jikan API." +
                    "To have more info about the bot, type /help.");
                break;
            case "/help" or "/help@mI_animes_bot":
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    $"Available commands: {Environment.NewLine}1./findanime - Find an anime by it's title{Environment.NewLine}2./randomanime - Random anime" +
                    $"{Environment.NewLine}3./findmanga - Find manga by it's name{Environment.NewLine}4./randommanga - Random manga" +
                    $"{Environment.NewLine}5./addtotop - Add a specified anime to 'Top User's Anime List' using an id" + 
                    $"{Environment.NewLine}6./deletefromtop - Delete a specified anime from the 'Top User's Anime List' using an id" +
                    $"{Environment.NewLine}7./showtop - Show 'Top User's AnimeList'");
                break;
            case "/findanime":
            case "/findanime@mI_animes_bot":
                await botClient.SendTextMessageAsync(message.Chat.Id, "Please enter Anime title: ");
                currentMenu = "findanime";
                break;
            case "/randomanime" or "/randomanime@mI_animes_bot":
                await TelegramCommands.RandomAnime(botClient, message);
                break;
            case "/findmanga" or "/findmanga@mI_animes_bot":
                await botClient.SendTextMessageAsync(message.Chat.Id, "Please enter Manga title: ");
                currentMenu = "findmanga";
                break;
            case "/randommanga" or "/randommanga@mI_animes_bot":
                await TelegramCommands.RandomManga(botClient, message);
                break;
            case "/addtotop" or "/addtotop@mI_animes_bot":
                await botClient.SendTextMessageAsync(message.Chat.Id, "Enter the ID of an Anime you would like to add to the top:");
                currentMenu = "addtotop";
                break;
            case "/deletefromtop" or "/deletefromtop@mI_animes_bot":
                await botClient.SendTextMessageAsync(message.Chat.Id, "Enter the ID of an Anime you would like to delete from the top:");
                currentMenu = "deletefromtop";
                break;
            case "/showtop" or "/showtop@mI_animes_bot":
                await TelegramCommands.ShowTop(botClient, message);
                break;
            case "/neko" or "/neko@mI_animes_bot":
                await TelegramCommands.Neko(botClient, message);
                break;
            case "/waifu" or "/waifu@mI_animes_bot":
                await TelegramCommands.Waifu(botClient, message);
                break;
            case "/bl" or "/bl@mI_animes_bot":
                await TelegramCommands.Bl(botClient, message);
                break;
        }

        return;
        
    }
}

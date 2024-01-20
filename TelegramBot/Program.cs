using System;
using Telegram.Bot;
using Telegram.Bot.Types;


internal class Program
{
    static void Main(string[] args)
    {
        var client = new TelegramBotClient("6791814675:AAFdzNpWAJFB7EXxh0gVmmrp_89TPn3EHKQ");
        client.StartReceiving(Update, Error);
        Console.ReadLine();
    }

    private async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        string answer = "Error";
        var message = update.Message;
        
        if(message?.Text != null)
        {
            await Console.Out.WriteLineAsync($"{message.Chat.FirstName}   |   {message.Text}");

            answer = await GPTBot.SendCompletionRequest(message.Text);

            await botClient.SendTextMessageAsync(message.Chat.Id, answer);
            await Console.Out.WriteLineAsync($"bot: {answer}");

            //if(message.Text == "Здарова" || message.Text == "здарова")
            //{
            //    answer = "Корова";
            //    await botClient.SendTextMessageAsync(message.Chat.Id, answer);
            //    await Console.Out.WriteLineAsync($"bot: {answer}");
            //    return;
            //}

            //if(message.Text == "Изи" || message.Text == "изи")
            //{
            //    answer = "сосм";
            //    await botClient.SendTextMessageAsync(message.Chat.Id, answer);
            //    await Console.Out.WriteLineAsync($"bot: {answer}");
            //    return;
            //}

            //if(message.Text == "Корова" || message.Text == "корова")
            //{
            //    answer = "Здорова";
            //    await botClient.SendTextMessageAsync(message.Chat.Id, answer);
            //    await Console.Out.WriteLineAsync($"bot: {answer}");
            //    return;
            //}
            //if((message.Text == "Привет" || message.Text == "привет"))
            //{
            //    answer = "изи";
            //    await botClient.SendTextMessageAsync(message.Chat.Id, answer);
            //    await Console.Out.WriteLineAsync($"bot: {answer}");
            //    return;
            //}


        }
    }


    private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"Произошла ошибка: {exception.Message}");
        return Task.CompletedTask;
        //throw new NotImplementedException();
    }

}

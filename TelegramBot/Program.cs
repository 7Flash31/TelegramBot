using System.Diagnostics;
using System.Reflection;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

internal class Program
{
    private static string fileName = DateTime.Now.ToString().Replace("/", ".").Replace(":", ".");
    private static string logPath = $"telegram-bot\\Logs\\{fileName}.txt";
    public static string imagePath = $"telegram-bot\\Image\\";

    enum MessageType
    {
        Bot, //Простое сообщение от бота
        Gpt //Ответ от gpt
    }

    static void Main(string[] args)
    {
        string basePath = "./"; 

        logPath = Path.Combine(basePath, logPath);
        imagePath = Path.Combine(basePath, imagePath);

        EnsureDirectoriesExist();

        Console.ForegroundColor = ConsoleColor.White;
        var client = new TelegramBotClient("6791814675:AAFdzNpWAJFB7EXxh0gVmmrp_89TPn3EHKQ");

        client.StartReceiving(Update, Error);
        var me = client.GetMeAsync();
        Console.ReadLine();
    }

    static void EnsureDirectoriesExist()
    {
        string logDirectory = Path.GetDirectoryName(logPath);
        if(!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        if(!Directory.Exists(imagePath))
        {
            Directory.CreateDirectory(imagePath);
        }
    }

    private async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        string answer = "Error";

        var message = update.Message;

        if(message?.Text != null)
        {
            var isSub = await botClient.GetChatMemberAsync("@WhizGPT", message.Chat.Id);

            if (isSub.Status.ToString().Length >= 5)
            {
                //if(ProcessCommand(message.Text, true) == "Нарисуй")
                //{
                //    SendMessage(MessageType.Bot, botClient, update, "Рисую");

                //    string promt = ProcessCommand(message.Text, false);

                //    await Kandinsky.GetGenerateImage(promt);

                //    WriteLog($"Image ({promt}) saved to {Kandinsky.GetFilePath()}");

                //    using(var fileStream = new FileStream(Kandinsky.GetFilePath(), FileMode.Open, FileAccess.Read, FileShare.Read))
                //    {
                //        await botClient.SendPhotoAsync(
                //            chatId: message.Chat.Id,
                //            photo: InputFile.FromStream(fileStream),
                //            caption: promt
                //        );

                //        fileStream.Close();
                //    }

                //}

                if(update.Message.Chat.Username.ToString() == "@chepuxxaa")
                    SendMessage(MessageType.Bot, botClient, update, "Соси");

                else
                    SendMessage(MessageType.Gpt, botClient, update, message.Text);
            }

            else
            {
                answer = "Вы не подписаны на канал @WhizGPT";
                await botClient.SendTextMessageAsync(message.Chat.Id, answer);
                await Console.Out.WriteLineAsync($"bot: {answer} | ответ для @{message.Chat.Username}");
                return;
            }
        }
    }

    private static async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"Telegram: {exception.Message}");
        WriteLog($"Telegram: {exception.Message}");

        var executablePath = Assembly.GetExecutingAssembly().Location;

        try
        {
            // Запуск исполняемого файла, а не .dll файла
            var processStartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error restarting application: {ex.Message}");
            WriteLog($"Error restarting application: {ex.Message}");
        }

        // Завершение текущего процесса
        Environment.Exit(0);

        await Task.CompletedTask;
    }

    private async static void WriteLog(string text)
    {
        using(FileStream fstream = new FileStream(logPath, FileMode.Append))
        {
            byte[] buffer = Encoding.Default.GetBytes(text);
            await fstream.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    private static string ProcessCommand(string command, bool first)
    {
        string[] parts = command.Split(':');

        if(parts.Length >= 2)
        {
            string firstWord = parts[0].Trim();
            string afterColon = parts[1].Trim();

            if(first)
                return firstWord;

            else
                return afterColon;
        }
        else
            return "Некорректный формат команды. Убедитесь, что есть двоеточие.";
    }

    private async static void SendMessage(MessageType type, ITelegramBotClient botClient, Update update, string textBot)
    {
        string logText;

        if(type == MessageType.Bot)
        {
            //Client
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{DateTime.Now} @{update.Message.Chat.Username}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{update.Message.Text}");

            logText = $"{Environment.NewLine}{DateTime.Now} @{update.Message.Chat.Username}: {update.Message.Text}";
            WriteLog(logText);

            //Bot
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{DateTime.Now} Bot: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{textBot} | Ответ для {update.Message.Chat.Username}");

            await botClient.SendTextMessageAsync(update.Message.Chat.Id, textBot);

            logText = $"{Environment.NewLine}{DateTime.Now} Bot: {textBot} | Ответ для @{update.Message.Chat.Username}";
            WriteLog(logText);
        }

        if(type == MessageType.Gpt)
        {
            string answer = await YandexGPT.SendCompletionRequest(update.Message.Text);

            //Client
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{DateTime.Now} @{update.Message.Chat.Username}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{update.Message.Text}");

            logText = $"{Environment.NewLine}{DateTime.Now} @{update.Message.Chat.Username}: {update.Message.Text}";
            WriteLog(logText);

            //Bot
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{DateTime.Now} Bot: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{answer} | Ответ для @{update.Message.Chat.Username}");

            await botClient.SendTextMessageAsync(update.Message.Chat.Id, answer);

            logText = $"{Environment.NewLine}{DateTime.Now} Bot: {answer} | Ответ для @{update.Message.Chat.Username}";
            WriteLog(logText);
        }
    }
}

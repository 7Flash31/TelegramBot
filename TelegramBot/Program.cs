using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

internal class Program
{
    private static string fileName = DateTime.Now.ToString().Replace("/", ".").Replace(":", ".");
    public static string logPath = $"telegram-bot\\Logs\\{fileName}.txt";
    public static string imagePath = $"telegram-bot\\Image\\";
    private static Dictionary<long, UserState> userStates = new Dictionary<long, UserState>();

    public static bool drawQuestion;
    public static Update update;

    private static async Task Main(string[] args)
    {
        string basePath = "./";
        logPath = Path.Combine(basePath, logPath);
        imagePath = Path.Combine(basePath, imagePath);

        Utilities.EnsureDirectoriesExist(logPath, imagePath);

        Console.ForegroundColor = ConsoleColor.White;
        var client = new TelegramBotClient("6791814675:AAFA9__aGx879v1y-1FtnZNr1LHgRU1i7sc");

        ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message },
            ThrowPendingUpdates = true,
        };

        client.StartReceiving(Update, Error, receiverOptions);
        await Task.Delay(Timeout.Infinite);
    }

    private static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        try
        {
            var message = update.Message;

            if(update.Type == UpdateType.Message)
            {
                var isSub = await botClient.GetChatMemberAsync("@WhizGPT", message.Chat.Id);
                if(isSub.Status.ToString().Length >= 5)
                {
                    await ProcessMessage(botClient, update, message);
                }
                else
                {
                    await SendMessage(MessageType.Bot, botClient, update, "Вы не подписаны на @WhizGPT");
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error {ex}");
            Utilities.WriteLog(ex.ToString());
        }
    }

    private static async Task ProcessMessage(ITelegramBotClient botClient, Update update, Message message)
    {
        string drawPrompt;

        UserState userState;
        if(!userStates.TryGetValue(message.Chat.Id, out userState))
        {
            userState = new UserState();
            userStates[message.Chat.Id] = userState;
        }

        if(userState.DrawQuestion)
        {
            drawPrompt = message.Text;
            await SendMessage(MessageType.Bot, botClient, update, $"Рисую: {drawPrompt}");

            await GenerationImage(drawPrompt, botClient, update);

            userState.DrawQuestion = false;
        }
        else
        {
            if(message.Text.StartsWith("/"))
            {
                await ProcessCommand(botClient, update, message.Text.Substring(1), userState);
            }
            else if(message.Text == "Нарисуй")
            {
                userState.DrawQuestion = true;
                await SendMessage(MessageType.Bot, botClient, update, "Что нарисовать?");
            }
            else
            {
                await SendMessage(MessageType.Gpt, botClient, update, message.Text);
            }
        }
    }

    private static async Task ProcessCommand(ITelegramBotClient botClient, Update update, string command, UserState userState)
    {
        switch(command)
        {
            case "reply":
                var replyKeyboard = new ReplyKeyboardMarkup(new List<KeyboardButton[]>() {
                new KeyboardButton[]
                {
                    new KeyboardButton("Нарисуй"),
                },})
                {
                    ResizeKeyboard = true,
                };
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Готово", replyMarkup: replyKeyboard);
                break;

            case "draw":
                userState.DrawQuestion = true;
                await SendMessage(MessageType.Bot, botClient, update, "Что нарисовать?");
                break;

            default:
                break;
        }
    }

    private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"Telegram: {exception.Message}");
        Utilities.WriteLog($"Telegram: {exception.Message}");
        return Task.CompletedTask;
    }

    private async static Task SendMessage(MessageType type, ITelegramBotClient botClient, Update update, string textBot)
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
            Utilities.WriteLog(logText);

            //Bot
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{DateTime.Now} Bot: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{textBot} | Ответ для {update.Message.Chat.Username}");

            await botClient.SendTextMessageAsync(update.Message.Chat.Id, textBot, replyToMessageId: update.Message.MessageId);

            logText = $"{Environment.NewLine}{DateTime.Now} Bot: {textBot} | Ответ для @{update.Message.Chat.Username}";
            Utilities.WriteLog(logText);
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
            Utilities.WriteLog(logText);

            //Bot
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{DateTime.Now} Bot: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{answer} | Ответ для @{update.Message.Chat.Username}");

            await botClient.SendTextMessageAsync(update.Message.Chat.Id, answer, replyToMessageId: update.Message.MessageId);

            logText = $"{Environment.NewLine}{DateTime.Now} Bot: {answer} | Ответ для @{update.Message.Chat.Username}";
            Utilities.WriteLog(logText);
        }
    }

    private async static Task GenerationImage(string promt, ITelegramBotClient botClient, Update update)
    {
        try
        {
            await Kandinsky.GetGenerateImage(promt); // Асинхронный вызов

            Utilities.WriteLog($"Image ({promt}) saved to {Kandinsky.GetFilePath()}");

            using(var fileStream = new FileStream(Kandinsky.GetFilePath(), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await botClient.SendPhotoAsync(
                    chatId: update.Message.Chat.Id,
                    photo: InputFile.FromStream(fileStream),
                    caption: promt
                ); // Асинхронный вызов
            }
        }
        catch(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{DateTime.Now} Error: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{ex}");

            await SendMessage(MessageType.Bot, botClient, update, "Error"); // Асинхронный вызов
        }
    }
}


enum MessageType
{
    Bot, //Простое сообщение от бота
    Gpt //Ответ от gpt
}

class UserState
{
    public bool DrawQuestion { get; set; }
    // Другие свойства, если необходимо
}


using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Utilities
{
    public static void EnsureDirectoriesExist(string logPath, string imagePath)
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

    public async static void WriteLog(string text)
    {
        using(FileStream fstream = new FileStream(Program.logPath, FileMode.Append))
        {
            byte[] buffer = Encoding.Default.GetBytes(text);
            await fstream.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    public static string ProcessCommand(string command, bool first)
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
            return "Некорректный формат команды.";
    }

    static void ProcessCommand(string command)
    {
        switch(command.ToLower())
        {
            case "/draw":
                Console.WriteLine("Введите, что нарисовать:");
                string drawing = Console.ReadLine();
                break;

            // Добавьте другие команды по необходимости.

            default:
                Console.WriteLine("Неизвестная команда.");
                break;
        }
    }


    public static bool ProcessUserInput(string userInput)
    {
        return false;
    }
}
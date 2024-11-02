using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Data.Sqlite;
using Telegram.Bot.Types.Enums;

namespace AnonimusBot
{
    class Program
    {
        private static string token { get; set; } = "6743289067:AAG-iMSmCZ8AgofmZXeef2PBiZXDVA4RWM8";
        private static string dbConnectionString { get; set; } = @"Data Source=C:\Users\broke\source\repos\TextTranslatorBot\TextTranslatorBot\Databases\Test.db";

        static void Main()
        {
            BotService tgbot = new BotService(token, dbConnectionString);

            tgbot.Start();

            Console.ReadLine();
        }
    }
}
namespace AnonimusBot
{
    class Program
    {
        private static string token { get; set; } = "YOUR_TOKEN";
        private static string dbConnectionString { get; set; } = @"Data Source=C:\Users\broke\source\repos\TextTranslatorBot\TextTranslatorBot\Databases\Test.db";

        static void Main()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            BotService tgbot = new BotService(token, dbConnectionString, cts.Token);

            tgbot.Start();

            Console.ReadLine();

            cts.Cancel();
        }
    }
}

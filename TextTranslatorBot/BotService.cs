using System.Reflection.Metadata.Ecma335;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AnonimusBot
{

    public class BotService
    {
        private TelegramBotClient _bot;
        private string connectionString;
        private Registrator registrator;
        private Database database;
        private Server generalServer;
        private CommandAnalizator commandAnalizator;

        public BotService(string token, string dbConnectionString)
        {
            connectionString = dbConnectionString;
            _bot = new TelegramBotClient(token);

            database = new Database(dbConnectionString);
            registrator = new Registrator(_bot, database);
            commandAnalizator = new CommandAnalizator(database);
            generalServer = new Server("Server_1");
        }
        public void Start()
        {
            _bot.StartReceiving(UpdateTask, ErrorTask);
        }

        private async Task ErrorTask(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
            await Task.CompletedTask;
        }

        private async Task UpdateTask(ITelegramBotClient client, Update update, CancellationToken token)
        {
            Message? senderMessage = update.Message;

            if (senderMessage is null)
                return;

            long updateUserId = senderMessage.Chat.Id;
            string updateMessageText = senderMessage.Text is null ? "" : senderMessage.Text;

            if (!await database.IsUserIdInVerifed(updateUserId))
            {
                await registrator.HandleRegistration(updateUserId, updateMessageText, generalServer);
            }
            else
            {
                await commandAnalizator.AnalizeCommand(client, update);

                if (commandAnalizator.IsWaitForCommand(updateUserId))
                    return;

                User sender = await database.GetVerifedUserById(updateUserId);
                List<User> users = await database.GetUsersAsync(sender.ServerConnectedName);
                await GlobalMessageSender.HandleSendMessage(client, sender, senderMessage, users);
            }
        }
    }
}
using Telegram.Bot;
using Telegram.Bot.Types;

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
        private NavigationMenu navigationMenu;
        private CancellationToken cancellationToken;
        public BotService(string token, string dbConnectionString, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

            connectionString = dbConnectionString;
            _bot = new TelegramBotClient(token);

            generalServer = new Server("None");
            database = new Database(dbConnectionString, generalServer);
            registrator = new Registrator(_bot, database);
            commandAnalizator = new CommandAnalizator(database);
            navigationMenu = new NavigationMenu(database);

            commandAnalizator.ServerQuitEvent += navigationMenu.ShowServerChoosingMenu;
            registrator.RegistrationCompleteEvent += navigationMenu.ShowServerChoosingMenu;
        }
        public void Start()
        {
            _bot.StartReceiving(
                UpdateTask,
                ErrorTask,
                cancellationToken: cancellationToken
                );
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
            long updateUserId = senderMessage is null ? update.CallbackQuery.Message.Chat.Id : senderMessage.Chat.Id;


            if (!await database.IsUserIdInVerifed(updateUserId))
            {
                string updateMessageText = senderMessage.Text is null ? "" : senderMessage.Text;
                await registrator.HandleRegistration(updateUserId, updateMessageText, generalServer);
            }
            else
            {
                if (await database.GetServerAsync(updateUserId) == generalServer.Name && update.CallbackQuery is null)
                    await navigationMenu.ShowServerChoosingMenu(client, updateUserId);
                if (update.CallbackQuery is not null)
                    await navigationMenu.NavigateUserToServer(client, update);

                if (senderMessage is null)
                    return;

                await commandAnalizator.AnalizeCommand(client, update);

                if (commandAnalizator.IsWaitForCommand(updateUserId) || senderMessage.Text[0] == '/')
                    return;

                User sender = await database.GetVerifedUserById(updateUserId);
                List<User> users = await database.GetUsersAsync(sender.ServerConnectedName);
                await GlobalMessageSender.HandleSendMessage(client, sender, senderMessage, users);
            }
        }
    }
}
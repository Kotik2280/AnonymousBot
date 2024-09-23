using System.Net.Mail;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AnonimusBot
{

    public class BotService
    {
        private TelegramBotClient _bot;
        private string connectionString;
        private Registrator registrator;
        private Database database;

        public BotService(string token, string dbConnectionString)
        {
            connectionString = dbConnectionString;
            _bot = new TelegramBotClient(token);

            database = new Database(dbConnectionString);
            registrator = new Registrator(database);
        }
        public void Start()
        {
            _bot.StartReceiving(UpdateTask, ErrorTask);
        }

        private async Task ErrorTask(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
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
                await registrator.HandleRegistration(client, updateUserId, updateMessageText);
            }
            else
            {
                List<User> users = await database.GetUsersAsync();
                User sender = await database.GetVerifedUserById(update.Message.Chat.Id);
                await HandleSendMessage(client, sender, senderMessage, users);
            }
        }
        private async Task HandleSendMessage(ITelegramBotClient client, User sender, Message message, IEnumerable<User> users)
        {
            switch (message.Type)
            {
                case MessageType.Text:
                    await MessageSender.NotifyTextUsersExcludingSender(client, message.Text, users, sender);
                    break;
                case MessageType.Sticker:
                    await MessageSender.NotifyStickerUsersExcludingSender(client, message.Sticker, users, sender);
                    break;
                default:
                    break;
            }
        }
    }
}
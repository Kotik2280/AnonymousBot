using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AnonimusBot
{

    public class BotService
    {
        private TelegramBotClient _bot;
        private string connectionString;
        private Database database;

        public BotService(string token, string dbConnectionString)
        {
            connectionString = dbConnectionString;
            _bot = new TelegramBotClient(token);

            database = new Database(dbConnectionString);
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
            MessageType messageType;
            Message? senderMessage = update.Message;

            if (senderMessage is null)
                return;

            messageType = senderMessage.GetMessageType();

            long updateUserId = senderMessage.Chat.Id;
            string updateMessageText = senderMessage.Text is null ? "" : senderMessage.Text;

            if (!await database.IsUserIdInVerifed(updateUserId))
            {
                if (!await database.IsUserIdInRegistration(updateUserId))
                {
                    await client.SendTextMessageAsync(updateUserId, "Добро пожаловать!");
                    await database.AddToRegistration(updateUserId);
                    await client.SendTextMessageAsync(updateUserId, "Введите ник");
                }
                else
                {
                    await database.AddToVerifedUser(new User(updateUserId, updateMessageText));
                    await database.RemoveFromRegistration(updateUserId);
                    await client.SendTextMessageAsync(updateUserId, $"Приятного общения, {updateMessageText}!");


                    List<User> users = await database.GetUsersAsync();

                    foreach (User user in users)
                    {
                        Console.WriteLine(user);
                        await client.SendTextMessageAsync(user.Id, $"{updateMessageText} присоединился к чату!");
                    }
                }
            }
            else
            {
                User sender = await database.GetVerifedUserById(updateUserId);

                string userServer = await database.GetServerAsync(sender.Id);

                List<User> users = await database.GetUsersAsync();

                switch (messageType)
                {
                    case MessageType.Text:
                        foreach (User user in users)
                        {
                            if (user.Id == updateUserId)
                                continue;
                            await client.SendTextMessageAsync(user.Id, $"{database.GetVerifedUserById(updateUserId).Result.Name} : {updateMessageText}");
                        }
                        break;
                    case MessageType.Sticker:
                        foreach (User user in users)
                        {
                            if (user.Id == updateUserId)
                                continue;
                            await client.SendTextMessageAsync(user.Id, $"{database.GetVerifedUserById(updateUserId).Result.Name} :");
                            await client.SendStickerAsync(user.Id, new InputFileId(update.Message.Sticker.FileId));
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
using System;
using Telegram.Bot;

namespace AnonimusBot
{
    public class Registrator
    {
        private Database database;
        private ITelegramBotClient client;
        public Registrator(ITelegramBotClient client, Database database)
        {
            this.database = database;
            this.client = client;
        }

        public async Task HandleRegistration(long senderId, string message, Server server)
        {
            if (!await database.IsUserIdInRegistration(senderId))
            {
                await client.SendTextMessageAsync(senderId, "Добро пожаловать!");
                await database.AddToRegistration(senderId);
                await client.SendTextMessageAsync(senderId, "Введите ник");
            }
            else
            {
                await CompleteUserRegistration(senderId, message, server);
            }
        }

        public async Task CompleteUserRegistration(long senderId, string nickname, Server server)
        {
            await database.AddToVerifedUser(new User(senderId, nickname, server.Name));
            await database.RemoveFromRegistration(senderId);
            await client.SendTextMessageAsync(senderId, $"Приятного общения, {nickname}!");
        }
    }
}

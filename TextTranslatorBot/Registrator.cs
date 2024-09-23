using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace AnonimusBot
{
    public class Registrator
    {
        private Database database;
        public Registrator(Database database) => this.database = database;
        public async Task HandleRegistration(ITelegramBotClient client, long senderId, string message)
        {
            if (!await database.IsUserIdInRegistration(senderId))
            {
                await client.SendTextMessageAsync(senderId, "Добро пожаловать!");
                await database.AddToRegistration(senderId);
                await client.SendTextMessageAsync(senderId, "Введите ник");
            }
            else
            {
                await CompleteUserRegistration(client, senderId, message);
            }
        }

        public async Task CompleteUserRegistration(ITelegramBotClient client, long senderId, string nickname)
        {
            await database.AddToVerifedUser(new User(senderId, nickname));
            await database.RemoveFromRegistration(senderId);
            await client.SendTextMessageAsync(senderId, $"Приятного общения, {nickname}!");


            List<User> users = await database.GetUsersAsync();

            await MessageSender.NotifyUsers(client, $"{nickname} присоединился к чату!", users);
        }
    }
}

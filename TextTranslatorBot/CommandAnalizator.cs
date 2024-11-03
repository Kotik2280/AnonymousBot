using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AnonimusBot
{
    public class CommandAnalizator
    {
        enum CommandType
        {
            Rename,
            None
        }

        Dictionary<long, CommandType> waitDictionaty;
        Database database;
        public CommandAnalizator(Database database)
        {
            this.database = database;
            waitDictionaty = new Dictionary<long, CommandType>();
        }
        public async Task AnalizeCommand(ITelegramBotClient client, Update update)
        {
            string command = update.Message.Text;

            long senderId = update.Message.Chat.Id;

            if (IsWaitForCommand(senderId))
            {
                await RunCommand(client, update);
                waitDictionaty.Remove(senderId);
                return;
            }

            switch (command)
            {
                case "/rename":
                    await this.Rename(client, update);
                    break;
                default:
                    break;
            }
        }
        private async Task Rename(ITelegramBotClient client, Update update)
        {
            long senderId = update.Message.Chat.Id;

            await LocalMessageSender.SendFromSystemToUser(client, senderId, (senderId) => "Введите новый никнейм");
            waitDictionaty[senderId] = CommandType.Rename;
        }
        public async Task RunCommand(ITelegramBotClient client, Update update)
        {
            long senderId = update.Message.Chat.Id;

            CommandType commandType = waitDictionaty[senderId];

            switch (commandType)
            {
                case CommandType.Rename:
                    string oldUsername = (await database.GetVerifedUserById(senderId)).Name;
                    string newUsername = update.Message.Text;
                    await database.SetUsername(senderId, newUsername);
                    await GlobalMessageSender.NotifyUsers(client, await database.GetUsersAsync(await database.GetServerAsync(senderId)), (user) => $"Пользователь {oldUsername} сменил никнейм на {newUsername}");
                    break;
                case CommandType.None:
                    break;
                default:
                    break;
            }
        }
        public bool IsWaitForCommand(long senderId)
        {
            return waitDictionaty.ContainsKey(senderId);
        }
    }
}

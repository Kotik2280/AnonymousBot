using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AnonimusBot
{
    public enum BotCommandType
    {
        Rename,
        Tell,
        None
    }
    public class CommandAnalizator
    {
        public delegate Task QuitDelegate(ITelegramBotClient client, long id);
        public event QuitDelegate ServerQuitEvent;
        

        Dictionary<long, TempCommand> waitDictionaty;
        Database database;
        public CommandAnalizator(Database database)
        {
            this.database = database;
            waitDictionaty = new Dictionary<long, TempCommand>();
        }
        public async Task AnalizeCommand(ITelegramBotClient client, Update update)
        {
            string command = update.Message.Text;

            long senderId = update.Message.Chat.Id;

            if (IsWaitForCommand(senderId))
            {
                await RunCommand(client, update); 
                CutCommandInputTime(senderId);
                return;
            }

            switch (command)
            {
                case "/rename":
                    await this.Rename(client, update);
                    break;
                case "/quit":
                    await this.QuitServer(client, update);
                    break;
                case "/tell":
                    await this.Tell(client, update);
                    break;
                case "/userslist":
                    await this.ShowUsersList(client, update);
                    break;
                case "/info":
                    await this.GetSelfInfo(client, update);
                    break;
                default:
                    break;
            }
        }
        private async Task Rename(ITelegramBotClient client, Update update)
        {
            long senderId = update.Message.Chat.Id;

            await LocalMessageSender.SendFromSystemToUser(client, senderId, (senderId) => "Введите новый никнейм");
            waitDictionaty[senderId] = new TempCommand(BotCommandType.Rename, 2);
        }
        private async Task Tell(ITelegramBotClient client, Update update)
        {
            long senderId = update.Message.Chat.Id;

            await LocalMessageSender.SendFromSystemToUser(client, senderId, (senderId) => "Введите ник получателя личного сообщения");
            waitDictionaty[senderId] = new TempCommand(BotCommandType.Tell, 3);
        }
        private async Task QuitServer(ITelegramBotClient client, Update update)
        {
            User sender = await database.GetVerifedUserById(update.Message.Chat.Id);

            if (sender.ServerConnectedName == "None")
            {
                await LocalMessageSender.SendFromSystemToUser(client, sender.Id, (id) => $"Команда /quit работает только на сервере, вы находитесь в главном меню!");
                return;
            }

            await GlobalMessageSender.NotifyUsersExcludingSender(client, await database.GetUsersAsync(sender.ServerConnectedName), sender.Id, (user) => $"{sender.Name} покинул сервер");

            await database.SetServerAsync(sender.Id, database.DefaultServer.Name);

            await LocalMessageSender.SendFromSystemToUser(client, sender.Id, (senderId) => "Вы находитесь в главном меню");

            await database.SetUsername(sender.Id, sender.Name.Split(' ')[0]);

            await ServerQuitEvent.Invoke(client, sender.Id);
        }
        public async Task ShowUsersList(ITelegramBotClient client, Update update)
        {
            long senderId = update.Message.Chat.Id;
            string serverName = await database.GetServerAsync(senderId);
            List<User> users = await database.GetUsersAsync(serverName);

            string message = $"На сервере {serverName} {users.Count} пользовател";

            if (users.Count == 1)
                message += "ь: ";
            else if (users.Count > 1 && users.Count < 5)
                message += "я: ";
            else
                message += "ей: ";

            for (int i = 0; i < users.Count; i++)
            {
                message += $"\n{users[i].Name}";
            }

            await LocalMessageSender.SendFromSystemToUser(client, senderId, (senderId) => message);
        }
        public async Task GetSelfInfo(ITelegramBotClient client, Update update)
        {
            User sender = await database.GetVerifedUserById(update.Message.Chat.Id);

            await LocalMessageSender.SendFromSystemToUser(client, sender.Id, (id) => $"Ваш ник - \"{sender.Name}\", вы находитесь на сервере \"{sender.ServerConnectedName}\"");
        }
        public async Task RunCommand(ITelegramBotClient client, Update update)
        {
            long senderId = update.Message.Chat.Id;

            BotCommandType commandType = waitDictionaty[senderId].CommandType;

            if (waitDictionaty[senderId].InputTime < 2)
                return;

            switch (commandType)
            {
                case BotCommandType.Rename:
                    string oldUsername = (await database.GetVerifedUserById(senderId)).Name;
                    string newUsername = update.Message.Text;
                    await database.SetUsername(senderId, newUsername);
                    await GlobalMessageSender.NotifyUsers(client, await database.GetUsersAsync(await database.GetServerAsync(senderId)), (user) => $"Пользователь {oldUsername} сменил никнейм на {newUsername}");
                    break;
                case BotCommandType.Tell:
                    if (waitDictionaty[senderId].InputTime == 3)
                    {
                        string receiverNickname = update.Message.Text;
                        waitDictionaty[senderId].ParameterList.Add(receiverNickname);

                        await LocalMessageSender.SendFromSystemToUser(client, senderId, (senderId) => "Введите текст личного сообщения");
                    }
                    if (waitDictionaty[senderId].InputTime == 2)
                    {
                        string receiverName = waitDictionaty[senderId].ParameterList[0];
                        string message = update.Message.Text;

                        User sender = await database.GetVerifedUserById(senderId);
                        User? receiver = await database.GetUserAsync(receiverName, sender.ServerConnectedName);

                        if (receiver is null)
                        {
                            await LocalMessageSender.SendFromSystemToUser(client, senderId, (senderId) => $"Пользователя с указаным именем нет на сервере");
                            return;
                        }

                        await LocalMessageSender.SendFromSystemToUser(client, senderId, (senderId) => $"Вы -> {sender.Name}: {message}");
                        await LocalMessageSender.SendFromSystemToUser(client, receiver.Id, (receiverId) => $"[Личное сообщение] {sender.Name}: {message}");
                    }
                    break;
                case BotCommandType.None:
                    break;
                default:
                    break;
            }
        }
        public bool IsWaitForCommand(long senderId)
        {
            return waitDictionaty.ContainsKey(senderId) && waitDictionaty[senderId].InputTime > 0;
        }

        public void CutCommandInputTime(long id)
        {
            waitDictionaty[id].InputTime--;
            if (waitDictionaty[id].InputTime == 0)
                waitDictionaty.Remove(id);
        }
    }
}

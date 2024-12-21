using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AnonimusBot
{
    public class NavigationMenu
    {
        Database database;
        public NavigationMenu(Database database)
        {
            this.database = database;
        }
        public async Task ShowServerChoosingMenu(ITelegramBotClient client, long id)
        {
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Сервер 1", "server1"),
                    InlineKeyboardButton.WithCallbackData("Сервер 2", "server2"),
                    InlineKeyboardButton.WithCallbackData("Сервер 3", "server3")
                }
            );

            await client.SendTextMessageAsync(
                id,
                text: "Выберите сервер",
                replyMarkup: keyboard
            );
        }
        public async Task NavigateUserToServer(ITelegramBotClient client, Update update)
        {
            if (update.Type != UpdateType.CallbackQuery)
                return;

            User sender = await database.GetVerifedUserById(update.CallbackQuery.Message.Chat.Id);

            if (sender.ServerConnectedName != "None")
            {
                await LocalMessageSender.SendFromSystemToUser(client, sender.Id, (id) => "Сначала нужно отключится от сервера. Используйте команду /quit");
                return;
            }

            CallbackQuery callbackQuery = update.CallbackQuery;

            string server = callbackQuery.Data;

            List<string> userList = (from user in (await database.GetUsersAsync(server))
                                  select user.Name).ToList();
            if (userList.Contains(sender.Name))
            {
                if (sender.Name.Contains(' '))
                {
                    sender.Name = $"{sender.Name.Split(' ')[0]} {Convert.ToInt32(sender.Name.Split(' ')[0]) + 1}";
                }
                else
                {
                    sender.Name = sender.Name + " 1";
                }
            }
            await database.SetUsername(sender.Id, sender.Name);

            await database.SetServerAsync(sender.Id, server);

            await GlobalMessageSender.NotifyUsers(client, await database.GetUsersAsync(server), (user) => $"{sender.Name} подключился к серверу!");
            await LocalMessageSender.SendFromSystemToUser(client, sender.Id, (user) => $"Для того, чтобы покинуть сервер пропишите /quit");
        }
    }
}

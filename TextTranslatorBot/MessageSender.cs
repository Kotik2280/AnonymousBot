using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AnonimusBot
{
    static public class MessageSender
    {
        static public async Task NotifyUsers(ITelegramBotClient client, string message, IEnumerable<User> receivers)
        {
            foreach (User user in receivers)
            {
                await client.SendTextMessageAsync(user.Id, message);
            }
        }
        static public async Task NotifyTextUsersExcludingSender(ITelegramBotClient client, string message, IEnumerable<User> receivers, User sender)
        {
            foreach (User user in receivers)
            {
                if (user.Id == sender.Id)
                    continue;
                await client.SendTextMessageAsync(user.Id, $"{sender.Name}: {message}");
            }
        }
        static public async Task NotifyStickerUsersExcludingSender(ITelegramBotClient client, Sticker sticker, IEnumerable<User> receivers, User sender)
        {
            foreach (User user in receivers)
            {
                if (user.Id == sender.Id)
                    continue;
                await client.SendTextMessageAsync(user.Id, $"{sender.Name} :");
                await client.SendStickerAsync(user.Id, new InputFileId(sticker.FileId));
            }
        }
    }
}

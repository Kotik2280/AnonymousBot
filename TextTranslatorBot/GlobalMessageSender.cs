﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AnonimusBot
{
    static public class GlobalMessageSender
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
        static public async Task HandleSendMessage(ITelegramBotClient client, User sender, Message message, IEnumerable<User> users)
        {
            switch (message.Type)
            {
                case MessageType.Text:
                    await GlobalMessageSender.NotifyTextUsersExcludingSender(client, message.Text, users, sender);
                    break;
                case MessageType.Sticker:
                    await GlobalMessageSender.NotifyStickerUsersExcludingSender(client, message.Sticker, users, sender);
                    break;
                default:
                    break;
            }
        }
    }
}
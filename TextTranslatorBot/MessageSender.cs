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
        static public async Task SendAsync(ITelegramBotClient client, Message message, long receiverId, User sender)
        {
            MessageType messageType;

            if (message is null)
                return;

            messageType = message.GetMessageType();

            switch (messageType)
            {
                case MessageType.Text:
                    await client.SendTextMessageAsync(receiverId, $"{sender.Name} : {message.Text}");
                    break;
                case MessageType.Sticker:
                    {
                        await client.SendTextMessageAsync(receiverId, $"{sender.Name} :");
                        await client.SendStickerAsync(receiverId, new InputFileId(message.Sticker.FileId));
                    }
                    break;
                default:
                    break;
            }
        }

        static public async Task SendAsync(ITelegramBotClient client, Message message, long receiverId, User sender, ReplyKeyboardMarkup replyKeyboard)
        {
            MessageType messageType;

            if (message is null || message.GetMessageType() != MessageType.Text)
                return;

            await client.SendTextMessageAsync(receiverId, $"{sender.Name} : {message.Text}", replyMarkup: replyKeyboard);
        }

        static public async Task SendManyAsync(ITelegramBotClient client, Message message, IEnumerable<User> receivers, User sender)
        {
            MessageType messageType;

            if (message is null)
                return;

            messageType = message.GetMessageType();

            switch (messageType)
            {
                case MessageType.Text:
                    foreach (User user in receivers)
                    {
                        if (user.Id == sender.Id)
                            continue;
                        await client.SendTextMessageAsync(user.Id, $"{sender.Name} : {message.Text}");
                    }
                    break;
                case MessageType.Sticker:
                    foreach (User user in receivers)
                    {
                        if (user.Id == sender.Id)
                            continue;
                        await client.SendTextMessageAsync(user.Id, $"{sender.Name} :");
                        await client.SendStickerAsync(user.Id, new InputFileId(message.Sticker.FileId));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

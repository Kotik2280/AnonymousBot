using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace AnonimusBot
{
    static public class LocalMessageSender
    {
        static public async Task SendFromSystemToUser(ITelegramBotClient client, long receiverId, Func<long, string> messageMethod)
        {
            await client.SendTextMessageAsync(receiverId, messageMethod.Invoke(receiverId));
        }

        static public async Task SendFromUserToUser(ITelegramBotClient client, long receiverId, long senderId, Func<long, long, string> messageMethod)
        {
            await client.SendTextMessageAsync(receiverId, messageMethod.Invoke(receiverId, senderId));
        }
    }
}

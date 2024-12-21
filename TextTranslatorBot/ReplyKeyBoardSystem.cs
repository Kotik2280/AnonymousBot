using Telegram.Bot;

namespace AnonimusBot
{
    public class ReplyKeyBoardSystem
    {
        private ITelegramBotClient client;
        public ReplyKeyBoardSystem(ITelegramBotClient client)
        {
            this.client = client;
        }
        /*
            Будет использоваться вместо текстовых команд
        */
    }
}

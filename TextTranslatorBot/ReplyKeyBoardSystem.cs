using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

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
        public void ShowButtons(params string[] buttonsText, long id)
        {
            KeyboardButton[] keyboards = new KeyboardButton[buttonsText.Length];

            for (int i = 0; i < buttonsText.Length; i++)
            {
                keyboards[i] = new KeyboardButton(buttonsText[i]);
            }

            var replyMarkUp = new ReplyKeyboardMarkup(keyboards) { ResizeKeyboard = true };


        }*/
    }
}

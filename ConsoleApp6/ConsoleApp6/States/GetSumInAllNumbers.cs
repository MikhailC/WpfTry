using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp6.States
{
    public class GetSumInAllNumbers:StartState
    {
        public GetSumInAllNumbers(Chat chat) : base(chat)
        {
            
        }

        protected override void InitStates()
        {
            base.InitStates();
            AddNextState(x => true, new StartState(CurrentChat));
        }

        protected override async Task Execute(ITelegramBotClient bot, Update message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            int sum = 0;
            char delimiter = ' ';
             foreach (var v in CurrentChat.Data)
             {
                sum += (int) v;
                stringBuilder.Append(delimiter);
                stringBuilder.Append(v);
                delimiter = '+';

             }
            
           await bot.SendTextMessageAsync(CurrentChat.Id, $"{stringBuilder.ToString()} = {sum}");
        }
    }
}
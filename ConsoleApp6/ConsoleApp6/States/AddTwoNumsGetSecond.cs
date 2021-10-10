using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp6.States
{
    public class AddTwoNumsGetSecond:StartState
    {
        
        public AddTwoNumsGetSecond(Chat chat) : base(chat)
        {
            this.AddNextState(
                x => x.Message != null && x.Type == UpdateType.Message && x.Message.Text!.Equals("/add"), new AddTwoNumsGetFirst(chat));

        }

        protected override void InitStates()
        {
            base.InitStates();
        }

        protected override async Task Execute(ITelegramBotClient bot, Update message)
        {
            await bot.SendTextMessageAsync(CurrentChat.Id, "Enter second number");
        }

  
    }
}
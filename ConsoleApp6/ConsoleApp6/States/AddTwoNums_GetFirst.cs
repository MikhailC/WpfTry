using System;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp6.States
{
    public class AddTwoNumsGetFirst:StartState
    {
        int _cnt = 1;
        
        public AddTwoNumsGetFirst(Chat chat) : base(chat)
        {
         


        }



        protected override void InitStates()
        {
            base.InitStates();
            AddNextState(
                x => x.Message != null && 
                                 x.Message.Text != null &&
                                 x.Type ==  UpdateType.Message && 
                                 int.TryParse(x.Message!.Text, out _),
                this);
            
            AddNextState(
                x => x.Message != null 
                                 && x.Message.Text!=null
                                 && x.Type == UpdateType.Message 
                                 && !int.TryParse(x.Message.Text, out _),
                new GetSumInAllNumbers(CurrentChat));
        }

        protected override async Task Execute(ITelegramBotClient bot, Update message)
        {
      
            await bot.SendTextMessageAsync(CurrentChat.Id, $"Enter {_cnt++} number");
        }
        
        protected override  void OnBeforeExecuteNewState(StartState newState,Update message)
        {
            if(newState is AddTwoNumsGetFirst)
            CurrentChat.Data.Push(int.Parse(message.Message!.Text!));
            
            
            
        }
    }
}
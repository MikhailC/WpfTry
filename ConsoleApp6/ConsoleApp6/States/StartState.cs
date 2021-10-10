using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp6.States
{
    public class StartState
    {
        protected readonly Chat CurrentChat = null;
        
        public StartState(Chat chat)
        {
            CurrentChat = chat;
            
             }

        private  Dictionary<Func<Update, bool>, StartState> _nextStates = null;

        protected virtual void InitStates()
        {
            _nextStates = new Dictionary<Func<Update, bool>, StartState>();
            
                //Default commands for all possible states
                AddNextState(
                    x => x.Message != null && 
                                     x.Message.Text != null && 
                                     x.Type ==  UpdateType.Message && 
                                     x.Message.Text.Equals("/start"),
                    new StartState(CurrentChat));
                AddNextState(
                    x => x.Message != null  
                         &&x.Message.Text is not null 
                         &&x.Type == UpdateType.Message  
                         &&x.Message.Text!.Equals("/add"), 
                    new AddTwoNumsGetFirst(CurrentChat));
                AddNextState(
                    x => x.Message != null 
                         && x.Message.Text is not null 
                         &&x.Type == UpdateType.Message 
                         && x.Message.Text!.Equals("/listfiles"), 
                    new ListFiles(CurrentChat));
                
                AddNextState(x => x.Message is not null
                                  &&x.Message.Text is  null 
                                  && x.Type == UpdateType.Message 
                                  && x.Message.Document is not null,
                    new GetFile(CurrentChat));

                AddNextState(x => x.Message!=null
                                  && (x.Message.Voice is not null ),
                    new GetVoice(CurrentChat));
                
                AddNextState(x =>  x.Message is not null
                                  &&  x.Message.Photo is not null,
                    new GetPic(CurrentChat));
        } 

        //Добавляем возможные переходы
        protected StartState AddNextState(Func<Update,bool> action, StartState state)
        {
            _nextStates.Add(action, state);
            return this;
        }

        public StartState FindNextState(Update message)=>
        
        _nextStates.FirstOrDefault(stateCondition => stateCondition.Key.Invoke(message)).Value ?? this;

        public async Task<StartState> DoAction(ITelegramBotClient bot, Update message)
        {
            //Lazy init states
            if (_nextStates is null)
            {
                InitStates();
            }
            
            CurrentChat.State = FindNextState(message);
            try
            {
                OnBeforeExecuteNewState(CurrentChat.State, message);
                await CurrentChat.State.Execute(bot, message);
            }
            catch(Exception e)
            {
                await bot.SendTextMessageAsync(CurrentChat.Id, $"Something went wrong. Try again. Error {e.Message}",
                    replyMarkup: new ReplyKeyboardRemove());
            }
    
            return CurrentChat.State;
        }

        protected virtual async Task Execute(ITelegramBotClient bot, Update message)
        {
            string ins = "You're using a test bot. You can use next commands:\n" +
                         "/start - this message\n" +
                         "/add - summarize many numbers\n" +
                         "/listfiles - list files in dir\n" +
                         "or simple send a file to save in local bot dir\n" +
                         "to Exit from any state send /start command in any brunch";
                         
                
            await bot.SendTextMessageAsync(CurrentChat.Id, ins, replyMarkup:new ReplyKeyboardRemove());
        }


        protected virtual void OnBeforeExecuteNewState(StartState newState, Update message)
        {
            //Nothing doing 
        }

    }
}
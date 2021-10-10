using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp6.States;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp6
{
    public class Chat
    {
        public static Dictionary<long, Chat> Chats = new Dictionary<long, Chat>();

        public delegate void NewMessageHandler(Update update); 
        

        public static event NewMessageHandler OnNewMessage;
        public static event NewMessageHandler OnNewChat;

        public static CancellationTokenSource cts;

        public static Telegram.Bot.ITelegramBotClient botClient;
     
        public static async Task StartBot(string botId)
        {
            botClient = new TelegramBotClient("1979348960:AAEHwE6b3bVjCbZoeS1n4YbxtWMpgeuTJDY");
            var me = await botClient.GetMeAsync();
            Debug.WriteLine(
                $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            cts = new CancellationTokenSource();

            botClient.StartReceiving(UpdateHandler, ExceptionHandler, cancellationToken: cts.Token);

            await botClient.GetUpdatesAsync(cancellationToken: cts.Token);

        }

        ~Chat(){
            cts.Cancel();
        }

        private static Task ExceptionHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine($"Something happend {arg2.Message}");
            return null;
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            //Console.WriteLine(update.Type);

            await Chat.GetChat(update).State.DoAction(botClient, update);



        }

        public static Chat GetChat(Update update)
        {

            long id = update.Type switch
            {
                UpdateType.Message => update.Message!.Chat.Id,
                UpdateType.CallbackQuery =>update.CallbackQuery.Message.Chat.Id,
                _ => default
            };
            
            
            var value = Chats.FirstOrDefault(x => x.Key == id).Value;
            
            if (value is null)
            {
                Chat newChat = new Chat(id);
                Chats.Add(id, newChat);
                value = newChat;

                OnNewChat?.Invoke(update);
            }
            OnNewMessage?.Invoke(update);
            //Добавим в коллекцию чатов информацию об сообщении
            return value;
        }

        public Stack Data = new Stack();


        public StartState State { get; set; }

        public ITelegramBotClient bot;


        public Chat(long chatId)
        {
            this.Id = chatId;
            State = new StartState(this);
  
        }

        public ChatId Id
        {
            get; set;
        }
        
        
    }
}
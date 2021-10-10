using ConsoleApp6;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WpfApp3.Helpers;
using Telegram.Bot;
using Newtonsoft.Json;
using System.IO;
//using Telegram.Bot.Types;

namespace WpfApp3.ViewModel
{
    public class MainWindowsViewModel : ObservableObject
    {
        Dispatcher  _dispatcher = Dispatcher.CurrentDispatcher;

      //  ObservableCollection<Telegram.Bot.Types.Message> _messages;

        public ObservableCollection<Telegram.Bot.Types.Chat> Chats { get; private set; } = new ObservableCollection<Telegram.Bot.Types.Chat>();

        private Dictionary<long?, ObservableCollection<Telegram.Bot.Types.Message>> messages { get;  set; } = new Dictionary<long?, ObservableCollection<Telegram.Bot.Types.Message>>();

        Task chbot;

        
        private Telegram.Bot.Types.Chat? _currentChat = null;
      

        

        public Telegram.Bot.Types.Chat? CurrentChat { 
            get => _currentChat; 
            set
            {
                 
                _currentChat = value;
               RaisePropertyChangedEvent("Messages");
            } 
        }

        public ObservableCollection<Telegram.Bot.Types.Message> Messages => CurrentChat is not null? messages[CurrentChat?.Id]:new ObservableCollection<Telegram.Bot.Types.Message>();
            
 

        public Telegram.Bot.Types.Message? CurrentMessage { get; set; }

        public String ReplyMessage { get; set; } = "";

        public ICommand ReconnectToBot => new DelegateCommand(_ => chbot = Chat.StartBot("1979348960:AAEHwE6b3bVjCbZoeS1n4YbxtWMpgeuTJDY"),
            _ => chbot is null || chbot.IsFaulted);

        public ICommand SendMessage => new DelegateCommand
                (_ => Chat.botClient.SendTextMessageAsync(CurrentChat.Id, ReplyMessage, replyToMessageId: CurrentMessage?.MessageId),
                _ => CurrentChat is not null && CurrentMessage is not null
                );



        public ICommand ExitApp => new DelegateCommand(_ => App.Current.Shutdown());


        public ICommand LoadFromDisk => new DelegateCommand(
            _ =>
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                using var sr = new StreamReader("chatdata.json");
                using var jr = new JsonTextReader(sr);
                messages = jsonSerializer.Deserialize<Dictionary<long?, ObservableCollection<Telegram.Bot.Types.Message>>>(jr)!;

                using var sw1 = new StreamReader("chats.json");
                using var jw1 = new JsonTextReader(sw1);
                Chats = jsonSerializer.Deserialize < ObservableCollection < Telegram.Bot.Types.Chat >> (jw1)!;

               
                RaisePropertyChangedEvent("Chats");
                CurrentChat = Chats.FirstOrDefault();
                CurrentMessage = Messages.LastOrDefault();
            });

        public ICommand SaveToDisk => new DelegateCommand(
            a =>
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                using var sw = new StreamWriter("chatdata.json");
                using var jw = new JsonTextWriter(sw);
                jsonSerializer.Serialize(jw, messages);

                using var sw1 = new StreamWriter("chats.json");
                using var jw1 = new JsonTextWriter(sw1);
                jsonSerializer.Serialize(jw1, Chats);

                
            },
            a => true);



        public MainWindowsViewModel()
        {
            Chat.OnNewChat += NewChatPlaced;
            Chat.OnNewMessage += NewMessagePlaced;
            ReconnectToBot.Execute(null);

           // chbot = Chat.StartBot("1979348960:AAEHwE6b3bVjCbZoeS1n4YbxtWMpgeuTJDY");


           
          
        }

        private void NewMessagePlaced(Telegram.Bot.Types.Update update)
        {
            //Принимаем только сообщения
            if (update.Message is null || update.Message.Chat is null)
            {
                return;
            }
            if(!messages.ContainsKey(update.Message?.Chat!.Id))
            {
                messages.Add(update.Message?.Chat.Id, new ObservableCollection<Telegram.Bot.Types.Message>());
            }

            _dispatcher.Invoke(new Action(()=>messages[update.Message!.Chat.Id].Add(update.Message)));

            

        }

        private void NewChatPlaced(Telegram.Bot.Types.Update update)
        {
             if (update is not null && update.Message is not null)
                 _dispatcher.Invoke(new Action(() => Chats.Add(update!.Message!.Chat)));
           
        }


    }
    
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace ConsoleApp6.States
{
    public class ListFiles:StartState
    {
        public ListFiles(Chat chat) : base(chat)
        {
        }

        protected override void InitStates()
        {
            base.InitStates();
            AddNextState(x => x.Message is not null
                                &&x.Message.Text is not null 
                              && File.Exists($"files/{x.Message.Text.Trim()}"), new UploadDocument(CurrentChat));
            AddNextState(x => x.CallbackQuery is not null 
                              &&File.Exists($"files/{x.CallbackQuery.Data.Trim()}"), new UploadDocument(CurrentChat));

            AddNextState(x => true, new StartState(CurrentChat));
        }

        protected override async Task Execute(ITelegramBotClient bot, Update message)
        {
            var sw = new StringWriter();

            if (Directory.Exists("files"))
            {


                await sw.WriteLineAsync("Files in directory:");
                //await bot.SendTextMessageAsync(CurrentChat.Id, "directory files:");
                //List<KeyboardButton> btns = new List<KeyboardButton>();
                List<List<InlineKeyboardButton>> btns = new List<List<InlineKeyboardButton>>();
                foreach (var file in Directory.GetFiles("files"))
                {
                    var fi = new FileInfo(file);
                    await sw.WriteLineAsync($"{fi.Name} Size: {fi.Length}");
                    btns.Add( new List<InlineKeyboardButton>{new InlineKeyboardButton(fi.Name){CallbackData = fi.Name}});
                }

                var inlineKeyboard = new InlineKeyboardMarkup(btns)
                {
                    
                };
                // {
                //     ResizeKeyboard = true
                // };
                
                

                await sw.WriteLineAsync("Enter file name to get the file to chat or /start to cancel");

                await bot.SendTextMessageAsync(CurrentChat.Id, sw.ToString(), replyMarkup:inlineKeyboard);
            }
            else
            {
                await bot.SendTextMessageAsync(CurrentChat.Id, "Upload files first. Files not found");
            }

        }
    }
}
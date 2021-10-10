using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;

namespace ConsoleApp6.States
{
    public class UploadDocument:StartState
    {
        public UploadDocument(Chat chat) : base(chat)
        {
        }

        protected override void InitStates()
        {
            base.InitStates();
            
            //Всегда возвращаемся к этому статусу, если не старт
            AddNextState(x => true, this);
        }

        protected override async Task Execute(ITelegramBotClient bot, Update message)
        {
            string shortFileName = message.CallbackQuery is null
                ? message.Message.Text.Trim()
                : message.CallbackQuery.Data.Trim();
            string fileName = $"files/{shortFileName}";
            
            
            if (File.Exists(fileName))
            {
                string extension = fileName.Substring(fileName.Length - 3) ;
                var iof = new InputOnlineFile(new FileStream(fileName, FileMode.Open));
                if (extension== "pic")
                {
                    await bot.SendPhotoAsync(
                        CurrentChat.Id, 
                        iof, 
                        shortFileName);
                    
                }
                else if (extension == "ogg")
                {
                    await bot.SendVoiceAsync(CurrentChat.Id, iof, shortFileName);
                }else
                {
                    await bot.SendDocumentAsync(CurrentChat.Id,
                              iof,
                              caption:shortFileName);
                }
            }
            else
            {
                await bot.SendTextMessageAsync(CurrentChat.Id, "File not found. Please, try again or /start to finish");
            }
        }
    }
}
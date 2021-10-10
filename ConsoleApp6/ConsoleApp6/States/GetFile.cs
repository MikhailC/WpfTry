using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace ConsoleApp6.States
{
    public class GetFile:StartState
    {
        
        
        public GetFile(Chat chat) : base(chat)
        {
           
        }

        protected override async Task Execute(ITelegramBotClient bot, Update message)
        {
            if (!Directory.Exists("files"))
            {
                Directory.CreateDirectory("files");
            }

            await using FileStream fs = new FileStream($"files/{message.Message.Document.FileName}", FileMode.Create);

            
             await  bot.GetInfoAndDownloadFileAsync(message.Message.Document.FileId, fs,
                 cancellationToken: CancellationToken.None);
             
             // fs.Flush(true);
             // fs.Close();

             await bot.SendTextMessageAsync(CurrentChat.Id, $"File {message.Message.Document.FileName} saved");
           
        }
    }
}
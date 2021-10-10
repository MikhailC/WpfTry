using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp6.States
{
    public class GetVoice:StartState
    {
        public GetVoice(Chat chat) : base(chat)
        {
            
        }

        protected override async Task Execute(ITelegramBotClient bot, Update message)
        {
            
            if (!Directory.Exists("files"))
            {
                Directory.CreateDirectory("files");
            }

            string filename = $"files/{DateTime.Now:yyyyMMddHHmm}_{message.Id}.ogg";

            await using FileStream fs = new FileStream(filename, FileMode.Create);

            await  bot.GetInfoAndDownloadFileAsync(message.Message.Voice.FileId, fs,
                cancellationToken: CancellationToken.None);
             
            // fs.Flush(true);
            // fs.Close();

            await bot.SendTextMessageAsync(CurrentChat.Id, $"File {filename} saved");
            //await bot.SendTextMessageAsync(CurrentChat.Id, "Send me file name to save you file");
        }
    }
}
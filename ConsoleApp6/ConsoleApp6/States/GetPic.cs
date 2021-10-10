using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp6.States
{
    public class GetPic:StartState
    {
        public GetPic(Chat chat) : base(chat)
        {
        }

        protected override async Task Execute(ITelegramBotClient bot, Update message)
        {  
            await base.Execute(bot, message);
              
            if (!Directory.Exists("files"))
            {
                Directory.CreateDirectory("files");
            }

            int i = 0;
            foreach (var photo in message.Message.Photo)
            {
                string filename = $"files/{DateTime.Now:yyyyMMddHHmm}_{message.Id}_{photo.Width}x{photo.Height}.pic";
                await using FileStream fs = new FileStream(filename, FileMode.Create);

                await  bot.GetInfoAndDownloadFileAsync(photo.FileId, fs,
                    cancellationToken: CancellationToken.None);


            }

        }
    }
}
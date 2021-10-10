using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace ConsoleApp6
{
    class Program
    {


        static async Task Main(string[] args)
        {
            await Chat.StartBot("1979348960:AAEHwE6b3bVjCbZoeS1n4YbxtWMpgeuTJDY");

            Console.ReadLine();

  

        }

 
    }
}

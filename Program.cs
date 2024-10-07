using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    static string[] quotes = new[]
    {
        "Однажды городской тип купил поселок. Теперь это поселок городского типа.",
        "Жи-ши пиши от души.",
        "Если тебе где-то не рады в рваных носках, то и в целых туда идти не стоит.",
        "Запомни: Всего одна ошибка и ты ошибся.",
        "Позвоночник знаешь? Я позвонил.",
        "Когда меня рожали, собственно, тогда я и родился.",
        "Заблудился в лесу - иди домой.",
        "Работа не волк. Никто не волк. Только волк - волк.",
        "Запомни, а то забудешь.",
        "Порхай как бабочка, жаль, что мужик без жены, как рыба без велосипеда.",
        "Если тебя обидели не заслуженно, то вернись и заслужи.",
        "Если вы в чем-то не разбираетесь, начните разбираться и вы разберётесь.",
        "Соленое море знаешь? Я посолил.",
        "Не надо париться по пустякам. Париться надо в бане.",
        "Не упускай возможности упустить возможность.",
        "Я скажу тебе две фразы, которые откроют любые двери: От себя и на себя.",
        "Если закрыть глаза становится темно"
    };

    private static ITelegramBotClient botClient;

    static void Main(string[] args)
    {
        botClient = new TelegramBotClient("7673062142:AAGNKr7Kux6ssRwTJtF8sSX8q9C1gCDg-H4");

        var cts = new CancellationTokenSource(); // Токен для управления процессом получения обновлений
        var cancellationToken = cts.Token; // Сам токен
        var receiverOptions = new ReceiverOptions 
        {
            AllowedUpdates = Array.Empty<UpdateType>() // Получать все типы апдейтов
        };
        
        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        Console.WriteLine("Бот запущен...");
        Console.ReadLine();
    }

    // Обработка входящих сообщений
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message.Text != null)
        {
            var message = update.Message;
            Console.WriteLine($"Получено сообщение от {message.Chat.Id}: {message.Text}");

            // Проверка на наличие фразы "выдай цитату" в сообщении
            if (message.Text.ToLower().Contains("выдай цитату"))
            {
                // Генерируем случайную цитату
                var random = new Random();
                int index = random.Next(quotes.Length);
                string quote = quotes[index];

                // Отправляем цитату пользователю
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"Цитата Джейсона Стетхема:\n\n\"{quote}\"",
                    cancellationToken: cancellationToken
                );
            }
            else
            {
                // Можно отправить другое сообщение, если не указано "выдай цитату"
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Чтобы получить цитату, напишите 'выдай цитату'.",
                    cancellationToken: cancellationToken
                );
            }
        }
    }

    // Обработка ошибок
    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}

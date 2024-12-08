using Models;
using System.Text;

namespace WebApplicationProducer
{
    public class MessageGenerator
    {
        private readonly KafkaProducer _kafkaProducer;

        public MessageGenerator(KafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        /// <summary>
        /// Генерация и отправка сообщений в Kafka.
        /// </summary>
        /// <param name="request">Условия генерации сообщений</param>
        /// <returns></returns>
        public async Task GenerateMessagesAsync(MessageGenerationRequest request)
        {
            var tasks = new List<Task>();
            var totalMessages = request.MessageCount;
            var sequenceNumber = 0; // Глобальный атомарный счетчик

            for (int threadId = 0; threadId < request.ThreadCount; threadId++)
            {
                int threadNumber = threadId + 1; // Для удобства поток нумеруется с 1
                tasks.Add(Task.Run(async () =>
                {
                    var random = new Random();
                    while (true)
                    {
                        int currentSequenceNumber;
                        // Потокобезопасно увеличиваем счетчик
                        lock (this)
                        {
                            if (sequenceNumber >= totalMessages) break;
                            currentSequenceNumber = ++sequenceNumber;
                        }

                        // Генерация случайного содержимого сообщения
                        var messageSize = random.Next(request.MinMessageSize, request.MaxMessageSize + 1);
                        var messageContent = GenerateRandomMessageContent(messageSize);

                        // Создание KafkaMessage
                        var kafkaMessage = new KafkaMessage
                        {
                            ThreadId = threadNumber,
                            Timestamp = DateTime.UtcNow,
                            SequenceNumber = currentSequenceNumber,
                            MessageContent = messageContent
                        };

                        // Сериализация сообщения в JSON
                        var serializedMessage = SerializeMessage(kafkaMessage);

                        // Отправка сообщения в Kafka
                        await _kafkaProducer.ProduceAsync(serializedMessage);

                        // Задержка между сообщениями
                        await Task.Delay(request.DelayMs);
                    }
                }));
            }

            // Дождаться завершения всех задач
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Генерация случайного содержимого сообщения заданного размера.
        /// </summary>
        /// <param name="size">Размер содержимого в байтах</param>
        /// <returns>Случайное содержимое сообщения</returns>
        private string GenerateRandomMessageContent(int size)
        {
            var random = new Random();
            var builder = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                builder.Append((char)random.Next(32, 127)); // Генерация символов ASCII
            }
            return builder.ToString();
        }

        /// <summary>
        /// Сериализация KafkaMessage в строку JSON.
        /// </summary>
        /// <param name="message">Объект KafkaMessage</param>
        /// <returns>Строка в формате JSON</returns>
        private string SerializeMessage(KafkaMessage message)
        {
            return System.Text.Json.JsonSerializer.Serialize(message);
        }
    }
}

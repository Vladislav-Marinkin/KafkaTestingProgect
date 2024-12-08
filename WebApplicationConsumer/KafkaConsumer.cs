namespace WebApplicationConsumer
{
    using System.Text.Json;
    using Confluent.Kafka;
    using ClickHouse.Client.ADO;
    using Models;
    using System.Data.Common;

    public class KafkaConsumer
    {
        private readonly string _topic;
        private readonly IConsumer<Null, string> _consumer;
        private readonly string _clickhouseConnectionString;

        public KafkaConsumer(string bootstrapServers, string groupId, string topic, string clickhouseConnectionString)
        {
            _topic = topic;
            _clickhouseConnectionString = clickhouseConnectionString;

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Null, string>(config).Build();
        }

        public void Consume(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topic);
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(cancellationToken);

                    try
                    {
                        // Десериализация сообщения
                        var kafkaMessage = JsonSerializer.Deserialize<KafkaMessage>(result.Message.Value);
                        if (kafkaMessage != null)
                        {
                            // Сохранение сообщения в ClickHouse
                            SaveMessageToClickHouse(kafkaMessage);
                            Console.WriteLine($"Message saved to ClickHouse: Thread {kafkaMessage.ThreadId}, Seq {kafkaMessage.SequenceNumber}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                _consumer.Close();
            }
        }

        private void SaveMessageToClickHouse(KafkaMessage message)
        {
            using var connection = new ClickHouseConnection(_clickhouseConnectionString);
            connection.Open();

            // SQL для вставки данных
            var insertQuery = @"
                INSERT INTO kafka_messages (ThreadId, Timestamp, SequenceNumber, MessageContent, MessageSizeKb)
                VALUES (@ThreadId, @Timestamp, @SequenceNumber, @MessageContent, @MessageSizeKb)";

            using var cmd = connection.CreateCommand();
            cmd.CommandText = insertQuery;

            // Добавление параметров
            AddParameter(cmd, "ThreadId", message.ThreadId);
            AddParameter(cmd, "Timestamp", message.Timestamp);
            AddParameter(cmd, "SequenceNumber", message.SequenceNumber);
            AddParameter(cmd, "MessageContent", message.MessageContent);
            AddParameter(cmd, "MessageSizeKb", message.MessageSizeKb);

            // Выполнение команды
            cmd.ExecuteNonQuery();
        }

        private void AddParameter(DbCommand cmd, string name, object value)
        {
            cmd.Parameters.Add(new ClickHouse.Client.ADO.Parameters.ClickHouseDbParameter
            {
                ParameterName = name,
                Value = value
            });
        }
    }
}

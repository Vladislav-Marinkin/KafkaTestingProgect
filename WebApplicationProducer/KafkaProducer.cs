using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace WebApplicationProducer
{
    public class KafkaProducer
    {
        private readonly string _topic;
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;

        public KafkaProducer(string bootstrapServers, string topic, ILogger<KafkaProducer> logger)
        {
            _topic = topic;
            _logger = logger;
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task ProduceAsync(string message)
        {
            try
            {
                _logger.LogInformation("Попытка отправить сообщение в Kafka-топик '{Topic}': {Message}", _topic, message);

                var deliveryResult = await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });

                _logger.LogInformation("Сообщение успешно отправлено в топик '{Topic}' с оффсетом {Offset}.",
                    _topic, deliveryResult.Offset);
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex, "Ошибка при отправке сообщения в Kafka-топик '{Topic}': {Message}", _topic, message);
                throw;
            }
        }
    }
}

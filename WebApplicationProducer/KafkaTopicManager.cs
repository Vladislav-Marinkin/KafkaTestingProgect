namespace WebApplicationProducer
{
    using Confluent.Kafka;
    using Confluent.Kafka.Admin;

    public class KafkaTopicManager
    {
        private readonly string _bootstrapServers;
        private readonly ILogger<KafkaTopicManager> _logger;

        public KafkaTopicManager(string bootstrapServers, ILogger<KafkaTopicManager> logger)
        {
            _bootstrapServers = bootstrapServers;
            _logger = logger;
        }

        public async Task CreateTopicAsync(string topicName, int numPartitions, short replicationFactor)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _bootstrapServers }).Build();

            try
            {
                _logger.LogInformation("Попытка создания топика '{Topic}' с {Partitions} партициями.", topicName, numPartitions);

                var topicSpecification = new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor
                };

                await adminClient.CreateTopicsAsync(new[] { topicSpecification });

                _logger.LogInformation("Топик '{Topic}' успешно создан.", topicName);
            }
            catch (CreateTopicsException ex)
            {
                if (ex.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
                {
                    _logger.LogWarning("Топик '{Topic}' уже существует.", topicName);
                }
                else
                {
                    _logger.LogError(ex, "Ошибка при создании топика '{Topic}'.", topicName);
                    throw;
                }
            }
        }
    }

}

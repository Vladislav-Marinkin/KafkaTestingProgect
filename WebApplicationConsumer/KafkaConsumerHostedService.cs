namespace WebApplicationConsumer
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public class KafkaConsumerHostedService : BackgroundService
    {
        private readonly KafkaConsumer _consumer;

        public KafkaConsumerHostedService(KafkaConsumer consumer)
        {
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() => _consumer.Consume(stoppingToken), stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Выполняем корректное завершение работы
            await base.StopAsync(cancellationToken);
        }
    }

}

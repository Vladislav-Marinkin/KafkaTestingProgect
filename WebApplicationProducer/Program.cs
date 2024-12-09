using WebApplicationProducer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Конфигурация KafkaProducer
builder.Services.AddSingleton(provider =>
{
    var kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"];
    var kafkaTopic = builder.Configuration["Kafka:Topic"];
    var logger = provider.GetRequiredService<ILogger<KafkaProducer>>();
    return new KafkaProducer(kafkaBootstrapServers, kafkaTopic, logger);
});

// Регистрация MessageGenerator
builder.Services.AddSingleton<MessageGenerator>();

var app = builder.Build();

// Создание топика
var topicManager = app.Services.GetRequiredService<KafkaTopicManager>();
await topicManager.CreateTopicAsync(builder.Configuration["Kafka:Topic"], numPartitions:10, replicationFactor: 1);

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

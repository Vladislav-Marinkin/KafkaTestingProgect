using System;
using WebApplicationProducer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ������������ KafkaProducer
builder.Services.AddSingleton(provider =>
{
    var kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"];
    var kafkaTopic = builder.Configuration["Kafka:Topic"];
    var logger = provider.GetRequiredService<ILogger<KafkaProducer>>();
    return new KafkaProducer(kafkaBootstrapServers, kafkaTopic, logger);
});

// ������������ KafkaTopicManager
builder.Services.AddSingleton(provider =>
{
    var kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"];
    var logger = provider.GetRequiredService<ILogger<KafkaTopicManager>>();
    return new KafkaTopicManager(kafkaBootstrapServers, logger);
});

// ����������� MessageGenerator
builder.Services.AddSingleton<MessageGenerator>();

var app = builder.Build();

// �������� ������
var topicManager = app.Services.GetRequiredService<KafkaTopicManager>();
await topicManager.CreateTopicAsync(builder.Configuration["Kafka:Topic"], numPartitions:10, replicationFactor: 1);

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

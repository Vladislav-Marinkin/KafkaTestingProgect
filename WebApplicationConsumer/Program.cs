using ClickHouse.Client.ADO;
using WebApplicationConsumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Конфигурация KafkaConsumer
builder.Services.AddSingleton(provider =>
{
    var environment = builder.Environment;
    var kafkaBootstrapServers = environment.IsDevelopment()
        ? builder.Configuration["Kafka:BootstrapServers"]
        : builder.Configuration["Kafka:BootstrapServersProd"];
    var kafkaTopic = builder.Configuration["Kafka:Topic"];
    var kafkaGroupId = builder.Configuration["Kafka:GroupId"];
    var clickhouseConnectionString = builder.Configuration["ClickHouse:ConnectionString"];
    return new KafkaConsumer(kafkaBootstrapServers, kafkaGroupId, kafkaTopic, clickhouseConnectionString);
});

// Настройка хоста для корректной остановки KafkaConsumer
builder.Services.AddHostedService<KafkaConsumerHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

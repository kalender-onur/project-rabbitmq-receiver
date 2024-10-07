using project_rabbitmq_log_service.Models;
using project_rabbitmq_reciever;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMqSettings"));

var host = builder.Build();
host.Run();

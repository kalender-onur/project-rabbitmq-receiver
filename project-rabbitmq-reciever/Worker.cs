using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;
using project_rabbitmq_log_service.Models;

namespace project_rabbitmq_reciever
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqSettings _settings;


        public Worker(ILogger<Worker> logger, IOptions<RabbitMqSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_settings.ConnectionString),
                ClientProvidedName = _settings.ClientProvidedName
            };

            IConnection connection = null;
            IModel channel = null;

            try
            {
                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.ExchangeDeclare(_settings.ExchangeName, ExchangeType.Direct);
                channel.QueueDeclare(_settings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(_settings.QueueName, _settings.ExchangeName, _settings.RoutingKey);
                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, args) =>
                {
                    var body = args.Body.ToArray();
                    string receivedMessage = Encoding.UTF8.GetString(body);


                    _logger.LogInformation($"Received message: {receivedMessage}");

                    channel.BasicAck(args.DeliveryTag, false);
                };

                channel.BasicConsume(queue: _settings.QueueName, autoAck: false, consumer: consumer);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming messages.");
                throw; 
            }
            finally
            {
                channel?.Close();
                connection?.Close();
            }
        }
    }
}

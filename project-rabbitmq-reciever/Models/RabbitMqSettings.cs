using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_rabbitmq_log_service.Models
{
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
        public string ClientProvidedName { get; set; }
    }
}

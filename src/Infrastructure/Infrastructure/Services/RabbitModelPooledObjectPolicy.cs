using Common.Settings;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.Services
{
    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly RabbitMQSettings _rabbitMQSettings;

        private readonly IConnection _connection;

        public RabbitModelPooledObjectPolicy(IOptions<RabbitMQSettings> rabbitMQSettings)
        {
            _rabbitMQSettings = rabbitMQSettings.Value;
            _connection = GetConnection();
        }

        private IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password,
                HostName = _rabbitMQSettings.HostName,
                VirtualHost = _rabbitMQSettings.VirtualHost,
                Port = _rabbitMQSettings.Port,
            };

            return factory.CreateConnection();
        }

        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                obj?.Dispose();
                return false;
            }
        }
    }
}

using Application.Interfaces.Services;
using Infrastructure.Services;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.MqManager
{
    public class RabbitManager : IRabbitService
    {
        private readonly DefaultObjectPool<IModel> _objectPool;

        public RabbitManager(RabbitModelPooledObjectPolicy objectPolicy)
        {
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
        }

        public void Publish<T>(T message, string exchangeName, string routeKey) where T : class
        {
            if (message == null)
                return;

            var channel = _objectPool.Get();

            try
            {
                var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchangeName, routeKey, properties, sendBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }

        public void Publish(byte[] message, string exchangeName, string routeKey)
        {
            if (message == null)
                return;

            var channel = _objectPool.Get();

            try
            {
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchangeName, routeKey, properties, message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }

        public void DeclareQueueWithBinding(string queueName, string exchangeName, string routingKey)
        {
            var channel = _objectPool.Get();
            try
            {
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }

    }
}

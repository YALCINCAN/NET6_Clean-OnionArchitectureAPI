namespace Application.Interfaces.Services
{
    public interface IRabbitService
    {
        void Publish<T>(T message, string exchangeName, string routeKey)
           where T : class;

        void Publish(byte[] message, string exchangeName, string routeKey);

        void DeclareQueueWithBinding(string queueName, string exchangeName, string routingKey);
    }
}

using Application.Interfaces.Services;
using Common.Settings;
using Domain.RabbitMessages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Application.BackgroundServices
{
    public class ConfirmationMailSenderBackgroundService : BackgroundService
    {
        private IConnection _connection;
        //private IModel _channel;
        private IConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _rabbitMqSettings;
        private readonly ILogger<ConfirmationMailSenderBackgroundService> _logger;
        private IServiceProvider _serviceProvider;
        private IEmailService _emailService;

        public ConfirmationMailSenderBackgroundService(IOptions<RabbitMQSettings> rabbitMqSettings, ILogger<ConfirmationMailSenderBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _rabbitMqSettings = rabbitMqSettings.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _connectionFactory = new ConnectionFactory
            {
                HostName = _rabbitMqSettings.HostName,
                UserName = _rabbitMqSettings.UserName,
                Password = _rabbitMqSettings.Password,
                VirtualHost = _rabbitMqSettings.VirtualHost,
                Port = _rabbitMqSettings.Port,
            };
            _connection = _connectionFactory.CreateConnection();
            //_channel = _connection.CreateModel();
            //InitRabbitMQDeclarations();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (int i = 0; i < _rabbitMqSettings.EmailSenderRabbitMQSettings.ConfirmationMailRabbitMQSettings.ConsumerCount_ConfirmationMailSender; i++)
            {
                _logger.LogInformation("ConfirmationMailSenderBackgroundService Running");
                var _channel = _connection.CreateModel();
                var consumer = new EventingBasicConsumer(_channel);
                InitRabbitMQDeclarations(_channel);
                consumer.Received += async (ch, ea) =>
                {
                    try
                    {
                        string content = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var message = JsonConvert.DeserializeObject<ConfirmationMailSendMessage>(content);
                        if (message is null)
                        {
                            _logger.LogError("ConfirmationMailSenderBackgroundService Message Not Found = {@Message}", message);
                            return;
                        }

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            _emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                            await _emailService.ConfirmationMailAsync(message.Link, message.Email);
                        }
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("ConfirmationMailSenderBackgroundService Exception = {@Exception}", e);
                        _logger.LogError($"ConfirmationMailSenderBackgroundService Exception Message Body = {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                };
                _channel.BasicConsume(_rabbitMqSettings.EmailSenderRabbitMQSettings.ConfirmationMailRabbitMQSettings.Queue_ConfirmationMailSender, false, consumer);
            };
            return Task.CompletedTask;
        }


        private void InitRabbitMQDeclarations(IModel _channel)
        {
            _channel.BasicQos(0, 1, false);
            _channel.ExchangeDeclare(exchange: _rabbitMqSettings.EmailSenderRabbitMQSettings.Exchange_Default, type: ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(queue: _rabbitMqSettings.EmailSenderRabbitMQSettings.ConfirmationMailRabbitMQSettings.Queue_ConfirmationMailSender, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: _rabbitMqSettings.EmailSenderRabbitMQSettings.ConfirmationMailRabbitMQSettings.Queue_ConfirmationMailSender, exchange: _rabbitMqSettings.EmailSenderRabbitMQSettings.Exchange_Default, routingKey: _rabbitMqSettings.EmailSenderRabbitMQSettings.ConfirmationMailRabbitMQSettings.Queue_ConfirmationMailSender);

        }
    }
}

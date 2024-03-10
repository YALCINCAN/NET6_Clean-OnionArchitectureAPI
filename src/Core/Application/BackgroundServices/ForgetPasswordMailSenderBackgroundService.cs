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
    public class ForgetPasswordMailSenderBackgroundService : BackgroundService
    {
        private IConnection _connection;
        //private IModel _channel;
        private IConnectionFactory _connectionFactory;
        private readonly RabbitMQSettings _rabbitMqSettings;
        private readonly ILogger<ForgetPasswordMailSenderBackgroundService> _logger;
        private IServiceProvider _serviceProvider;
        private IEmailService _emailService;

        public ForgetPasswordMailSenderBackgroundService(IOptions<RabbitMQSettings> rabbitMqSettings, ILogger<ForgetPasswordMailSenderBackgroundService> logger, IServiceProvider serviceProvider)
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
            for (int i = 0; i < _rabbitMqSettings.EmailSenderRabbitMQSettings.ForgetPasswordMailRabbitMQSettings.ConsumerCount_ForgetPasswordMailSender; i++)
            {
                _logger.LogInformation("ForgetPasswordMailSenderBackgroundService Running");
                var _channel = _connection.CreateModel();
                var consumer = new EventingBasicConsumer(_channel);
                InitRabbitMQDeclarations(_channel);
                consumer.Received += async (ch, ea) =>
                {
                    try
                    {
                        string content = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var message = JsonConvert.DeserializeObject<ForgetPasswordMailSendMessage>(content);
                        if (message is null)
                        {
                            _logger.LogError("ForgetPasswordMailSenderBackgroundService Message Not Found = {@Message}", message);
                            return;
                        }
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            _emailService = _serviceProvider.GetRequiredService<IEmailService>();
                            await _emailService.ForgetPasswordMailAsync(message.Link, message.Email);
                        }

                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("ForgetPasswordMailSenderBackgroundService Exception = {@Exception}", e);
                        _logger.LogError($"ForgetPasswordMailSenderBackgroundService Exception Message Body = {Encoding.UTF8.GetString(ea.Body.ToArray())}");
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                };
                _channel.BasicConsume(_rabbitMqSettings.EmailSenderRabbitMQSettings.ForgetPasswordMailRabbitMQSettings.Queue_ForgetPasswordMailSender, false, consumer);
            };
            return Task.CompletedTask;
        }


        private void InitRabbitMQDeclarations(IModel _channel)
        {
            _channel.ExchangeDeclare(exchange: _rabbitMqSettings.EmailSenderRabbitMQSettings.Exchange_Default, type: ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(queue: _rabbitMqSettings.EmailSenderRabbitMQSettings.ForgetPasswordMailRabbitMQSettings.Queue_ForgetPasswordMailSender, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: _rabbitMqSettings.EmailSenderRabbitMQSettings.ForgetPasswordMailRabbitMQSettings.Queue_ForgetPasswordMailSender, exchange: _rabbitMqSettings.EmailSenderRabbitMQSettings.Exchange_Default, routingKey: _rabbitMqSettings.EmailSenderRabbitMQSettings.ForgetPasswordMailRabbitMQSettings.Queue_ForgetPasswordMailSender);

        }
    }
}

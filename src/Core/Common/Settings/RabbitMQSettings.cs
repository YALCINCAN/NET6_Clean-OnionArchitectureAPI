namespace Common.Settings
{
    public class RabbitMQSettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public EmailSenderRabbitMQSettings EmailSenderRabbitMQSettings { get; set; }
    }

    public class EmailSenderRabbitMQSettings
    {
        public string Exchange_Default { get; set; }
        public ConfirmationMailRabbitMQSettings ConfirmationMailRabbitMQSettings { get; set; }
        public ForgetPasswordMailRabbitMQSettings ForgetPasswordMailRabbitMQSettings { get; set; }
        public ResetPasswordMailRabbitMQSettings ResetPasswordMailRabbitMQSettings { get; set; }
    }

    public class ConfirmationMailRabbitMQSettings
    {
        public int ConsumerCount_ConfirmationMailSender { get; set; }
        public string Queue_ConfirmationMailSender { get; set; }
    }

    public class ForgetPasswordMailRabbitMQSettings
    {
        public int ConsumerCount_ForgetPasswordMailSender { get; set; }
        public string Queue_ForgetPasswordMailSender { get; set; }
    }

    public class ResetPasswordMailRabbitMQSettings
    {
        public int ConsumerCount_ResetPasswordMailSender { get; set; }
        public string Queue_ResetPasswordMailSender { get; set; }
    }
}

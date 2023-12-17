namespace Domain.RabbitMessages
{
    public class ForgetPasswordMailSendMessage
    {
        public string Link { get; set; }
        public string Email { get; set; }
    }
}

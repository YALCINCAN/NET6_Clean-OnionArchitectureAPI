namespace Domain.RabbitMessages
{
    public class ConfirmationMailSendMessage
    {
        public string Link { get; set; }
        public string Email { get; set; }
    }
}

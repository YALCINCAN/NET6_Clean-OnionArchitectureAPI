namespace Domain.RabbitMessages
{
    public class ResetPasswordMailSendMessage
    {
        public string NewPassword { get; set; }
        public string Email { get; set; }
    }
}

using Domain.Entities.Base;

namespace Domain.Entities
{
    public class User : BaseEntity<Guid>
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public string EmailConfirmationCode { get; set; }
        public string EmailConfirmedCode { get; set; }
        public string ResetPasswordCode { get; set; }
    }
}
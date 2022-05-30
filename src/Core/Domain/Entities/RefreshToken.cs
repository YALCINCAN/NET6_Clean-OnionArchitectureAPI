using Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RefreshToken:BaseEntity<int>
    {
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public DateTime Expiration { get; set; }
    }
}

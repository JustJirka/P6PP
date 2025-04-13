using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PaymentService.API.Persistence.Entities.DB.Models
{
    public class UserCredit : Entity
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public long RoleId { get; set; }

        public long CreditBalance { get; set; }

    }
}

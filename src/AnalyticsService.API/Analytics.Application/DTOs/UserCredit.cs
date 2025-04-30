using System.ComponentModel.DataAnnotations;

namespace Analytics.Application.DTOs
{
    public class UserCreditDto
    {
        public int id { get; set; }
        public long userId { get; set; }
        public long roleId { get; set; }
        public long creditBalance { get; set; }
    }
}
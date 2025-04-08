using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analytics.Application.DTOs
{
    public class UserDto
    {
        public int id { get; set; }
        public int roleId { get; set; }
        public string state { get; set; }
        public int sex { get; set; }
        public int weight { get; set; }
        public int height { get; set; }
        public string birthDate { get; set; }
        public string createdAt { get; set; }
        public string lastUpdated { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Analytics.Application.DTOs
{
    public class UserDto
    {
        public int id { get; set; }
        public int roleId { get; set; }
        public string state { get; set; }
        public string? sex { get; set; }
        [JsonConverter(typeof(NullableIntConverter))]
        public int? weight { get; set; }
        [JsonConverter(typeof(NullableIntConverter))]
        public int? height { get; set; }
        public string birthDate { get; set; }
        public string createdAt { get; set; }
        public string lastUpdated { get; set; }
    }

    public class ExternalUserDto
    {
        public int id { get; set; }
        public int roleId { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string state { get; set; }
        public string phoneNumber { get; set; }
        public string sex { get; set; }
        public int? weight { get; set; }
        public int? height { get; set; }
        public string? dateOfBirth { get; set; }
        public string createdOn { get; set; }
        public string updatedOn { get; set; }
    }
    public class UserDataResponse
    {
        public List<UserDto> users { get; set; }
        public int totalCount { get; set; }
    }
}

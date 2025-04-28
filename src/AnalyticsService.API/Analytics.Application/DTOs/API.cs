using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analytics.Application.DTOs
{
    public class ApiResponse<T>
    {
        public T data { get; set; }
        public bool success { get; set; }
        public string? message { get; set; }
    }
}

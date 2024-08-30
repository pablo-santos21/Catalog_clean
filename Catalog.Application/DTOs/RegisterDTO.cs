using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.DTOs;
public class RegisterDTO
{
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
}


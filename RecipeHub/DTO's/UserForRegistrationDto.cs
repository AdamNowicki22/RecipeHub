﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeHub.DTO_s
{
    public class UserForRegistrationDto
    {
        [Required]
        [MaxLength(256)]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength (256)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MaxLength(64)]
        public string Password { get; set; } = string.Empty;
        [JsonIgnore]
        public ICollection<string> Roles { get; set; }= new List<string> {"User"};
    }
}

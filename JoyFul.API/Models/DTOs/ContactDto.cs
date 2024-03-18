using System.ComponentModel.DataAnnotations;

namespace JoyFul.API.Models.DTOs
{
    public class ContactDto
    {
        [Required,MaxLength(100)]
        public string FirstName { get; set; }

        [Required ,MaxLength(100)]
        public string LastName { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string? PhoneNumber { get; set; }


        public int SubjectId { get; set; }

        public string Message { get; set; }
    }
}

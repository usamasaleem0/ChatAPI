using System.ComponentModel.DataAnnotations;

namespace MyChatRoom.Controllers.Dtos
{
    public class UserDto
    {
        [Required]
        [StringLength(15 , MinimumLength =3, ErrorMessage ="Name must be at least {2} , and maximum {15} characters")]
        public string Name { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace MyChatRoom.Controllers.Dtos
{
    public class MessageDto
    {
        [Required]
        public string From { get; set; }
        public string To { get; set; }
        public string Content { get; set; }
    }
}

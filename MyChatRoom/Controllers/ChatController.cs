using Microsoft.AspNetCore.Mvc;
using MyChatRoom.Controllers.Dtos;
using MyChatRoom.Services;

namespace MyChatRoom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        public readonly ChatService _chatService;
        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("register-user")]
        public IActionResult RegisterUser(UserDto model)
        {
            if(_chatService.AddUserToList(model.Name))
            {
                return NoContent();
            }
            return BadRequest("The name is taken please choose another name");
        }


    }
}

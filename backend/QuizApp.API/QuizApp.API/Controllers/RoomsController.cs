using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Application.Interfaces;

namespace QuizApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController(IGameService gameService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
        {
            var session = await gameService.CreateRoomAsync(request.Topic, request.Difficulty);
            return Ok(new { session.Id, session.RoomCode, session.Topic, session.Difficulty });
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetRoom(string code)
        {
            var session = await gameService.GetRoomByCodeAsync(code);
            if (session is null) return NotFound();

            return Ok(new
            {
                session.RoomCode,
                session.Topic,
                session.Status,
                Players = session.PlayerSessions.Select(ps => new
                {
                    ps.Player.Username,
                    ps.Player.AvatarColor
                })
            });
        }

        [HttpPost("{code}/join")]
        public async Task<IActionResult> JoinRoom(string code, [FromBody] JoinRoomRequest request)
        {
            try
            {
                var playerSession = await gameService.JoinRoomAsync(code, request.PlayerName);
                var token = Convert.ToBase64String(playerSession.Id.ToByteArray());

                return Ok(new
                {
                    playerSession.Id,
                    playerSession.Player.Username,
                    playerSession.Player.AvatarColor,
                    Token = token
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public record CreateRoomRequest(string Topic, int Difficulty);
    public record JoinRoomRequest(string PlayerName);
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Application.Interfaces;

namespace QuizApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController(IGameSessionRepository sessionRepo, IPlayerRepository playerRepo) : ControllerBase
    {
 
        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var players = await playerRepo.GetTopPlayersAsync(10);
            return Ok(players.Select(p => new
            {
                p.Username,
                p.TotalGames,
                p.TotalWins,
                WinRate = p.TotalGames > 0
                    ? Math.Round((double)p.TotalWins / p.TotalGames * 100, 1)
                    : 0
            }));
        }

        [HttpGet("players/{playerId}")]
        public async Task<IActionResult> GetPlayerStats(Guid playerId)
        {
            var player = await playerRepo.GetByIdAsync(playerId);
            if (player is null) return NotFound();

            return Ok(new
            {
                player.Username,
                player.TotalGames,
                player.TotalWins,
                WinRate = player.TotalGames > 0
                    ? Math.Round((double)player.TotalWins / player.TotalGames * 100, 1)
                    : 0
            });
        }

        [HttpGet("games/{roomCode}")]
        public async Task<IActionResult> GetGameStats(string roomCode)
        {
            var session = await sessionRepo.GetByCodeAsync(roomCode);
            if (session is null) return NotFound();

            return Ok(new
            {
                session.RoomCode,
                session.Topic,
                session.Difficulty,
                session.Status,
                session.StartedAt,
                session.FinishedAt,
                players = session.PlayerSessions
                    .OrderBy(ps => ps.FinalRank)
                    .Select(ps => new
                    {
                        ps.Player.Username,
                        ps.FinalScore,
                        ps.FinalRank,
                        ps.CorrectAnswers
                    })
            });
        }
    }
}

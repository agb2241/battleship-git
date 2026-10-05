using Battleship.API.GameStore.Abstractions;
using Battleship.API.Models;
using Battleship.Data.Services;
using Battleship.Domain.Board;
using Battleship.Domain.Factories;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly GameFactory _gameFactory;
        private readonly IGameStore _gameStore;
        private readonly CompletedGameService _completedGameService;

        public GamesController(GameFactory gameFactory, IGameStore gameStore, CompletedGameService completedGameService)
        {
            _gameFactory = gameFactory;
            _gameStore = gameStore;
            _completedGameService = completedGameService;
        }

        [HttpPost]
        public ActionResult<CreateGameResponse> CreateGame()
        {
            var game = _gameFactory.CreateGame();

            _gameStore.Add(game);

            return Ok(new CreateGameResponse(game.Id, game.GameConfiguration.BoardSize, game.GameConfiguration.FleetConfiguration.Fleet.Count));
        }

        [HttpGet("{id:guid}")]
        public ActionResult<GameStateResponse> GetGame(Guid id)
        {
            var game = _gameStore.Get(id);

            if (game == null)
            {
                return NotFound();
            }


            var shots = new List<ShotResponse>();
            foreach(var shot in game.Gameboard.Shots)
            {
                shots.Add(new ShotResponse(shot.Coordinate.X, shot.Coordinate.Y, shot.Outcome.ToString().ToLowerInvariant(), shot.SunkShip?.Name));
            }

            return Ok(new GameStateResponse(
                game.Id,
                game.GameConfiguration.BoardSize,
                game.Gameboard.Shots.Count,
                game.ShipsRemaining,
                game.IsWon,
                shots));
        }

        [HttpPost("{id:guid}/shots")]
        public async Task<ActionResult<FireShotResponse>> FireShot(Guid id, FireShotRequest request)
        {
            var game = _gameStore.Get(id);

            if (game == null)
            {
                return NotFound();
            }

            try
            {
                var summary = game.FireShot(new Coordinate(request.X, request.Y));

                if (summary.IsWon == true)
                {
                    await _completedGameService.SaveCompletedGameAsync(game);
                }

                return Ok(new FireShotResponse(
                    summary.ShotResult.Outcome.ToString().ToLowerInvariant(),
                    summary.ShotResult.SunkShip?.Name,
                    summary.ShotsFired,
                    summary.ShipsRemaining,
                    summary.IsWon));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/api/summaries")]
        public async Task<ActionResult> GetSummaries()
        {
            var summaries = await _completedGameService.GetSummariesAsync();

            var response = summaries
                .Select(x => new CompletedGameSummaryResponse(
                    x.GameId,
                    x.BoardSize,
                    x.ShipCount,
                    x.TotalShots,
                    DateTime.SpecifyKind(x.CompletedAtUtc, DateTimeKind.Utc)))
                .ToList();

            return Ok(response);
        }
    }
}

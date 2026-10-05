# Battleship

## Overview
A small Battleship application built with ASP.NET Core and C#.

The application keeps the game rules and state on the server. The web UI is minimal, it creates games, fires shots, and renders the public game state returned by the API.

## Architecture
The solution is separated into several projects based on responsibility.

### Battleship.Domain
Contains the core game logic and has no dependency on the API, UI, Entity Framework, or database.

Responsibilities include:

- Board and coordinate modeling
- Ship definitions and ship state
- Ship placement
- Fleet and game configuration
- Shot processing
- Hit, sunk, and win detection
- Random game creation

Keeping these rules in the domain project allows the game behavior to be tested independently from the web and persistence layers.

### Battleship.Data
Contains EF Core persistence for completed games.

Only completed game summaries are persisted. Active game state is intentionally not stored in the database.

A completed summary contains:

- Game ID
- Board size
- Ship count
- Total shots
- Completion time

### Battleship.API
Provides the HTTP API and coordinates the domain and persistence layers.

Active games are stored in memory using an `InMemoryGameStore`.

Endpoints:

- `POST /api/Games` - Creates a new game.
- `POST /api/Games/{id}/shots` - Fires a shot.
- `GET /api/Games/{id}` - Returns the public state of an active game.
- `GET /api/Summaries` - Returns completed game summaries.

Ship locations are not exposed through the API while a game is active.
- Battleship.WebUI
- Tests

### Battleship.WebUI
A minimal ASP.NET Core MVC frontend.

The frontend:

- Creates games through the API
- Displays a clickable game board
- Fires shots through the API
- Displays miss, hit, and sunk states
- Displays sunk ship and winning messages
- Displays completed game summaries
- Rehydrates an existing game after a browser refresh

The browser stores only the active game ID. Game state remains authoritative on the server and is reconstructed using `GET /api/Games/{id}`.

## Game State
Active games are stored in memory for the lifetime of the API process.

Completed game summaries are persisted using Entity Framework Core and SQL Server.

This was an intentional choice to keep the implementation focused on the assessment requirements without introducing persistence concerns into the core game model.

One consequence is that restarting the API clears any games currently in progress. Completed game summaries remain persisted.

## Random Ship Placement
Randomness is accessed through the `IRandomProvider` abstraction.

The production implementation is RandomeProvider which uses the .NET random number generator. 

Tests implementation is TestRandomProvider, it is a Queue that provides a deterministic implementation that returns predetermined values.

This allows ship placement to remain random in production while making placement behavior predictable and repeatable in unit tests.

Placement validates that:

- Ships remain within board boundaries
- Ships do not overlap
- Ships are placed horizontally or vertically

A maximum number of placement attempts prevents an invalid random sequence from creating an infinite placement loop.

## Shot Processing
Shots are processed by the domain rather than the controller or frontend.

A shot can result in:

- Miss
- Hit
- Sunk

When a ship is sunk, the result identifies the ship.

The game also tracks:

- Number of shots fired
- Ships remaining
- Whether the game has been won

Repeated shots at the same coordinate are idempotent. The original shot result is returned and the repeated request does not increase the shot count.

Shots outside the board are rejected.

Shots fired after the game has been won are also rejected.

When the winning shot is processed, a completed game summary is persisted before the API returns the successful response.

## Testing
The solution contains automated tests covering the core game rules and persistence behavior.

Tests include:

- Valid ship placement
- Boundary validation
- Overlap prevention
- Deterministic ship placement
- Placement retries
- Misses
- Hits
- Sunk ships
- Winning shots
- Duplicate shots
- Out-of-bounds shots
- Shots after a completed game
- Completed-game persistence
- Correct persisted shot count

The persistence test uses an in-memory SQLite database so it exercises EF Core through a relational database without requiring a SQL Server instance for the test suite.

## Prerequisites
The following are required to build and run the application:

- .NET 10 SDK
- SQL Server LocalDB
- Entity Framework Core CLI tools

Verify the .NET SDK is installed:

```powershell
dotnet --version
```

Install the Entity Framework Core CLI tools if they are not already installed:

```powershell
dotnet tool install --global dotnet-ef
```

Verify the EF Core tools are available:

```powershell
dotnet ef --version
```

## Getting Started
Clone the repository and navigate to the repository root.

### 1. Restore Dependencies
```powershell
dotnet restore
```

### 2. Build the Solution
```powershell
dotnet build
```

### 3. Run the Tests
```powershell
dotnet test
```

All tests should pass before running the application.

### 4. Create the Database
The application uses SQL Server LocalDB for completed-game persistence.

Apply the included Entity Framework Core migrations:

```powershell
dotnet ef database update --project src/Battleship.Data --startup-project src/Battleship.API
```

This will create the Battleship database and required tables.

### 5. Start the API
From the repository root:

```powershell
dotnet run --project src/Battleship.API --launch-profile https
```

The API will run at:

```text
https://localhost:7239
```

Swagger UI is available at:

```text
https://localhost:7239/swagger
```

Leave the API running.

### 6. Start the Web UI
Open a second terminal at the repository root and run:

```powershell
dotnet run --project src/Battleship.WebUI --launch-profile https
```

The Web UI will run, type the following in a new browser:

```text
https://localhost:7264
```

Both the API and Web UI must be running to play the game.

## Development Database
Completed game summaries are stored in SQL Server LocalDB.

Active games are stored in memory and therefore do not survive an API restart. Completed game summaries remain available after restarting the application.

## Design Decisions and Tradeoffs
The implementation intentionally focuses on keeping the game rules isolated from infrastructure while avoiding unnecessary complexity for the scope of the assessment.

### Active Game State
Active games are stored in memory using an `InMemoryGameStore`.

This keeps active game state separate from the persistence requirements for completed games and avoids introducing database persistence into the core gameplay loop.

The tradeoff is that active games do not survive an API restart and the current implementation is intended for a single API instance. In a production environment, active state could be moved to a durable or distributed store.

### Completed Game Persistence
Only completed game summaries are persisted using Entity Framework Core.

The winning shot waits for the completed-game summary to be successfully persisted before returning the response to the client. This adds a small amount of latency to the final shot but ensures the game has been recorded before the API reports successful completion.

### Domain Separation
Core game behavior is contained in `Battleship.Domain` and does not depend on ASP.NET Core, Entity Framework Core, or the frontend.

The API is responsible for translating HTTP requests into domain operations and mapping domain results into public API responses.

This prevents web and persistence concerns from becoming part of the game rules and allows the domain to be tested independently.

### Randomness and Testability
Random ship placement uses the `IRandomProvider` abstraction rather than directly accessing a random number generator throughout the domain.

The production implementation generates random values normally, while tests provide predetermined values. This makes randomized placement deterministic and repeatable during testing.

A maximum number of placement attempts is also enforced to prevent an unfavorable random sequence from causing an infinite placement loop.

### Repeated Shots
Repeated shots at the same coordinate are treated as idempotent.

The original shot result is returned and the shot count is not increased. This prevents a repeated request for the same coordinate from changing the state of the game.

Explicit HTTP idempotency keys were not implemented.

### API Contracts
The API returns dedicated response DTOs rather than exposing domain or persistence objects directly.

This keeps the public HTTP contract separate from the internal implementation and, importantly, prevents hidden ship locations from accidentally being exposed to the frontend.

### Game Rehydration
The frontend stores only the active game ID in browser storage.

When the page is refreshed, the frontend retrieves the current public game state from the API and reconstructs the board from the shot history.

The browser therefore does not become the authoritative source of game state.

Because active games are stored in memory, rehydration works across browser refreshes but not across an API restart.

### Concurrency
The active game store uses a `ConcurrentDictionary`, which provides thread-safe access to the collection of games.

Individual `Game` instances are not currently synchronized against multiple simultaneous shot requests. Per-game synchronization or another concurrency strategy would be appropriate if concurrent requests against the same game were expected in a production environment.

### Fleet Configuration
The domain supports board and fleet configuration, but the application currently creates a standard 10x10 board with a fixed fleet.

Exposing configurable board sizes and fleets through the API was intentionally left out to keep the implementation focused on the core requirements.

### Ship Adjacency
The current placement rules prevent ships from overlapping but allow ships to be directly adjacent to one another.

A no-adjacency rule could be added as an additional board placement constraint without changing the overall placement design.

### Frontend
The frontend is intentionally minimal.

It is responsible for displaying the board, accepting player input, and rendering the state returned by the API. Game rules remain on the server rather than being duplicated in JavaScript.

### Docker
Docker support was not added. The application currently uses the standard .NET development environment and SQL Server LocalDB.

## Potential Improvements
Given additional time, possible improvements include:

- Persist active games so they survive application restarts.
- Support multiple API instances using a shared or distributed active-game store.
- Add per-game concurrency protection for simultaneous shot requests.
- Expose configurable board sizes and fleet configurations through the API.
- Add explicit HTTP idempotency keys for shot requests.
- Add an optional rule preventing ships from being placed adjacent to one another.
- Add Docker and Docker Compose support for the API, Web UI, and database.
- Add additional API-level integration tests covering HTTP status codes and response contracts.
- Improve frontend styling, accessibility, and user feedback.
- Add structured logging and additional operational diagnostics.

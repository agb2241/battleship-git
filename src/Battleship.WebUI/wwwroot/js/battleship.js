const apiBaseUrl = 'https://localhost:7239/api/Games';
const summariesApiUrl = 'https://localhost:7239/api/Summaries';

let gameId = null;
let boardSize = 0;
let gameWon = false;

document.addEventListener('DOMContentLoaded', async function () {
    document
        .getElementById('new-game')
        .addEventListener('click', createGame);

    await loadSummaries();

    const existingGameId = localStorage.getItem('battleshipGameId');

    if (existingGameId) {
        await loadGame(existingGameId);
    }
});

async function createGame() {
    const response = await fetch(apiBaseUrl, {
        method: 'POST'
    });

    if (!response.ok) {
        setStatus('Unable to create game.');
        return;
    }

    const game = await response.json();

    gameId = game.gameId;
    boardSize = game.boardSize;
    gameWon = false;

    localStorage.setItem('battleshipGameId', game.gameId);

    document.getElementById('shots-fired').textContent = '0';
    document.getElementById('ships-remaining').textContent = game.shipCount;
    document.getElementById('game-info').classList.remove('d-none');

    createBoard();

    setStatus('Game started. Fire away!');
}

function createBoard() {
    const board = document.getElementById('game-board');

    board.innerHTML = '';
    board.style.gridTemplateColumns =
        `repeat(${boardSize}, 42px)`;

    for (let y = 0; y < boardSize; y++) {
        for (let x = 0; x < boardSize; x++) {
            const cell = document.createElement('button');

            cell.classList.add('board-cell');
            cell.dataset.x = x;
            cell.dataset.y = y;

            cell.addEventListener('click', fireShot);

            board.appendChild(cell);
        }
    }
}

async function fireShot(event) {
    if (gameWon) {
        return;
    }

    const cell = event.currentTarget;

    const x = Number(cell.dataset.x);
    const y = Number(cell.dataset.y);

    const response = await fetch(
        `${apiBaseUrl}/${gameId}/shots`,
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                x: x,
                y: y
            })
        });

    if (!response.ok) {
        const message = await response.text();
        setStatus(message);
        return;
    }

    const result = await response.json();

    cell.classList.add(result.outcome);
    cell.disabled = true;

    document.getElementById('shots-fired').textContent =
        result.shotsFired;

    document.getElementById('ships-remaining').textContent =
        result.shipsRemaining;

    if (result.outcome === 'sunk') {
        setStatus(`${result.sunkShip} sunk!`);
    }
    else {
        setStatus(
            result.outcome === 'hit'
                ? 'Hit!'
                : 'Miss.'
        );
    }

    if (result.isWon) {
        gameWon = true;

        setStatus(
            `You won in ${result.shotsFired} shots!`
        );

        disableBoard();
    }
}

function disableBoard() {
    document
        .querySelectorAll('.board-cell')
        .forEach(cell => cell.disabled = true);
}

function setStatus(message) {
    document.getElementById('game-status').textContent = message;
}

async function loadGame(id) {
    const response = await fetch(`${apiBaseUrl}/${id}`);

    if (!response.ok) {
        localStorage.removeItem('battleshipGameId');
        return;
    }

    const game = await response.json();

    gameId = game.gameId;
    boardSize = game.boardSize;
    gameWon = game.won;

    createBoard();

    for (const shot of game.shots) {
        const cell = document.querySelector(
            `.board-cell[data-x="${shot.x}"][data-y="${shot.y}"]`);

        if (cell) {
            cell.classList.add(shot.outcome.toLowerCase());
            cell.disabled = true;
        }
    }

    document.getElementById('shots-fired').textContent =
        game.shotsFired;

    document.getElementById('ships-remaining').textContent =
        game.shipsRemaining;

    document.getElementById('game-info')
        .classList.remove('d-none');

    if (game.won) {
        setStatus(
            `Game completed in ${game.shotsFired} shots.`
        );

        disableBoard();
    }
    else {
        setStatus('Game restored.');
    }
}

async function loadSummaries() {
    const response = await fetch(summariesApiUrl);

    if (!response.ok) {
        return;
    }

    const summaries = await response.json();

    const tbody =
        document.getElementById('game-summaries');

    tbody.innerHTML = '';

    if (summaries.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5">
                    No completed games.
                </td>
            </tr>`;

        return;
    }

    for (const summary of summaries) {
        const row = document.createElement('tr');

        row.innerHTML = `
            <td>${summary.gameId}</td>
            <td>${summary.boardSize} x ${summary.boardSize}</td>
            <td>${summary.shipCount}</td>
            <td>${summary.totalShots}</td>
            <td>${formatDate(summary.completedAtUtc)}</td>
        `;

        tbody.appendChild(row);
    }
}

function formatDate(value) {
    return new Date(value).toLocaleString();
}
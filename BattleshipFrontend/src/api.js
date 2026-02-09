const API_BASE = 'http://localhost:5133/api/game';

export const api = {
    async getStatus() {
        const res = await fetch(`${API_BASE}/status`);
        return res.json();
    },

    async createGame(playerNames) {
        const res = await fetch(`${API_BASE}/create`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ playerNames }),
        });
        return res.json();
    },

    async resetGame() {
        const res = await fetch(`${API_BASE}/reset`, {
            method: 'POST',
        });
        return res.json();
    },

    async getGameState() {
        const res = await fetch(`${API_BASE}/state`);
        if (!res.ok) {
            const error = await res.json();
            throw new Error(error.error);
        }
        return res.json();
    },

    async placeShip(playerName, shipType, row, col, orientation) {
        const res = await fetch(`${API_BASE}/place-ship`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ playerName, shipType, row, col, orientation }),
        });
        const data = await res.json();
        if (!res.ok) throw new Error(data.error);
        return data;
    },

    async startBattle() {
        const res = await fetch(`${API_BASE}/start-battle`, {
            method: 'POST',
        });
        const data = await res.json();
        if (!res.ok) throw new Error(data.error);
        return data;
    },

    async fire(playerName, row, col) {
        const res = await fetch(`${API_BASE}/fire`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ playerName, row, col }),
        });
        const data = await res.json();
        if (!res.ok) throw new Error(data.error);
        return data;
    },

    async endTurn(playerName) {
        const res = await fetch(`${API_BASE}/end-turn`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ playerName }),
        });
        const data = await res.json();
        if (!res.ok) throw new Error(data.error);
        return data;
    },

    async getBoard(playerName, hideShips = false) {
        const res = await fetch(`${API_BASE}/board/${playerName}?hideShips=${hideShips}`);
        return res.json();
    },
};

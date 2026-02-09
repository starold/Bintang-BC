import { useState, useEffect, useCallback } from 'react';
import { api } from '../api';
import Board from './Board';

const SHIPS = [
    { type: 'Carrier', size: 5 },
    { type: 'Battleship', size: 4 },
    { type: 'Cruiser', size: 3 },
];

export default function Game({ gameState: initialState, onReset }) {
    const [gameState, setGameState] = useState(initialState);
    const [selectedShip, setSelectedShip] = useState(null);
    const [orientation, setOrientation] = useState('Horizontal');
    const [message, setMessage] = useState({ type: 'info', text: '' });
    const [loading, setLoading] = useState(false);
    const [placedShips, setPlacedShips] = useState({});

    const currentPlayer = gameState?.currentPlayerName;
    const phase = gameState?.gamePhase;
    const players = gameState?.players || [];

    const refreshGameState = useCallback(async () => {
        try {
            const state = await api.getGameState();
            setGameState(state);
        } catch (err) {
            console.error('Failed to refresh game state:', err);
        }
    }, []);

    useEffect(() => {
        // Initialize placed ships from game state
        const placed = {};
        players.forEach(p => {
            placed[p.name] = [];
            if (p.board?.cells) {
                const ships = new Set();
                p.board.cells.forEach(row => {
                    row.forEach(cell => {
                        if (cell.shipType) ships.add(cell.shipType);
                    });
                });
                placed[p.name] = Array.from(ships);
            }
        });
        setPlacedShips(placed);
    }, [players]);

    const handlePlaceShip = async (row, col) => {
        if (!selectedShip || phase !== 'Setup') return;

        setLoading(true);
        setMessage({ type: '', text: '' });

        try {
            await api.placeShip(currentPlayer, selectedShip, row, col, orientation);
            setMessage({ type: 'success', text: `${selectedShip} placed successfully!` });
            setPlacedShips(prev => ({
                ...prev,
                [currentPlayer]: [...(prev[currentPlayer] || []), selectedShip]
            }));
            setSelectedShip(null);
            await refreshGameState();
        } catch (err) {
            setMessage({ type: 'error', text: err.message });
        } finally {
            setLoading(false);
        }
    };

    const handleFire = async (row, col) => {
        if (phase !== 'Battle') return;

        setLoading(true);
        setMessage({ type: '', text: '' });

        try {
            const result = await api.fire(currentPlayer, row, col);

            let text = '';
            if (result.result === 'Hit') text = '🎯 Hit!';
            else if (result.result === 'Sink') text = `💥 You sunk their ${result.sunkShipType}!`;
            else text = '💨 Miss!';

            setMessage({ type: result.result === 'Miss' ? 'info' : 'success', text });

            if (result.isGameOver) {
                await refreshGameState();
            } else {
                // Auto switch turn after fire
                await api.endTurn(currentPlayer);
                await refreshGameState();
            }
        } catch (err) {
            setMessage({ type: 'error', text: err.message });
        } finally {
            setLoading(false);
        }
    };

    const handleStartBattle = async () => {
        setLoading(true);
        try {
            await api.startBattle();
            setMessage({ type: 'success', text: 'Battle started! 🔥' });
            await refreshGameState();
        } catch (err) {
            setMessage({ type: 'error', text: err.message });
        } finally {
            setLoading(false);
        }
    };

    const handleEndTurn = async () => {
        setLoading(true);
        try {
            await api.endTurn(currentPlayer);
            setMessage({ type: 'info', text: '' });
            await refreshGameState();
        } catch (err) {
            setMessage({ type: 'error', text: err.message });
        } finally {
            setLoading(false);
        }
    };

    const handleResetGame = async () => {
        try {
            await api.resetGame();
            onReset();
        } catch (err) {
            setMessage({ type: 'error', text: err.message });
        }
    };

    const getPlayerPlacedShips = (playerName) => {
        return placedShips[playerName] || [];
    };

    const allShipsPlaced = (playerName) => {
        const placed = getPlayerPlacedShips(playerName);
        return placed.length >= SHIPS.length;
    };

    const bothPlayersReady = players.every(p => allShipsPlaced(p.name));

    if (gameState?.isGameOver) {
        return (
            <div className="winner-overlay">
                <div className="winner-modal">
                    <div className="winner-emoji">🏆</div>
                    <div className="winner-text">Victory!</div>
                    <div className="winner-name">{gameState.winnerName} wins!</div>
                    <button className="btn btn-primary" onClick={handleResetGame}>
                        Play Again
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="game-container">
            <div className="game-header">
                <h1 className="game-title">⚓ Battleship</h1>
                <div className="game-status">
                    <span className={`phase-badge phase-${phase?.toLowerCase()}`}>
                        {phase === 'Setup' ? '🔧 Setup Phase' : '⚔️ Battle Phase'}
                    </span>
                    <span className="current-player">
                        🎮 {currentPlayer}'s Turn
                    </span>
                </div>
                <button className="btn btn-secondary" onClick={handleResetGame} style={{ width: 'auto' }}>
                    🔄 Reset
                </button>
            </div>

            {message.text && (
                <div className={`message ${message.type}`}>{message.text}</div>
            )}

            <div className="boards-container">
                {players.map((player, idx) => {
                    const isCurrentPlayer = player.name === currentPlayer;
                    const playerPlacedShips = getPlayerPlacedShips(player.name);
                    const opponent = players.find(p => p.name !== currentPlayer);

                    return (
                        <div key={player.name} className="board-section">
                            <div className="board-header">
                                <span className={`player-name ${isCurrentPlayer ? 'active' : ''}`}>
                                    {player.name}
                                </span>
                                <div className="player-stats">
                                    <span>🚢 Ships: {player.shipsRemaining}/{SHIPS.length}</span>
                                    <span>💀 Lost: {player.shipsDestroyed}</span>
                                </div>
                            </div>

                            <Board
                                board={player.board}
                                isOwn={isCurrentPlayer}
                                disabled={
                                    loading ||
                                    (phase === 'Setup' && !isCurrentPlayer) ||
                                    (phase === 'Setup' && !selectedShip) ||
                                    (phase === 'Battle' && isCurrentPlayer)
                                }
                                onCellClick={
                                    phase === 'Setup' && isCurrentPlayer
                                        ? handlePlaceShip
                                        : phase === 'Battle' && !isCurrentPlayer
                                            ? handleFire
                                            : undefined
                                }
                                selectedShip={selectedShip}
                                orientation={orientation}
                                canPlace={phase === 'Setup' && isCurrentPlayer}
                            />

                            {phase === 'Setup' && isCurrentPlayer && (
                                <div className="ship-placement">
                                    <h4>Select Ship to Place:</h4>
                                    <div className="ships-list">
                                        {SHIPS.map((ship) => (
                                            <button
                                                key={ship.type}
                                                className={`ship-btn ${selectedShip === ship.type ? 'selected' : ''}`}
                                                onClick={() => setSelectedShip(ship.type)}
                                                disabled={playerPlacedShips.includes(ship.type)}
                                            >
                                                {ship.type} ({ship.size})
                                            </button>
                                        ))}
                                    </div>
                                    <div className="orientation-toggle">
                                        <button
                                            className={`orientation-btn ${orientation === 'Horizontal' ? 'active' : ''}`}
                                            onClick={() => setOrientation('Horizontal')}
                                        >
                                            ➡️ Horizontal
                                        </button>
                                        <button
                                            className={`orientation-btn ${orientation === 'Vertical' ? 'active' : ''}`}
                                            onClick={() => setOrientation('Vertical')}
                                        >
                                            ⬇️ Vertical
                                        </button>
                                    </div>
                                </div>
                            )}
                        </div>
                    );
                })}
            </div>

            <div className="action-buttons">
                {phase === 'Setup' && (
                    <>
                        {!allShipsPlaced(currentPlayer) && (
                            <div className="message info">
                                Place all your ships before continuing
                            </div>
                        )}
                        {allShipsPlaced(currentPlayer) && !bothPlayersReady && (
                            <button className="btn btn-secondary" onClick={handleEndTurn} disabled={loading}>
                                ✅ Done - Next Player
                            </button>
                        )}
                        {bothPlayersReady && (
                            <button className="btn btn-primary" onClick={handleStartBattle} disabled={loading}>
                                ⚔️ Start Battle!
                            </button>
                        )}
                    </>
                )}
            </div>
        </div>
    );
}

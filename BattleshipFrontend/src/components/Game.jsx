import { useState, useEffect, useCallback } from 'react';
import { api } from '../api';
import Board from './Board';
import { useSound } from '../contexts/SoundContext';
import { useSettings } from '../contexts/SettingsContext';

// Import ship images
import sawit5Img from '../../asset/image/sawit 5.png';
import sawit4Img from '../../asset/image/sawit 4.png';
import sawit3Img from '../../asset/image/sawit 3.png';
import hutan5Img from '../../asset/image/hutan 5.png';
import hutan4Img from '../../asset/image/hutan 4.png';
import hutan3Img from '../../asset/image/hutan 3.png';

const SHIP_IMAGES = {
    sawit: { 5: sawit5Img, 4: sawit4Img, 3: sawit3Img },
    hutan: { 5: hutan5Img, 4: hutan4Img, 3: hutan3Img }
};

const SHIPS = [
    { type: 'Carrier', size: 5, displayName: 'Kapal Besar' },
    { type: 'Battleship', size: 4, displayName: 'Kapal Sedang' },
    { type: 'Cruiser', size: 3, displayName: 'Kapal Kecil' },
];

export default function Game({ gameState: initialState, onReset }) {
    const [gameState, setGameState] = useState(initialState);
    const [selectedShip, setSelectedShip] = useState(null);
    const [orientation, setOrientation] = useState('Horizontal');
    const [message, setMessage] = useState({ type: 'info', text: '' });
    const [loading, setLoading] = useState(false);
    const [placedShips, setPlacedShips] = useState({});
    const [screenShake, setScreenShake] = useState(false);
    const [shootCounter, setShootCounter] = useState(0);

    const { playSound, playSoundtrack, stopSoundtrack } = useSound();
    const { settings } = useSettings();

    const currentPlayer = gameState?.currentPlayerName;
    const phase = gameState?.gamePhase;
    const players = gameState?.players || [];

    // Determine player category (player1 = sawit, player2 = hutan)
    const getPlayerCategory = (playerName) => {
        const playerIdx = players.findIndex(p => p.name === playerName);
        return playerIdx === 0 ? 'sawit' : 'hutan';
    };

    // Add category to ships based on current player
    const getShipsWithCategory = () => {
        const category = getPlayerCategory(currentPlayer);
        return SHIPS.map(s => ({ ...s, category }));
    };

    const refreshGameState = useCallback(async () => {
        try {
            const state = await api.getGameState();
            setGameState(state);
        } catch (err) {
            console.error('Failed to refresh game state:', err);
        }
    }, []);

    // Play war soundtrack when battle starts
    useEffect(() => {
        if (phase === 'Battle') {
            playSoundtrack(settings.selectedWarSoundtrack);
        }
        return () => {
            if (phase === 'Battle') {
                stopSoundtrack();
            }
        };
    }, [phase, settings.selectedWarSoundtrack, playSoundtrack, stopSoundtrack]);

    useEffect(() => {
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

            // Play sound based on player category
            const category = getPlayerCategory(currentPlayer);
            if (category === 'sawit') {
                playSound('naroSawit');
            } else {
                playSound('naroPohon');
            }

            setMessage({ type: 'success', text: `${selectedShip} berhasil ditempatkan!` });
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
            // Play shooting sound based on current player's category
            const category = getPlayerCategory(currentPlayer);
            const newCounter = shootCounter + 1;
            setShootCounter(newCounter);

            if (category === 'sawit') {
                playSound('sawitNembak');
            } else {
                playSound('hutanNembak');
            }

            // Screen shake effect
            setScreenShake(true);
            setTimeout(() => setScreenShake(false), 300);

            const result = await api.fire(currentPlayer, row, col);

            let text = '';
            if (result.result === 'Hit') text = '🎯 KENA!';
            else if (result.result === 'Sink') text = `💥 ${result.sunkShipType} TENGGELAM!`;
            else text = '💨 Meleset!';

            setMessage({ type: result.result === 'Miss' ? 'info' : 'success', text });

            if (result.isGameOver) {
                playSound('menang');
                await refreshGameState();
            } else {
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
            setMessage({ type: 'success', text: 'Pertempuran dimulai! 🔥' });
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
        stopSoundtrack();
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

    // Victory screen
    if (gameState?.isGameOver) {
        return (
            <div className="winner-overlay">
                <div className="confetti"></div>
                <div className="winner-modal">
                    <div className="winner-trophy">🏆</div>
                    <h1 className="winner-title">KEMENANGAN!</h1>
                    <p className="winner-name">{gameState.winnerName}</p>
                    <p className="winner-subtitle">adalah pemenangnya!</p>
                    <button className="btn btn-primary btn-glow" onClick={handleResetGame}>
                        🔄 Main Lagi
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className={`game-container ${screenShake ? 'screen-shake' : ''}`}>
            <div className="game-header">
                <h1 className="game-title">⚔️ Sawit vs Hutan</h1>
                <div className="game-status">
                    <span className={`phase-badge phase-${phase?.toLowerCase()}`}>
                        {phase === 'Setup' ? '🔧 Setup' : '⚔️ Battle'}
                    </span>
                    <span className="current-player-badge">
                        {getPlayerCategory(currentPlayer) === 'sawit' ? '🌴' : '🌲'} {currentPlayer}
                    </span>
                </div>
                <button className="btn btn-danger" onClick={handleResetGame}>
                    🔄 Reset
                </button>
            </div>

            {message.text && (
                <div className={`message ${message.type} ${message.type === 'success' ? 'pulse' : ''}`}>
                    {message.text}
                </div>
            )}

            <div className="boards-container">
                {players.map((player, idx) => {
                    const isCurrentPlayer = player.name === currentPlayer;
                    const playerPlacedShips = getPlayerPlacedShips(player.name);
                    const playerCategory = idx === 0 ? 'sawit' : 'hutan';
                    const shipsWithCategory = SHIPS.map(s => ({ ...s, category: playerCategory }));

                    return (
                        <div key={player.name} className={`board-section ${isCurrentPlayer ? 'active-board' : ''}`}>
                            <div className="board-header">
                                <span className={`player-name ${isCurrentPlayer ? 'active' : ''}`}>
                                    {playerCategory === 'sawit' ? '🌴' : '🌲'} {player.name}
                                </span>
                                <div className="player-stats">
                                    <span>🚢 {player.shipsRemaining}/{SHIPS.length}</span>
                                    <span className="destroyed">💀 {player.shipsDestroyed}</span>
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
                                shipImages={SHIP_IMAGES}
                                ships={shipsWithCategory}
                                playerIndex={idx}
                            />

                            {phase === 'Setup' && isCurrentPlayer && (
                                <div className="ship-placement">
                                    <h4>Pilih Kapal untuk Ditempatkan:</h4>
                                    <div className="ships-list">
                                        {getShipsWithCategory().map((ship) => (
                                            <button
                                                key={ship.type}
                                                className={`ship-btn ${selectedShip === ship.type ? 'selected' : ''} ${ship.category}`}
                                                onClick={() => setSelectedShip(ship.type)}
                                                disabled={playerPlacedShips.includes(ship.type)}
                                            >
                                                {ship.category === 'sawit' ? '🌴' : '🌲'} {ship.displayName} ({ship.size})
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
                                Tempatkan semua kapal sebelum melanjutkan
                            </div>
                        )}
                        {allShipsPlaced(currentPlayer) && !bothPlayersReady && (
                            <button className="btn btn-secondary" onClick={handleEndTurn} disabled={loading}>
                                ✅ Selesai - Pemain Berikutnya
                            </button>
                        )}
                        {bothPlayersReady && (
                            <button className="btn btn-primary btn-glow" onClick={handleStartBattle} disabled={loading}>
                                ⚔️ Mulai Pertempuran!
                            </button>
                        )}
                    </>
                )}
            </div>
        </div>
    );
}

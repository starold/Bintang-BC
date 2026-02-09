import { useState } from 'react';

export default function Homepage({ onStartGame }) {
    const [player1, setPlayer1] = useState('');
    const [player2, setPlayer2] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleStart = async (e) => {
        e.preventDefault();
        if (!player1.trim() || !player2.trim()) {
            setError('Both player names are required');
            return;
        }
        if (player1.trim() === player2.trim()) {
            setError('Player names must be different');
            return;
        }
        setLoading(true);
        setError('');
        try {
            await onStartGame([player1.trim(), player2.trim()]);
        } catch (err) {
            setError(err.message);
            setLoading(false);
        }
    };

    return (
        <div className="homepage">
            <h1 className="homepage-title">⚓ Battleship</h1>
            <p className="homepage-subtitle">The Classic Naval Combat Game</p>

            <form className="player-form" onSubmit={handleStart}>
                <h2 className="form-title">Enter Player Names</h2>

                {error && <div className="message error">{error}</div>}

                <div className="input-group">
                    <label htmlFor="player1">Player 1</label>
                    <input
                        id="player1"
                        type="text"
                        placeholder="Enter name..."
                        value={player1}
                        onChange={(e) => setPlayer1(e.target.value)}
                        disabled={loading}
                    />
                </div>

                <div className="input-group">
                    <label htmlFor="player2">Player 2</label>
                    <input
                        id="player2"
                        type="text"
                        placeholder="Enter name..."
                        value={player2}
                        onChange={(e) => setPlayer2(e.target.value)}
                        disabled={loading}
                    />
                </div>

                <button type="submit" className="btn btn-primary" disabled={loading}>
                    {loading ? (
                        <span className="loading">
                            <span className="spinner"></span>
                            Starting...
                        </span>
                    ) : (
                        '🚀 Start Battle'
                    )}
                </button>
            </form>

            <div style={{ marginTop: '3rem', textAlign: 'center', color: 'var(--text-muted)' }}>
                <p>🎯 Place your ships strategically</p>
                <p>💥 Take turns attacking the enemy fleet</p>
                <p>🏆 Sink all ships to win!</p>
            </div>
        </div>
    );
}

import { useState } from 'react';

export default function Homepage({ onStartGame }) {
    const [player1, setPlayer1] = useState('');
    const [player2, setPlayer2] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleStart = async (e) => {
        e.preventDefault();
        if (!player1.trim() || !player2.trim()) {
            setError('Nama kedua pemain harus diisi');
            return;
        }
        if (player1.trim() === player2.trim()) {
            setError('Nama pemain harus berbeda');
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
            <div className="homepage-bg">
                <div className="floating-element palm-1">🌴</div>
                <div className="floating-element palm-2">🌴</div>
                <div className="floating-element tree-1">🌲</div>
                <div className="floating-element tree-2">🌲</div>
            </div>

            <div className="homepage-content">
                <h1 className="homepage-title">
                    <span className="title-word sawit">Sawit</span>
                    <span className="title-vs">VS</span>
                    <span className="title-word hutan">Hutan</span>
                </h1>
                <p className="homepage-subtitle">Pertempuran Strategis Klasik</p>

                <form className="player-form" onSubmit={handleStart}>
                    <h2 className="form-title">🎮 Mulai Permainan</h2>

                    {error && <div className="message error shake">{error}</div>}

                    <div className="input-group">
                        <label htmlFor="player1">
                            <span className="player-icon">🌴</span> Pemain 1 (Sawit)
                        </label>
                        <input
                            id="player1"
                            type="text"
                            placeholder="Masukkan nama..."
                            value={player1}
                            onChange={(e) => setPlayer1(e.target.value)}
                            disabled={loading}
                            className="input-sawit"
                        />
                    </div>

                    <div className="input-group">
                        <label htmlFor="player2">
                            <span className="player-icon">🌲</span> Pemain 2 (Hutan)
                        </label>
                        <input
                            id="player2"
                            type="text"
                            placeholder="Masukkan nama..."
                            value={player2}
                            onChange={(e) => setPlayer2(e.target.value)}
                            disabled={loading}
                            className="input-hutan"
                        />
                    </div>

                    <button type="submit" className="btn btn-primary btn-glow" disabled={loading}>
                        {loading ? (
                            <span className="loading">
                                <span className="spinner"></span>
                                Memulai...
                            </span>
                        ) : (
                            <>⚔️ Mulai Pertempuran!</>
                        )}
                    </button>
                </form>

                <div className="homepage-features">
                    <div className="feature">
                        <span className="feature-icon">🎯</span>
                        <p>Strategi Cerdas</p>
                    </div>
                    <div className="feature">
                        <span className="feature-icon">💥</span>
                        <p>Aksi Seru</p>
                    </div>
                    <div className="feature">
                        <span className="feature-icon">🏆</span>
                        <p>Menangkan!</p>
                    </div>
                </div>
            </div>
        </div>
    );
}

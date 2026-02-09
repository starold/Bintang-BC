import { useState, useEffect } from 'react';
import { api } from './api';
import Homepage from './components/Homepage';
import Game from './components/Game';
import './index.css';

function App() {
  const [gameState, setGameState] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    checkExistingGame();
  }, []);

  const checkExistingGame = async () => {
    try {
      const status = await api.getStatus();
      if (status.hasActiveGame) {
        const state = await api.getGameState();
        setGameState(state);
      }
    } catch (err) {
      console.error('Failed to check game status:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleStartGame = async (playerNames) => {
    const state = await api.createGame(playerNames);
    setGameState(state);
  };

  const handleReset = () => {
    setGameState(null);
  };

  if (loading) {
    return (
      <div className="app">
        <div className="homepage">
          <div className="loading">
            <span className="spinner"></span>
            Loading...
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="app">
      {gameState ? (
        <Game gameState={gameState} onReset={handleReset} />
      ) : (
        <Homepage onStartGame={handleStartGame} />
      )}
    </div>
  );
}

export default App;

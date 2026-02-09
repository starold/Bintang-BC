import { useState, useEffect } from 'react';
import { api } from './api';
import { SettingsProvider } from './contexts/SettingsContext';
import { SoundProvider, useSound } from './contexts/SoundContext';
import LoadingScreen from './components/LoadingScreen';
import Navbar from './components/Navbar';
import SettingsModal from './components/SettingsModal';
import Homepage from './components/Homepage';
import RulesPage from './components/RulesPage';
import Game from './components/Game';
import './index.css';

function AppContent() {
  const [gameState, setGameState] = useState(null);
  const [loading, setLoading] = useState(true);
  const [loadingProgress, setLoadingProgress] = useState(0);
  const [currentPage, setCurrentPage] = useState('home');
  const [settingsOpen, setSettingsOpen] = useState(false);
  const { playSoundtrack, stopSoundtrack } = useSound();

  useEffect(() => {
    // Simulate loading progress
    const interval = setInterval(() => {
      setLoadingProgress(prev => {
        if (prev >= 100) {
          clearInterval(interval);
          return 100;
        }
        return prev + Math.random() * 20;
      });
    }, 200);

    checkExistingGame();

    return () => clearInterval(interval);
  }, []);

  // Play homepage soundtrack
  useEffect(() => {
    if (!loading && !gameState && currentPage === 'home') {
      playSoundtrack('homepage');
    } else {
      stopSoundtrack();
    }
  }, [loading, gameState, currentPage, playSoundtrack, stopSoundtrack]);

  // Update page title
  useEffect(() => {
    const titles = {
      home: 'Sawit vs Hutan - Battleship',
      rules: 'Peraturan - Sawit vs Hutan',
    };
    document.title = gameState ? 'Pertempuran! - Sawit vs Hutan' : (titles[currentPage] || titles.home);
  }, [currentPage, gameState]);

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
      setTimeout(() => setLoading(false), 1500);
    }
  };

  const handleStartGame = async (playerNames) => {
    stopSoundtrack();
    const state = await api.createGame(playerNames);
    setGameState(state);
  };

  const handleReset = () => {
    setGameState(null);
    setCurrentPage('home');
  };

  const handleNavigate = (page) => {
    setCurrentPage(page);
  };

  if (loading || loadingProgress < 100) {
    return <LoadingScreen progress={loadingProgress} />;
  }

  return (
    <div className="app">
      {/* Only show navbar when NOT in game */}
      {!gameState && (
        <Navbar
          currentPage={currentPage}
          onNavigate={handleNavigate}
          onSettingsClick={() => setSettingsOpen(true)}
        />
      )}

      <SettingsModal
        isOpen={settingsOpen}
        onClose={() => setSettingsOpen(false)}
      />

      <main className="main-content">
        {gameState ? (
          <Game gameState={gameState} onReset={handleReset} />
        ) : currentPage === 'rules' ? (
          <RulesPage onBack={() => handleNavigate('home')} />
        ) : (
          <Homepage onStartGame={handleStartGame} />
        )}
      </main>
    </div>
  );
}

function App() {
  return (
    <SettingsProvider>
      <SoundProvider>
        <AppContent />
      </SoundProvider>
    </SettingsProvider>
  );
}

export default App;

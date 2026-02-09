import logoImg from '../../asset/image/logo.png';

export default function Navbar({ currentPage, onNavigate, onSettingsClick }) {
    return (
        <nav className="navbar">
            <div className="navbar-container">
                <div className="navbar-brand">
                    <img src={logoImg} alt="Logo" className="navbar-logo" />
                    <span className="navbar-title">Sawit vs Hutan</span>
                </div>

                <div className="navbar-links">
                    <button
                        className={`nav-link ${currentPage === 'home' ? 'active' : ''}`}
                        onClick={() => onNavigate('home')}
                    >
                        <span className="nav-icon">🎮</span>
                        <span>Bermain</span>
                    </button>
                    <button
                        className={`nav-link ${currentPage === 'rules' ? 'active' : ''}`}
                        onClick={() => onNavigate('rules')}
                    >
                        <span className="nav-icon">📜</span>
                        <span>Peraturan</span>
                    </button>
                </div>

                <button className="navbar-settings" onClick={onSettingsClick}>
                    <span className="settings-icon">⚙️</span>
                </button>
            </div>
        </nav>
    );
}

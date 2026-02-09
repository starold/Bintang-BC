import loadingBg from '../../asset/image/Loading Screen.png';

export default function LoadingScreen({ progress }) {
    return (
        <div className="loading-screen">
            <img src={loadingBg} alt="Loading" className="loading-bg" />
            <div className="loading-content">
                <h1 className="loading-title">Sawit vs Hutan</h1>
                <div className="loading-icons">
                    <span className="loading-icon sawit">🌴</span>
                    <span className="loading-vs">⚔️</span>
                    <span className="loading-icon hutan">🌲</span>
                </div>
                <div className="progress-container">
                    <div
                        className="progress-bar"
                        style={{ width: `${Math.min(progress, 100)}%` }}
                    ></div>
                </div>
                <p className="loading-status">
                    {progress < 100 ? 'Mempersiapkan pertempuran...' : 'Siap!'}
                </p>
            </div>
        </div>
    );
}

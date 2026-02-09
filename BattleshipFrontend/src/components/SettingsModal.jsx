import { useSettings } from '../contexts/SettingsContext';

export default function SettingsModal({ isOpen, onClose }) {
    const { settings, updateSetting, resetSettings } = useSettings();

    if (!isOpen) return null;

    const soundtrackOptions = [
        { value: 'perang1', label: '🎵 Soundtrack Perang 1' },
        { value: 'perang2', label: '🎵 Soundtrack Perang 2' },
        { value: 'perang3', label: '🎵 Soundtrack Perang 3' },
    ];

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="settings-modal" onClick={e => e.stopPropagation()}>
                <h2 className="modal-title">⚙️ Pengaturan</h2>

                <div className="settings-section">
                    <h3 className="section-title">🔊 Volume</h3>

                    <div className="settings-group">
                        <label>
                            <span>Volume Utama</span>
                            <span className="volume-value">{settings.masterVolume}%</span>
                        </label>
                        <input
                            type="range"
                            min="0"
                            max="100"
                            value={settings.masterVolume}
                            onChange={(e) => updateSetting('masterVolume', Number(e.target.value))}
                            className="volume-slider master"
                        />
                    </div>

                    <div className="settings-group">
                        <label>
                            <span>💥 Efek Suara</span>
                            <span className="volume-value">{settings.sfxVolume}%</span>
                        </label>
                        <input
                            type="range"
                            min="0"
                            max="100"
                            value={settings.sfxVolume}
                            onChange={(e) => updateSetting('sfxVolume', Number(e.target.value))}
                            className="volume-slider sfx"
                        />
                    </div>

                    <div className="settings-group">
                        <label>
                            <span>🎵 Musik</span>
                            <span className="volume-value">{settings.musicVolume}%</span>
                        </label>
                        <input
                            type="range"
                            min="0"
                            max="100"
                            value={settings.musicVolume}
                            onChange={(e) => updateSetting('musicVolume', Number(e.target.value))}
                            className="volume-slider music"
                        />
                    </div>
                </div>

                <div className="settings-section">
                    <h3 className="section-title">⚔️ Soundtrack Perang</h3>
                    <div className="soundtrack-selector">
                        {soundtrackOptions.map(option => (
                            <button
                                key={option.value}
                                className={`soundtrack-btn ${settings.selectedWarSoundtrack === option.value ? 'active' : ''}`}
                                onClick={() => updateSetting('selectedWarSoundtrack', option.value)}
                            >
                                {option.label}
                            </button>
                        ))}
                    </div>
                </div>

                <div className="settings-actions">
                    <button className="btn btn-secondary" onClick={resetSettings}>
                        🔄 Reset Default
                    </button>
                    <button className="btn btn-primary" onClick={onClose}>
                        ✅ Simpan
                    </button>
                </div>
            </div>
        </div>
    );
}

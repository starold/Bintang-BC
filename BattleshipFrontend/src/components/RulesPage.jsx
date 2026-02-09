export default function RulesPage({ onBack }) {
    return (
        <div className="rules-page">
            <div className="rules-container">
                <h1 className="rules-title">📜 Peraturan Permainan</h1>

                <div className="rules-content">
                    <div className="rule-card">
                        <span className="rule-icon">🎯</span>
                        <h3>Tujuan</h3>
                        <p>Tenggelamkan semua kapal lawan sebelum kapal Anda ditenggelamkan.</p>
                    </div>

                    <div className="rule-card">
                        <span className="rule-icon">🚢</span>
                        <h3>Kapal</h3>
                        <p>Setiap pemain memiliki 3 kapal dengan ukuran berbeda: 5, 4, dan 3 tiles.</p>
                    </div>

                    <div className="rule-card">
                        <span className="rule-icon">🔧</span>
                        <h3>Fase Setup</h3>
                        <p>Tempatkan kapal secara bergantian. Pilih orientasi horizontal atau vertikal.</p>
                    </div>

                    <div className="rule-card">
                        <span className="rule-icon">⚔️</span>
                        <h3>Fase Battle</h3>
                        <p>Bergantian menembak ke papan lawan. Kena = merah, Meleset = biru.</p>
                    </div>

                    <div className="rule-card">
                        <span className="rule-icon">🏆</span>
                        <h3>Menang</h3>
                        <p>Pemain pertama yang menenggelamkan semua kapal lawan adalah pemenangnya!</p>
                    </div>
                </div>

                <button className="btn btn-primary" onClick={onBack}>
                    ⬅️ Kembali ke Beranda
                </button>
            </div>
        </div>
    );
}

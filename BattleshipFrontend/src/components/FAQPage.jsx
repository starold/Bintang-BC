export default function FAQPage({ onBack }) {
    const faqs = [
        {
            q: "Bagaimana cara menempatkan kapal?",
            a: "Pilih kapal dari daftar, pilih orientasi (horizontal/vertikal), lalu klik pada papan Anda."
        },
        {
            q: "Apa bedanya Hit dan Miss?",
            a: "Hit berarti tembakan kena kapal lawan (merah). Miss berarti tembakan meleset (biru)."
        },
        {
            q: "Kapan giliran saya berakhir?",
            a: "Setelah Anda menembak, giliran otomatis berpindah ke lawan."
        },
        {
            q: "Bagaimana cara menang?",
            a: "Tenggelamkan semua kapal lawan sebelum kapal Anda habis!"
        },
        {
            q: "Bisakah kapal ditempatkan diagonal?",
            a: "Tidak, kapal hanya bisa ditempatkan secara horizontal atau vertikal."
        }
    ];

    return (
        <div className="page-container harvest-theme">
            <div className="harvest-bg">
                <div className="sun"></div>
                <div className="cloud cloud-1"></div>
                <div className="cloud cloud-2"></div>
                <div className="hills"></div>
                <div className="grass"></div>
            </div>

            <div className="page-content">
                <div className="page-card">
                    <h1 className="page-title">❓ Tanya Jawab (FAQ)</h1>

                    <div className="faq-list">
                        {faqs.map((faq, idx) => (
                            <div key={idx} className="faq-item">
                                <h3 className="faq-question">Q: {faq.q}</h3>
                                <p className="faq-answer">A: {faq.a}</p>
                            </div>
                        ))}
                    </div>

                    <button className="btn btn-primary" onClick={onBack}>
                        ⬅️ Kembali ke Menu
                    </button>
                </div>
            </div>
        </div>
    );
}

export default function Board({
    board,
    isOwn,
    onCellClick,
    disabled,
    selectedShip,
    orientation,
    canPlace
}) {
    const cols = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
    const rows = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];

    const getShipPreview = (row, col) => {
        if (!selectedShip || !isOwn || !canPlace) return [];
        const size = getShipSize(selectedShip);
        const cells = [];

        for (let i = 0; i < size; i++) {
            if (orientation === 'Horizontal') {
                cells.push({ row, col: col + i });
            } else {
                cells.push({ row: row + i, col });
            }
        }
        return cells;
    };

    const getShipSize = (type) => {
        switch (type) {
            case 'Carrier': return 5;
            case 'Battleship': return 4;
            case 'Cruiser': return 3;
            default: return 0;
        }
    };

    const handleCellClick = (row, col) => {
        if (disabled) return;
        onCellClick?.(row, col);
    };

    return (
        <div className="board-with-labels">
            <div></div>
            <div className="col-labels">
                {cols.map((col) => (
                    <div key={col} className="col-label">{col}</div>
                ))}
            </div>
            <div className="row-labels">
                {rows.map((row) => (
                    <div key={row} className="row-label">{row}</div>
                ))}
            </div>
            <div
                className="board"
                style={{ gridTemplateColumns: `repeat(${board?.cols || 10}, 1fr)` }}
            >
                {board?.cells?.map((row, rowIndex) =>
                    row.map((cell, colIndex) => {
                        const isShip = isOwn ? cell.hasShip : (cell.isShot && cell.hasShip);
                        const isHit = cell.isShot && cell.hasShip;
                        const isMiss = cell.isShot && !cell.hasShip;

                        return (
                            <div
                                key={`${rowIndex}-${colIndex}`}
                                className={`cell ${isShip ? 'has-ship' : ''} ${cell.isShot ? 'is-shot' : ''} ${disabled ? 'disabled' : ''}`}
                                onClick={() => handleCellClick(rowIndex, colIndex)}
                                title={`${rows[rowIndex]}${colIndex}`}
                            >
                                {isHit && '💥'}
                                {isMiss && '💨'}
                            </div>
                        );
                    })
                )}
            </div>
        </div>
    );
}

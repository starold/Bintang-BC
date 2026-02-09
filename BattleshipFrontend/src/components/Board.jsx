import hitImg from '../../asset/image/hit.png';
import missImg from '../../asset/image/miss.png';

export default function Board({
    board,
    isOwn,
    onCellClick,
    disabled,
    selectedShip,
    orientation,
    canPlace,
    shipImages,
    ships,
    playerIndex
}) {
    const cols = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
    const rows = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];

    const getShipSize = (type) => {
        const ship = ships?.find(s => s.type === type);
        return ship?.size || 0;
    };

    const getShipInfo = (type) => {
        return ships?.find(s => s.type === type);
    };

    const handleCellClick = (row, col) => {
        if (disabled) return;
        onCellClick?.(row, col);
    };

    // Group cells by ship type for image rendering
    const getShipCells = () => {
        const shipCells = {};
        if (!board?.cells) return shipCells;

        board.cells.forEach((row, rowIdx) => {
            row.forEach((cell, colIdx) => {
                if (cell.shipType && isOwn) {
                    if (!shipCells[cell.shipType]) {
                        shipCells[cell.shipType] = [];
                    }
                    shipCells[cell.shipType].push({ row: rowIdx, col: colIdx, cell });
                }
            });
        });
        return shipCells;
    };

    const shipCellGroups = getShipCells();

    const getShipPlacement = (cells) => {
        if (cells.length === 0) return null;
        const sortedCells = [...cells].sort((a, b) => {
            if (a.row === b.row) return a.col - b.col;
            return a.row - b.row;
        });

        const isHorizontal = sortedCells.every(c => c.row === sortedCells[0].row);
        return {
            startRow: sortedCells[0].row,
            startCol: sortedCells[0].col,
            isHorizontal,
            size: cells.length
        };
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
                        const shipInfo = cell.shipType ? getShipInfo(cell.shipType) : null;

                        const shipPlacement = cell.shipType && isOwn ?
                            getShipPlacement(shipCellGroups[cell.shipType] || []) : null;
                        const isFirstCell = shipPlacement &&
                            rowIndex === shipPlacement.startRow &&
                            colIndex === shipPlacement.startCol;

                        return (
                            <div
                                key={`${rowIndex}-${colIndex}`}
                                className={`cell 
                                    ${isShip ? 'has-ship' : ''} 
                                    ${cell.isShot ? 'is-shot' : ''} 
                                    ${disabled ? 'disabled' : ''}
                                    ${isHit ? 'cell-hit' : ''}
                                    ${isMiss ? 'cell-miss' : ''}
                                    ${shipInfo?.category || ''}
                                `}
                                onClick={() => handleCellClick(rowIndex, colIndex)}
                                title={`${rows[rowIndex]}${colIndex}`}
                            >
                                {/* Ship image */}
                                {isFirstCell && shipImages && shipInfo && (
                                    <div
                                        className={`ship-image-container ${shipPlacement.isHorizontal ? 'horizontal' : 'vertical'}`}
                                        style={{
                                            width: shipPlacement.isHorizontal ? `${shipPlacement.size * 100}%` : '100%',
                                            height: shipPlacement.isHorizontal ? '100%' : `${shipPlacement.size * 100}%`,
                                        }}
                                    >
                                        <img
                                            src={shipImages[shipInfo.category]?.[shipPlacement.size]}
                                            alt={shipInfo.displayName}
                                            className="ship-image"
                                        />
                                    </div>
                                )}

                                {/* Hit / Miss icons */}
                                {isHit && <img src={hitImg} alt="Hit" className="hit-marker" />}
                                {isMiss && <img src={missImg} alt="Miss" className="miss-marker" />}
                            </div>
                        );
                    })
                )}
            </div>
        </div>
    );
}

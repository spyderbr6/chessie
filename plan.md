# Capablanca Chess Implementation Plan

See full detailed architectural plan at: `C:\Users\Desktop\.claude\plans\merry-hopping-dove.md`

## Current Progress (2026-01-10)

### ✅ Phases 1-7 Complete
**Phase 1 - Core Infrastructure:**
- Folder structure
- Enums (PieceType, PieceColor, GameState, MoveType)
- BoardPosition struct with algebraic notation
- ChessBoard component (10×8 array, castling rights, en passant)
- BoardVisualizer component (renders board)
- BoardSquare component (raycasting for clicks)

**Phase 2 - Move System Foundation:**
- Move struct (immutable with factory methods)
- ChessPiece abstract base class
- TurnManager component

**Phase 3 - All Pieces:**
- Rook, Knight, Bishop, Queen, King, Pawn
- Archbishop (Bishop+Knight hybrid)
- Chancellor (Rook+Knight hybrid)
- BoardSetup for Capablanca starting position

**Phase 4 - Input & Visual Feedback:**
- MoveGenerator (calculates legal moves, filters pseudo-legal moves for check)
- SquareHighlighter (visual feedback with object pooling)
- PieceSelector (selection logic with turn validation)
- ChessInputHandler (mouse input with raycasting)

**Phase 5 - Move Validation & Execution:**
- MoveExecutor (applies moves, handles captures, updates state)
- MoveHistory (tracks all moves, provides notation)
- GameController (coordinates all systems, manages game flow)
- GameState enum (NotStarted, InProgress, Checkmate, Stalemate, Draw)
- Basic checkmate/stalemate detection
- 50-move rule tracking

**Phase 6 - Special Moves Implementation:**
- SpecialMoveHandler (generates castling, en passant, promotion moves)
- Capablanca castling rules (King f→c/i, Rook a→d/j→h)
- En passant capture detection and generation
- Pawn promotion (defaults to Queen, keyboard selector for other pieces)
- Integration with MoveGenerator for seamless special move handling

**Phase 7 - UI & Game Information Display:**
- GameStatusUI (displays turn, check warnings, game state)
- MoveHistoryUI (scrollable move list with auto-scroll)
- CapturedPiecesDisplay (shows captured pieces, material advantage)
- GameEndUI (game over screen with results, stats, new game/quit buttons)
- Integration with GameController for automatic UI updates
- TextMeshPro support for modern text rendering

### 🔄 Next: Complete!
All core features implemented! Optional enhancements:
- Graphical promotion piece selector (currently keyboard-based)
- Sound effects and animations
- Save/load games
- AI opponent
- Online multiplayer

### ⬜ Optional Phases
- Phase 8: Polish & Animations (piece movement, captures, sounds)
- Phase 9: Advanced Features (save/load, AI, settings menu)

## Quick Reference

### Capablanca Board Setup
```
Rank 7 (Black): R N A B Q K B C N R
Rank 6 (Black): P P P P P P P P P P
Rank 5-2:       (empty)
Rank 1 (White): P P P P P P P P P P
Rank 0 (White): R N A B Q K B C N R
Files:          a b c d e f g h i j
                0 1 2 3 4 5 6 7 8 9
```

**Archbishop (A)**: Moves like Bishop + Knight combined
**Chancellor (C)**: Moves like Rook + Knight combined

### Key Files Completed
**Core Board & Visualization:**
- `Assets/Scripts/Core/Board/BoardPosition.cs` - Coordinate system
- `Assets/Scripts/Core/Board/ChessBoard.cs` - Board state management
- `Assets/Scripts/Core/Board/BoardVisualizer.cs` - Renders 10×8 board
- `Assets/Scripts/Core/Board/BoardSquare.cs` - Click detection

**Move System:**
- `Assets/Scripts/Core/MoveSystem/Move.cs` - Move struct with factory methods
- `Assets/Scripts/Core/MoveSystem/MoveGenerator.cs` - Legal move generation with check filtering
- `Assets/Scripts/Core/MoveSystem/MoveExecutor.cs` - Move execution and state updates (Phase 5)
- `Assets/Scripts/Core/MoveSystem/MoveHistory.cs` - Move tracking and notation (Phase 5)
- `Assets/Scripts/Core/MoveSystem/SpecialMoveHandler.cs` - Castling, en passant, promotion (Phase 6)

**Pieces:**
- `Assets/Scripts/Core/Pieces/ChessPiece.cs` - Abstract base class
- `Assets/Scripts/Core/Pieces/` - All 8 piece types (Pawn, Rook, Knight, Bishop, Queen, King, Archbishop, Chancellor)

**Game Logic:**
- `Assets/Scripts/Core/GameLogic/BoardSetup.cs` - Initial Capablanca positioning
- `Assets/Scripts/Core/GameLogic/TurnManager.cs` - Turn management
- `Assets/Scripts/Core/GameLogic/GameController.cs` - Central game coordinator (Phase 5)
- `Assets/Scripts/Core/GameLogic/GameState.cs` - Game state enum

**Input & Interaction:**
- `Assets/Scripts/Core/Input/ChessInputHandler.cs` - Mouse input and raycasting
- `Assets/Scripts/Core/Input/PieceSelector.cs` - Piece selection logic
- `Assets/Scripts/Core/Input/SquareHighlighter.cs` - Visual feedback system

**UI (Phases 6-7):**
- `Assets/Scripts/Core/UI/PromotionSelector.cs` - Keyboard-based promotion piece selection
- `Assets/Scripts/Core/UI/GameStatusUI.cs` - Turn and game state display (Phase 7)
- `Assets/Scripts/Core/UI/MoveHistoryUI.cs` - Scrollable move history (Phase 7)
- `Assets/Scripts/Core/UI/CapturedPiecesDisplay.cs` - Captured pieces and material (Phase 7)
- `Assets/Scripts/Core/UI/GameEndUI.cs` - Game over screen with stats (Phase 7)

### Architecture Highlights
- **Coordinate System**: Files 0-9 (a-j), Ranks 0-7 (1-8)
- **Move Generation**: Two-phase (pseudo-legal → filter for check)
- **Extensibility**: IPlayer interface for future AI support
- **Performance**: King reference caching, object pooling for highlights
- **Capablanca Castling**: King starts at f-file (5), castles to c-file (2) queenside or i-file (8) kingside
- **Special Moves**: Integrated via SpecialMoveHandler for castling, en passant, and promotion

### Capablanca-Specific Rules Implemented
- **Castling Positions**:
  - Queenside: King f1→c1 (5→2), Rook a1→d1 (0→3)
  - Kingside: King f1→i1 (5→8), Rook j1→h1 (9→7)
- **Promotion Pieces**: Queen, Rook, Bishop, Knight, Archbishop, Chancellor
- **En Passant**: Standard rules on Capablanca 10×8 board

For complete implementation details, technical specs, and testing checklist, see the full plan file referenced at the top.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cecs475.BoardGames.Chess {
	
	public class ChessBoard : IGameBoard {
		/// <summary>
		/// The number of rows and columns on the chess board.
		/// </summary>
		public const int BOARD_SIZE = 8;

        // Reminder: there are 3 different types of rooks
        private sbyte[,] mBoard = new sbyte[8, 8] {
                {-2, -4, -5, -6, -7, -5, -4, -3 },
                {-1, -1, -1, -1, -1, -1, -1, 1 },
                {0, 0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 0, 0 },
                {-1, 1, 1, 1, 1, 1, 1, 1 },
                {2, 4, 5, 6, 7, 5, 4, 3 }
            };

        // TODO:
        // You need a way of keeping track of certain game state flags. For example, a rook cannot perform a castling move
        // if either the rook or its king has moved in the game, so you need a way of determining whether those things have 
        // happened. There are several ways to do it and I leave it up to you.
        private bool checkWRookKing, checkWRookQueen, checkWKing, checkBRookKing, checkBRookQueen, checkBKing;
        private int player;
        /// <summary>
        /// Constructs a new chess board with the default starting arrangement.
        /// </summary>
        public ChessBoard() {
			MoveHistory = new List<IGameMove>();
            player = 1;
            checkWKing = true;
            checkWRookKing = true;
            checkWRookQueen = true;
            checkBKing = true;
            checkBRookKing = true;
            checkBRookQueen = true;
            // TODO:
            // Finish any other one-time setup.
        }

		/// <summary>
		/// Constructs a new chess board by only placing pieces as specified.
		/// </summary>
		/// <param name="startingPositions">a sequence of tuple pairs, where each pair specifies the starting
		/// position of a particular piece to place on the board</param>
		public ChessBoard(IEnumerable<Tuple<BoardPosition, ChessPiecePosition>> startingPositions)
			
			: this() { // NOTE THAT THIS CONSTRUCTOR CALLS YOUR DEFAULT CONSTRUCTOR FIRST


			foreach (int i in Enumerable.Range(0, 8)) { // another way of doing for i = 0 to < 8
				foreach (int j in Enumerable.Range(0, 8)) {
					mBoard[i, j] = 0;
				}
			}
			foreach (var pos in startingPositions) {
				SetPieceAtPosition(pos.Item1, pos.Item2);
			}
            
		}

		/// <summary>
		/// A difference in piece values for the pieces still controlled by white vs. black, where
		/// a pawn is value 1, a knight and bishop are value 3, a rook is value 5, and a queen is value 9.
		/// </summary>
		public int Value { get; private set; }


		
		public int CurrentPlayer {
			get {
                return player == 1 ? 1 : 2;
			}
		}

		// An auto-property suffices here.
		public IList<IGameMove> MoveHistory {
			get; private set;
		}

        /// <summary>
        /// Returns the piece and player at the given position on the board.
        /// </summary>
        public ChessPiecePosition GetPieceAtPosition(BoardPosition position) {
			var boardVal = mBoard[position.Row, position.Col];
			return new ChessPiecePosition((ChessPieceType)Math.Abs(mBoard[position.Row, position.Col]),
				boardVal > 0 ? 1 : boardVal < 0 ? 2 : 0);
		}

    
		public void ApplyMove(IGameMove move) {

            ChessMove m = move as ChessMove;
            if(m.MoveType != ChessMoveType.PawnPromote) {
                m.Captured = GetPieceAtPosition(m.EndPosition);
            }
            
            m.Piece = GetPieceAtPosition(m.StartPosition);
            if (m.MoveType == ChessMoveType.CastleKingSide) {

                if(player == 1) {
                    SetPieceAtPosition(m.EndPosition,m.Piece);
                    SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                    SetPieceAtPosition(new BoardPosition(7,5), GetPieceAtPosition(new BoardPosition(7,7)));
                    SetPieceAtPosition(new BoardPosition(7, 7), new ChessPiecePosition(ChessPieceType.Empty,0));
                } else {
                    SetPieceAtPosition(m.EndPosition, m.Piece);
                    SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                    SetPieceAtPosition(new BoardPosition(0, 5), GetPieceAtPosition(new BoardPosition(0, 7)));
                    SetPieceAtPosition(new BoardPosition(0, 7), new ChessPiecePosition(ChessPieceType.Empty, 0));
                }
                MoveHistory.Add(move);
                player = -player;
            } else if (m.MoveType == ChessMoveType.CastleQueenSide) {
                if (player == 1) {
                    SetPieceAtPosition(m.EndPosition, m.Piece);
                    SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                    SetPieceAtPosition(new BoardPosition(7, 3), new ChessPiecePosition(ChessPieceType.RookQueen, 1));
                    SetPieceAtPosition(new BoardPosition(7, 0), new ChessPiecePosition(ChessPieceType.Empty, 0));
                } else {
                    SetPieceAtPosition(m.EndPosition, m.Piece);
                    SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                    SetPieceAtPosition(new BoardPosition(0, 3), GetPieceAtPosition(new BoardPosition(0, 0)));
                    SetPieceAtPosition(new BoardPosition(0, 0), new ChessPiecePosition(ChessPieceType.Empty, 0));
                }
                MoveHistory.Add(move);
                player = -player;
            } else if (m.MoveType == ChessMoveType.PawnPromote) {
                if(m.EndPosition.Col == (int)ChessPieceType.Bishop) {
                    SetPieceAtPosition(m.StartPosition,new ChessPiecePosition(ChessPieceType.Bishop,CurrentPlayer));
                    if(player == 1) {
                        Value -= GetPieceValue(ChessPieceType.Pawn);
                        Value += GetPieceValue(ChessPieceType.Bishop);
                    } else {
                        Value += GetPieceValue(ChessPieceType.Pawn);
                        Value -= GetPieceValue(ChessPieceType.Bishop);
                    }
                }else if (m.EndPosition.Col == (int)ChessPieceType.RookPawn) {
                    SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.RookPawn, CurrentPlayer));
                    if (player == 1) {
                        Value -= GetPieceValue(ChessPieceType.Pawn);
                        Value += GetPieceValue(ChessPieceType.RookPawn);
                    } else {
                        Value += GetPieceValue(ChessPieceType.Pawn);
                        Value -= GetPieceValue(ChessPieceType.RookPawn);
                    }
                } else if (m.EndPosition.Col == (int)ChessPieceType.Queen) {
                    SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Queen, CurrentPlayer));
                    if (player == 1) {
                        Value -= GetPieceValue(ChessPieceType.Pawn);
                        Value += GetPieceValue(ChessPieceType.Queen);
                    } else {
                        Value += GetPieceValue(ChessPieceType.Pawn);
                        Value -= GetPieceValue(ChessPieceType.Queen);
                    }
                } else { //knight
                    SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Knight, CurrentPlayer));
                    if (player == 1) {
                        Value -= GetPieceValue(ChessPieceType.Pawn);
                        Value += GetPieceValue(ChessPieceType.Knight);
                    } else {
                        Value += GetPieceValue(ChessPieceType.Pawn);
                        Value -= GetPieceValue(ChessPieceType.Knight);
                    }
                }
                MoveHistory.Add(move);
                player = -player;
            } else if (m.MoveType == ChessMoveType.EnPassant) {
                if (player == 1) {
                    var EnemyPawn = new BoardPosition(m.EndPosition.Row + 1, m.EndPosition.Col);
                    m.Captured = GetPieceAtPosition(EnemyPawn);
                    SetPieceAtPosition(m.EndPosition, m.Piece);
                    SetPieceAtPosition(EnemyPawn, new ChessPiecePosition(ChessPieceType.Empty, 0));
                    SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                    player = -player;
                    MoveHistory.Add(move);
                } else {
                    var EnemyPawn = new BoardPosition(m.EndPosition.Row - 1, m.EndPosition.Col);
                    m.Captured = GetPieceAtPosition(EnemyPawn);
                    SetPieceAtPosition(m.EndPosition, m.Piece);
                    SetPieceAtPosition(EnemyPawn, new ChessPiecePosition(ChessPieceType.Empty, 0));
                    SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                    player = -player;
                    MoveHistory.Add(move);
                }

                if (m.Captured.PieceType != ChessPieceType.Empty) {
                    if (m.Captured.Player == 1) {
                        Value -= GetPieceValue(m.Captured.PieceType);
                    } else {
                        Value += GetPieceValue(m.Captured.PieceType);
                    }

                }

            } else{
                SetPieceAtPosition(m.EndPosition, m.Piece);
                SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));

                if(m.Captured.PieceType != ChessPieceType.Empty) {
                    if (player == 1) {
                        Value += GetPieceValue(m.Captured.PieceType);
                    } else {
                        Value -= GetPieceValue(m.Captured.PieceType);
                    }
                    
                }

                //update before
                if (m.Piece.PieceType == ChessPieceType.Pawn && (m.EndPosition.Row == 0 || m.EndPosition.Row == 7)) {
                    MoveHistory.Add(m);
                } else {
                    player = -player;
                    MoveHistory.Add(m);
                }
                
                

            }
		}



		public IEnumerable<IGameMove> GetPossibleMoves() {
            List<ChessMove> PossibleMoves = new List<ChessMove>();

            bool CanPromote = false;
            BoardPosition PawnPromoteEnd = new BoardPosition();

            if (MoveHistory.Any()) {
                ChessMove m = MoveHistory[MoveHistory.Count - 1] as ChessMove;
                if (m.Piece.PieceType == ChessPieceType.Pawn && (m.EndPosition.Row == 0 || m.EndPosition.Row == 7)) {
                    PawnPromoteEnd = m.EndPosition;
                    CanPromote = true;
                } else {
                    CanPromote = false;
                }
            }

            if (CanPromote) {
                ChessMove pawnPromoteBishop = new ChessMove(PawnPromoteEnd, new BoardPosition(-1, (int)ChessPieceType.Bishop), ChessMoveType.PawnPromote);
                ChessMove pawnPromoteQueen = new ChessMove(PawnPromoteEnd, new BoardPosition(-1, (int)ChessPieceType.Queen), ChessMoveType.PawnPromote);
                ChessMove pawnPromoteKnight = new ChessMove(PawnPromoteEnd, new BoardPosition(-1, (int)ChessPieceType.Knight), ChessMoveType.PawnPromote);
                ChessMove pawnPromoteRook = new ChessMove(PawnPromoteEnd, new BoardPosition(-1, (int)ChessPieceType.RookPawn), ChessMoveType.PawnPromote);
                PossibleMoves.Add(pawnPromoteBishop);
                PossibleMoves.Add(pawnPromoteQueen);
                PossibleMoves.Add(pawnPromoteKnight);
                PossibleMoves.Add(pawnPromoteRook);
            } else {
                List<BoardPosition> ThreatenPositions = new List<BoardPosition>();
                ThreatenPositions.AddRange(GetThreatenedPositions(player * -1));
                foreach (int i in Enumerable.Range(0, 8)) { // another way of doing for i = 0 to < 8
                    foreach (int j in Enumerable.Range(0, 8)) {
                        BoardPosition position = new BoardPosition(i, j);
                        ChessPiecePosition p = GetPieceAtPosition(position);

                        if (p.Player == CurrentPlayer) {
                            switch (p.PieceType) {
                                case ChessPieceType.Pawn: //pawn - need to fix pawn class and also add pawn move fixes
                                    List<BoardPosition> pawnScan = new List<BoardPosition>();
                                    BoardPosition scanPos;

                                    pawnScan.AddRange(checkPawn(position));
                                    //add threat positions of pawn
                                    // if position in threaten is not an enemy
                                    for (int count = 0; count < pawnScan.Count(); count++) {
                                        if (PositionIsEmpty(pawnScan[count]) || GetPieceAtPosition(pawnScan[count]).Player == CurrentPlayer) {
                                            pawnScan.RemoveAt(count);
                                            count--;
                                        }
                                    }
                                    // List<BoardPosition> pawnThreat = ;
                                    //pawnScan.AddRange(
                                    //    from pos in checkPawn(position)
                                    //    where PositionIsEnemy(pos, CurrentPlayer)
                                    //    select pos
                                    //);
                                    if (player == 1) {
                                        //checks for init pos to move twice
                                        if (position.Row == 6 && PositionIsEmpty(position.Translate(-1,0))) {
                                            scanPos = position.Translate(-2, 0);
                                            if (PositionInBounds(scanPos) && PositionIsEmpty(scanPos)) {
                                                pawnScan.Add(scanPos);
                                            }
                                        }
                                        //enpassant
                                        if (MoveHistory.Any()) {
                                            ChessMove m = MoveHistory[MoveHistory.Count - 1] as ChessMove;
                                            if (m.Piece.PieceType == ChessPieceType.Pawn) {
                                                int moveDiference = Math.Abs(m.EndPosition.Row - m.StartPosition.Row);
                                                if (moveDiference == 2) {
                                                    BoardPosition Neighbor1 = new BoardPosition(m.EndPosition.Row, m.EndPosition.Col + 1);
                                                    BoardPosition Neighbor2 = new BoardPosition(m.EndPosition.Row, m.EndPosition.Col - 1);
                                                    if (PositionInBounds(Neighbor1)) {
                                                        if (GetPlayerAtPosition(Neighbor1) == CurrentPlayer && GetPieceAtPosition(Neighbor1).PieceType == ChessPieceType.Pawn) {
                                                            BoardPosition EndPos = new BoardPosition(m.EndPosition.Row - 1, m.EndPosition.Col);
                                                            ChessMove EnPassant = new ChessMove(position, EndPos, ChessMoveType.EnPassant);
                                                            PossibleMoves.Add(EnPassant);
                                                        }
                                                    }

                                                    if (PositionInBounds(Neighbor2)) {
                                                        if (GetPlayerAtPosition(Neighbor2) == CurrentPlayer && GetPieceAtPosition(Neighbor2).PieceType == ChessPieceType.Pawn) {
                                                            BoardPosition EndPos = new BoardPosition(m.EndPosition.Row - 1, m.EndPosition.Col);
                                                            ChessMove EnPassant = new ChessMove(position, EndPos, ChessMoveType.EnPassant);
                                                            PossibleMoves.Add(EnPassant);
                                                        }
                                                    }

                                                }
                                            }
                                        }

                                        //allow moving once
                                        scanPos = position.Translate(-1, 0);
                                        if (PositionInBounds(scanPos) && PositionIsEmpty(scanPos)) {
                                            pawnScan.Add(scanPos);
                                        }
                                    } else {
                                        //black pawn
                                        //move twice
                                        if (position.Row == 1) {
                                            scanPos = position.Translate(2, 0);
                                            if (PositionInBounds(scanPos) && PositionIsEmpty(scanPos)) {
                                                pawnScan.Add(scanPos);
                                            }
                                        }
                                        //enpassant
                                        if (MoveHistory.Any()) {
                                            ChessMove m = MoveHistory[MoveHistory.Count - 1] as ChessMove;
                                            if (m.Piece.PieceType == ChessPieceType.Pawn) {
                                                int moveDiference = Math.Abs(m.EndPosition.Row - m.StartPosition.Row);
                                                if (moveDiference == 2) {
                                                    BoardPosition Neighbor1 = new BoardPosition(m.EndPosition.Row, m.EndPosition.Col + 1);
                                                    BoardPosition Neighbor2 = new BoardPosition(m.EndPosition.Row, m.EndPosition.Col - 1);
                                                    if (PositionInBounds(Neighbor1)) {
                                                        if (GetPlayerAtPosition(Neighbor1) == CurrentPlayer && GetPieceAtPosition(Neighbor1).PieceType == ChessPieceType.Pawn) {
                                                            BoardPosition EndPos = new BoardPosition(m.EndPosition.Row + 1, m.EndPosition.Col);
                                                            ChessMove EnPassant = new ChessMove(position, EndPos, ChessMoveType.EnPassant);
                                                            PossibleMoves.Add(EnPassant);
                                                        }
                                                    }

                                                    if (PositionInBounds(Neighbor2)) {
                                                        if (GetPlayerAtPosition(Neighbor2) == CurrentPlayer && GetPieceAtPosition(Neighbor2).PieceType == ChessPieceType.Pawn) {
                                                            BoardPosition EndPos = new BoardPosition(m.EndPosition.Row + 1, m.EndPosition.Col);
                                                            ChessMove EnPassant = new ChessMove(position, EndPos, ChessMoveType.EnPassant);
                                                            PossibleMoves.Add(EnPassant);
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        //allow moving once
                                        scanPos = position.Translate(1, 0);
                                        if (PositionInBounds(scanPos) && PositionIsEmpty(scanPos)) {
                                            pawnScan.Add(scanPos);
                                        }
                                    }
                                    PossibleMoves.AddRange(scanMoves(position, pawnScan));


                                    break;
                                case ChessPieceType.RookQueen: //rookqueen
                                    PossibleMoves.AddRange(scanMoves(position, checkRook(position)));
                                    
                                    //PossibleMoves.AddRange(scanMoves(position, checkRook(position)));
                                    break;
                                case ChessPieceType.RookKing: //rookking
                                    PossibleMoves.AddRange(scanMoves(position, checkRook(position)));
                                    
                                    break;
                                case ChessPieceType.Knight: //knight
                                    PossibleMoves.AddRange(scanMoves(position, checkKnight(position)));
                                    
                                    break;
                                case ChessPieceType.Bishop: //bishop
                                    PossibleMoves.AddRange(scanMoves(position, checkBishop(position)));
                                    
                                    break;
                                case ChessPieceType.Queen: //queen
                                    PossibleMoves.AddRange(scanMoves(position, checkQueen(position)));
                                    
                                    break;
                                case ChessPieceType.King: //king
                                    List<BoardPosition> kingScan = new List<BoardPosition>();
                                    PossibleMoves.AddRange(scanMoves(position, checkKing(position)));

                                    bool NeighborInThreat = false;
                                    bool check = false;
                                    var kPos = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).FirstOrDefault();
                                    var threat = GetThreatenedPositions(-player);
                                    if (threat.Contains(kPos))
                                        check = true;

                                    if (player == 1) {

                                        foreach (ChessMove move in MoveHistory) {
                                            if (move.Piece.PieceType == ChessPieceType.King && move.Piece.Player == 1) {
                                                checkWKing = false;
                                            } else if (move.Piece.PieceType == ChessPieceType.RookKing && move.Piece.Player == 1) {
                                                checkWRookKing = false;
                                            } else if (move.Piece.PieceType == ChessPieceType.RookQueen && move.Piece.Player == 1) {
                                                checkWRookQueen = false;
                                            }
                                        }
                                        //Kingside Castling - need to add check if position has been moved
                                        if(threat.Contains(position.Translate(0, 2)) || threat.Contains(position.Translate(0, 1))) {
                                            NeighborInThreat = true;
                                        }
                                        if (GetPieceAtPosition(new BoardPosition(7,4)).PieceType == ChessPieceType.King && GetPieceAtPosition(new BoardPosition(7,7)).PieceType == ChessPieceType.RookKing) {
                                            if (PositionIsEmpty(position.Translate(0, 2)) && PositionIsEmpty(position.Translate(0, 1)) && (checkWKing && checkWRookKing) && !check && !NeighborInThreat) {
                                                ChessMove kingSideCastle = new ChessMove(new BoardPosition(7, 4), new BoardPosition(7, 6), ChessMoveType.CastleKingSide);
                                                PossibleMoves.Add(kingSideCastle);
                                            }
                                        }


                                        //Queenside Castling 
                                        NeighborInThreat = false;
                                        if (threat.Contains(position.Translate(0, -2)) || threat.Contains(position.Translate(0, -1))) {
                                            NeighborInThreat = true;
                                        }

                                        if (GetPieceAtPosition(new BoardPosition(7, 4)).PieceType == ChessPieceType.King && GetPieceAtPosition(new BoardPosition(7, 0)).PieceType == ChessPieceType.RookQueen) {
                                            if (PositionIsEmpty(position.Translate(0, -2)) && PositionIsEmpty(position.Translate(0, -1)) && PositionIsEmpty(position.Translate(0, -3)) && (checkWKing && checkWRookQueen) && !check && !NeighborInThreat) {
                                                ChessMove queenSideCastle = new ChessMove(new BoardPosition(7, 4), new BoardPosition(7, 2), ChessMoveType.CastleQueenSide);
                                                PossibleMoves.Add(queenSideCastle);
                                            }
                                        }

                                    } else {
                                        foreach (ChessMove move in MoveHistory) {
                                            if (move.Piece.PieceType == ChessPieceType.King && move.Piece.Player == CurrentPlayer) {
                                                checkBKing = false;
                                            } else if (move.Piece.PieceType == ChessPieceType.RookKing && move.Piece.Player == CurrentPlayer) {
                                                checkBRookKing = false;
                                            } else if (move.Piece.PieceType == ChessPieceType.RookQueen && move.Piece.Player == CurrentPlayer) {
                                                checkBRookQueen = false;
                                            }
                                        }
                                        //Kingside Castling - need to add check if position has been moved
                                        NeighborInThreat = false;
                                        if (threat.Contains(position.Translate(0, 2)) || threat.Contains(position.Translate(0, 1))) {
                                            NeighborInThreat = true;
                                        }
                                        if (GetPieceAtPosition(new BoardPosition(0, 4)).PieceType == ChessPieceType.King && GetPieceAtPosition(new BoardPosition(0, 7)).PieceType == ChessPieceType.RookKing) {
                                            if (PositionIsEmpty(position.Translate(0, 2)) && PositionIsEmpty(position.Translate(0, 1)) && (checkBKing && checkBRookKing) && !check && !NeighborInThreat) {
                                                ChessMove kingSideCastle = new ChessMove(new BoardPosition(0, 4), new BoardPosition(0, 6), ChessMoveType.CastleKingSide);
                                                PossibleMoves.Add(kingSideCastle);
                                            }
                                        }

                                        //Queenside Castling 
                                        NeighborInThreat = false;
                                        if (threat.Contains(position.Translate(0, -2)) || threat.Contains(position.Translate(0, -1))) {
                                            NeighborInThreat = true;
                                        }
                                        if (GetPieceAtPosition(new BoardPosition(0, 4)).PieceType == ChessPieceType.King && GetPieceAtPosition(new BoardPosition(0, 0)).PieceType == ChessPieceType.RookQueen) {
                                            if (PositionIsEmpty(position.Translate(0, -2)) && PositionIsEmpty(position.Translate(0, -1)) && PositionIsEmpty(position.Translate(0, -3)) && (checkBKing && checkBRookQueen) && !check && !NeighborInThreat) {
                                                ChessMove queenSideCastle = new ChessMove(new BoardPosition(0, 4), new BoardPosition(0, 2), ChessMoveType.CastleQueenSide);
                                                PossibleMoves.Add(queenSideCastle);
                                            }
                                        }
                                        
                                        
                                    }

                                    
                                    break;
                                case ChessPieceType.RookPawn: //rookpawn
                                    if (scanMoves(position, checkQueen(position)).Any()) {
                                        PossibleMoves.AddRange(scanMoves(position, checkRook(position)));
                                    }
                                    break;
                                default:
                                    break;

                            }
                        }

                    }

                }
            }

            //filters moves to only list moves that can save the king if needed.

            List<ChessMove> FinalMoveList = new List<ChessMove>();
            if (!CanPromote) {
                FinalMoveList.AddRange(filterMoves(PossibleMoves));
            } else {
                FinalMoveList.AddRange(PossibleMoves);
            }
            

            return FinalMoveList;
		}

		/// <summary>
		/// Gets a sequence of all positions on the board that are threatened by the given player. A king
		/// may not move to a square threatened by the opponent.
		/// </summary>
		public IEnumerable<BoardPosition> GetThreatenedPositions(int byPlayer) {
            int enemy = byPlayer;
            if(byPlayer == -1) {
                enemy = 2;
            }
            List<BoardPosition> ThreatenMove = new List<BoardPosition>();
            foreach (int i in Enumerable.Range(0, 8)) { // another way of doing for i = 0 to < 8
                foreach (int j in Enumerable.Range(0, 8)) {
                    BoardPosition position = new BoardPosition(i, j);
                    ChessPiecePosition p = GetPieceAtPosition(position);

                    if (p.Player == enemy) {
                        switch (p.PieceType) {
                            case ChessPieceType.Pawn: //pawn
                                ThreatenMove.AddRange(checkPawn(position));
                                break;
                            case ChessPieceType.RookQueen: //rookqueen
                                ThreatenMove.AddRange(checkRook(position));
                                break;
                            case ChessPieceType.RookKing: //rookking
                                ThreatenMove.AddRange(checkRook(position));
                                break;
                            case ChessPieceType.Knight: //knight
                                ThreatenMove.AddRange(checkKnight(position));
                                break;
                            case ChessPieceType.Bishop: //bishop
                                ThreatenMove.AddRange(checkBishop(position));
                                break;
                            case ChessPieceType.Queen: //queen
                                ThreatenMove.AddRange(checkQueen(position));
                                break;
                            case ChessPieceType.King: //king
                                ThreatenMove.AddRange(checkKing(position));
                                break;
                            case ChessPieceType.RookPawn: //rookpawn
                                ThreatenMove.AddRange(checkRook(position));
                                break;
                            default:
                                break;

                        }
                    }
                }
                
            }
            return ThreatenMove;
		}

        /// <summary>
		/// Returns a sequence of all positions that contain the given piece controlled by the given player.
		/// </summary>
		/// <returns>an empty sequence if the given player does not control any of the given piece type</returns>
        public IEnumerable<BoardPosition> GetPositionsOfPiece(ChessPieceType piece, int player) {
            List<BoardPosition> PositionsOfPiece = new List<BoardPosition>();
            foreach (int i in Enumerable.Range(0, 8)) { // another way of doing for i = 0 to < 8
                foreach (int j in Enumerable.Range(0, 8)) {
                    BoardPosition pos = new BoardPosition(i, j);
                    if (GetPieceAtPosition(pos).PieceType == piece && GetPlayerAtPosition(pos) == player) {
                        PositionsOfPiece.Add(pos);
                    }
                }
            }
            return PositionsOfPiece;
            // TODO: implement this method.
            //throw new NotImplementedException();
        }
        
        public void UndoLastMove() {

            if (MoveHistory.Any()) {
                ChessMove m = MoveHistory[MoveHistory.Count - 1] as ChessMove;
                if(m.MoveType == ChessMoveType.CastleKingSide) {
                    if(m.Piece.Player == 1) {
                        //original spaces
                        SetPieceAtPosition(new BoardPosition(7,4),new ChessPiecePosition(ChessPieceType.King,1));
                        SetPieceAtPosition(new BoardPosition(7,7),new ChessPiecePosition(ChessPieceType.RookKing,1));
                        //blank spaces
                        SetPieceAtPosition(new BoardPosition(7,5),new ChessPiecePosition(ChessPieceType.Empty,0));
                        SetPieceAtPosition(new BoardPosition(7,6), new ChessPiecePosition(ChessPieceType.Empty, 0));
                    } else {
                        SetPieceAtPosition(new BoardPosition(0,4), new ChessPiecePosition(ChessPieceType.King,2));
                        SetPieceAtPosition(new BoardPosition(0,7), new ChessPiecePosition(ChessPieceType.RookKing,2));
                        SetPieceAtPosition(new BoardPosition(0,5), new ChessPiecePosition(ChessPieceType.Empty, 0));
                        SetPieceAtPosition(new BoardPosition(0,6), new ChessPiecePosition(ChessPieceType.Empty, 0));
                    }
                    player = -player;
                    MoveHistory.RemoveAt(MoveHistory.Count - 1);
                } else if (m.MoveType == ChessMoveType.CastleQueenSide) {
                    if(m.Piece.Player == 1) {
                        SetPieceAtPosition(new BoardPosition(7, 4), new ChessPiecePosition(ChessPieceType.King, 1));
                        SetPieceAtPosition(new BoardPosition(7, 0), new ChessPiecePosition(ChessPieceType.RookQueen, 1));
                        SetPieceAtPosition(new BoardPosition(7, 2), new ChessPiecePosition(ChessPieceType.Empty, 0));
                        SetPieceAtPosition(new BoardPosition(7, 3), new ChessPiecePosition(ChessPieceType.Empty, 0));
                    } else {
                        SetPieceAtPosition(new BoardPosition(0, 4), new ChessPiecePosition(ChessPieceType.King, 2));
                        SetPieceAtPosition(new BoardPosition(0, 0), new ChessPiecePosition(ChessPieceType.RookQueen, 2));
                        SetPieceAtPosition(new BoardPosition(0, 2), new ChessPiecePosition(ChessPieceType.Empty, 0));
                        SetPieceAtPosition(new BoardPosition(0, 3), new ChessPiecePosition(ChessPieceType.Empty, 0));
                    }
                    player = -player;
                    MoveHistory.RemoveAt(MoveHistory.Count - 1);
                } else if (m.MoveType == ChessMoveType.EnPassant) {
                    if (m.Piece.Player == 1) {
                        BoardPosition EnemyPawnPos = new BoardPosition(m.EndPosition.Row + 1,m.EndPosition.Col);
                        SetPieceAtPosition(EnemyPawnPos,new ChessPiecePosition(ChessPieceType.Pawn,2));
                        SetPieceAtPosition(m.StartPosition, new ChessPiecePosition (ChessPieceType.Pawn, 1));
                        SetPieceAtPosition(m.EndPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                    } else {
                        BoardPosition EnemyPawnPos = new BoardPosition(m.EndPosition.Row - 1, m.EndPosition.Col);
                        SetPieceAtPosition(EnemyPawnPos, new ChessPiecePosition(ChessPieceType.Pawn, 1));
                        SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Pawn, 2));
                        SetPieceAtPosition(m.EndPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
                    }
                    if (!(m.Captured.PieceType == ChessPieceType.Empty)) {
                        if (m.Captured.Player == 1) {
                            Value += GetPieceValue(m.Captured.PieceType);
                        } else {
                            Value -= GetPieceValue(m.Captured.PieceType);
                        }
                    }
                    player = -player;
                    MoveHistory.RemoveAt(MoveHistory.Count - 1);
                } else if (m.MoveType == ChessMoveType.PawnPromote) {
                    SetPieceAtPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Pawn, m.Piece.Player));
                    if (m.EndPosition.Col == (int)ChessPieceType.Bishop) {
                        if (player == -1) {
                            Value += GetPieceValue(ChessPieceType.Pawn);
                            Value -= GetPieceValue(ChessPieceType.Bishop);
                        } else {
                            Value -= GetPieceValue(ChessPieceType.Pawn);
                            Value += GetPieceValue(ChessPieceType.Bishop);
                        }
                    } else if (m.EndPosition.Col == (int)ChessPieceType.RookPawn) {
                        if (player == -1) {
                            Value += GetPieceValue(ChessPieceType.Pawn);
                            Value -= GetPieceValue(ChessPieceType.RookPawn);
                        } else {
                            Value -= GetPieceValue(ChessPieceType.Pawn);
                            Value += GetPieceValue(ChessPieceType.RookPawn);
                        }
                    } else if (m.EndPosition.Col == (int)ChessPieceType.Queen) {
                        if (player == -1) {
                            Value += GetPieceValue(ChessPieceType.Pawn);
                            Value -= GetPieceValue(ChessPieceType.Queen);
                        } else {
                            Value -= GetPieceValue(ChessPieceType.Pawn);
                            Value += GetPieceValue(ChessPieceType.Queen);
                        }
                    } else { //knight
                        if (player == -1) {
                            Value += GetPieceValue(ChessPieceType.Pawn);
                            Value -= GetPieceValue(ChessPieceType.Knight);
                        } else {
                            Value -= GetPieceValue(ChessPieceType.Pawn);
                            Value += GetPieceValue(ChessPieceType.Knight);
                        }
                    }

                    MoveHistory.RemoveAt(MoveHistory.Count - 1);
                    player = -player;//removes the promotion move
                } else {
                    SetPieceAtPosition(m.EndPosition, m.Captured);
                    SetPieceAtPosition(m.StartPosition, m.Piece);

                    if(!(m.Captured.PieceType == ChessPieceType.Empty)) { //check logic when home
                        if(m.Captured.Player == 1) {
                            Value += GetPieceValue(m.Captured.PieceType);
                        } else {
                            Value -= GetPieceValue(m.Captured.PieceType);
                        }
                       
                    }

                    //check before
                    if (!(m.Piece.PieceType == ChessPieceType.Pawn && (m.EndPosition.Row == 0 || m.EndPosition.Row == 7))) {
                        player = -player;
                    }
                    MoveHistory.RemoveAt(MoveHistory.Count - 1);
                }
            } else {
                throw new Exception();
            }
		}

		
		/// <summary>
		/// Returns true if the given position on the board is empty.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEmpty(BoardPosition pos) {
            if (GetPieceAtPosition(pos).PieceType == ChessPieceType.Empty) {
                return true;
            }else {
                return false;
            }
			// TODO: implement this method, using GetGetPieceAtPosition for convenience.
			//throw new NotImplementedException();
		}

		/// <summary>
		/// Returns true if the given position contains a piece that is the enemy of the given player.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEnemy(BoardPosition pos, int player) {
            ChessPiecePosition piece = GetPieceAtPosition(pos);
            if (piece.Player == 0) {
                return false;
            } else if (piece.Player != player) {
                return true;
            } else {
                return false;
            }
		}

		/// <summary>
		/// Returns true if the given position is in the bounds of the board.
		/// </summary>
		public static bool PositionInBounds(BoardPosition pos) {
            return pos.Row >= 0 && pos.Row < BOARD_SIZE && pos.Col >= 0 && pos.Col < BOARD_SIZE;
			
		}

		/// <summary>
		/// Returns which player has a piece at the given board position, or 0 if it is empty.
		/// </summary>
		public int GetPlayerAtPosition(BoardPosition pos) {
            ChessPiecePosition piece = GetPieceAtPosition(pos);
            if (piece.Player == 1) {
                return 1;
            } else if (piece.Player == 2) {
                return 2;
            } else {
                return 0;
            }
		}

		/// <summary>
		/// Gets the value weight for a piece of the given type.
		/// </summary>
		/*
		 * VALUES:
		 * Pawn: 1
		 * Knight: 3
		 * Bishop: 3
		 * Rook: 5
		 * Queen: 9
		 * King: infinity (maximum integer value)
		 */
		public int GetPieceValue(ChessPieceType pieceType) {
            int t = (int)pieceType;
            int value = 0;
            switch (t) {
                case 1: //pawn
                    value = 1;
                    break;
                case 2: //rookqueen
                    value = 5;
                    break;
                case 3: //rookking
                    value = 5;
                    break;
                case 4: //knight
                    value = 3;
                    break;
                case 5: //bishop
                    value = 3;
                    break;
                case 6: //queen
                    value = 9;
                    break;
                case 7: //king
                    value = 100000;
                    break;
                case 8: //rookpawn
                    value = 5;
                    break;
                default:
                    break;
            }
            // TODO: implement this method.
            return value;
            //throw new NotImplementedException();
		}


		/// <summary>
		/// Manually places the given piece at the given position.
		/// </summary>
		// This is used in the constructor
		private void SetPieceAtPosition(BoardPosition position, ChessPiecePosition piece) {
			mBoard[position.Row, position.Col] = (sbyte)((int)piece.PieceType * (piece.Player == 2 ? -1 :
				piece.Player));
		}

        
        private List<BoardPosition> checkPawn (BoardPosition pos) {
            List<BoardPosition> newPos = new List<BoardPosition>();
            BoardPosition scanPos;
            //scan up left
            if(GetPieceAtPosition(pos).Player == 1) {
                scanPos = pos.Translate(-1, 1);
                if (PositionInBounds(scanPos)) {
                    newPos.Add(scanPos);
                }

                scanPos = pos.Translate(-1, -1);
                if (PositionInBounds(scanPos)) {
                    newPos.Add(scanPos);
                }
            } else {
                scanPos = pos.Translate(1, 1);
                if (PositionInBounds(scanPos)) {
                    newPos.Add(scanPos);
                }

                scanPos = pos.Translate(1, -1);
                if (PositionInBounds(scanPos)) {
                    newPos.Add(scanPos);
                }
            }
            
            return newPos;
        }

        private List<BoardPosition> checkKing(BoardPosition pos) {
            List<BoardPosition> newPos = new List<BoardPosition>();
            BoardPosition scanPos;
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    if (x == 0 && y == 0) continue;
                    scanPos = pos.Translate(x, y);
                    if (PositionInBounds(scanPos)) {
                        newPos.Add(scanPos);
                    }
                }
            }
            return newPos;
        }

        private List<BoardPosition> checkQueen(BoardPosition pos) {
            List<BoardPosition> newPos = new List<BoardPosition>();

            newPos.AddRange(checkRook(pos));
            newPos.AddRange(checkBishop(pos));

            return newPos;
        }

        private List<BoardPosition> checkRook(BoardPosition pos) {
            List<BoardPosition> newPos = new List<BoardPosition>();
            BoardPosition scanPos = pos;
            //scan up
            for (int i = 1; i <= BOARD_SIZE; i++) {
                scanPos = scanPos.Translate(-1, 0);
                if (PositionInBounds(scanPos)) {
                    if (PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                    } else if (!PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                        break;
                    }
                }

            }

            //scan down
            scanPos = pos;
            for (int i = 1; i<= BOARD_SIZE; i++) {
                scanPos = scanPos.Translate(1, 0);
                if (PositionInBounds(scanPos)) { 
                    if (PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                    } else if (!PositionIsEmpty(scanPos)) {  
                        newPos.Add(scanPos); 
                        break;
                    }
                } 
                
            }
            //scan right
            scanPos = pos;
            for (int i = 1; i <= BOARD_SIZE; i++) {
                scanPos = scanPos.Translate(0, 1);
                if (PositionInBounds(scanPos)) {
                    if (PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                    } else if (!PositionIsEmpty(scanPos)) {  // want to add positions that are empty once you hit non empty regardless if its enemy or own stop collecting
                        newPos.Add(scanPos);
                        break;
                    }
                } 

            }

            //scan left
            scanPos = pos;
            for (int i = 1; i <= BOARD_SIZE; i++) {
                scanPos = scanPos.Translate(0, -1);
                if (PositionInBounds(scanPos)) {
                    if (PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                    } else if (!PositionIsEmpty(scanPos)) {  // want to add positions that are empty once you hit non empty regardless if its enemy or own stop collecting
                        newPos.Add(scanPos);
                        break;
                    }
                }

            }


            return newPos;
        }

        private List<BoardPosition> checkBishop(BoardPosition pos) {
            List<BoardPosition> newPos = new List<BoardPosition>();
            //scan up right
            BoardPosition scanPos = pos;
            for (int i = 1; i <= BOARD_SIZE; i++) {
                scanPos = scanPos.Translate(-1, 1);
                if(PositionInBounds(scanPos)){
                    if (PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                    } else if (!PositionIsEmpty(scanPos)) {  
                        newPos.Add(scanPos);
                        break;
                    }
                }
            }

            //scan down right
            scanPos = pos;
            for (int i = 1; i <= BOARD_SIZE; i++) {
                scanPos = scanPos.Translate(1, 1);
                if (PositionInBounds(scanPos)) {
                    if (PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                    } else if (!PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                        break;
                    }
                }
            }

            //scan down left
            scanPos = pos;
            for (int i = 1; i <= BOARD_SIZE; i++) {
                scanPos = scanPos.Translate(1, -1);
                if (PositionInBounds(scanPos)) {
                    if (PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                    } else if (!PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                        break;
                    }
                }
            }

            //scan up left
            scanPos = pos;
            for (int i = 1; i <= BOARD_SIZE; i++) {
                scanPos = scanPos.Translate(-1, -1);
                if (PositionInBounds(scanPos)) {
                    if (PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                    } else if (!PositionIsEmpty(scanPos)) {
                        newPos.Add(scanPos);
                        break;
                    }
                }
            }
            return newPos;
        }

        private List<BoardPosition> checkKnight(BoardPosition pos) {
            List<BoardPosition> newPos = new List<BoardPosition>();
            BoardPosition scanPos;
            //scan up 1 right 2
            scanPos = pos.Translate(-1, 2);
            if (PositionInBounds(scanPos)) {
                newPos.Add(scanPos);
            }

            //scan up 2 right 1
            scanPos = pos.Translate(-2, 1);
            if (PositionInBounds(scanPos)) {
                newPos.Add(scanPos);
            }

            //scan down 1 right 2
            scanPos = pos.Translate(1, 2);
            if (PositionInBounds(scanPos)) {
                newPos.Add(scanPos);
            }

            //scan down 2 right 1
            scanPos = pos.Translate(2, 1);
            if (PositionInBounds(scanPos)) {
                newPos.Add(scanPos);
            }

            //scan down 2 left 1
            scanPos = pos.Translate(2, -1);
            if (PositionInBounds(scanPos)) {
                newPos.Add(scanPos);
            }

            //scan down 1 left 2
            scanPos = pos.Translate(1, -2);
            if (PositionInBounds(scanPos)) {
                newPos.Add(scanPos);
            }

            //scan up 1 left 2
            scanPos = pos.Translate(-1, -2);
            if (PositionInBounds(scanPos)) {
                newPos.Add(scanPos);
            }

            //scan up 2 left 1
            scanPos = pos.Translate(-2, -1);
            if (PositionInBounds(scanPos)) {
                newPos.Add(scanPos);
            }

            return newPos;
        }

        private List<ChessMove> scanMoves (BoardPosition startPos, List<BoardPosition> p) {
            List<ChessMove> moves = new List<ChessMove>();

            foreach (BoardPosition endPos in p){
                if (PositionIsEmpty(endPos) || PositionIsEnemy(endPos, CurrentPlayer)) {
                    ChessMove m = new ChessMove(startPos, endPos);
                    moves.Add(m);
                }

            }

            return moves;
        }
        
       

        public bool IsCheckmate {
            get {
                bool check = false;
                var kPos = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).FirstOrDefault();
                var threat = GetThreatenedPositions(-player);
                bool CanMove = GetPossibleMoves().Any();
                if (threat.Contains(kPos) && !CanMove) 
                    check = true;
                return check;
            }
        }
            

        public bool IsStalemate{
            get {
                bool check = false;
                bool CanMove = GetPossibleMoves().Any();
                var kPos = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).FirstOrDefault();
                var threat = GetThreatenedPositions(-player);
                if (!threat.Contains(kPos) && !CanMove)
                    check = true;
                return check;
            }   
        }
        
        public bool IsCheck {
            get {
                bool check = false;
                var kPos = GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).FirstOrDefault();
                var threat = GetThreatenedPositions(-player);
                if (threat.Contains(kPos) && GetPossibleMoves().Any())
                    check = true;
               
                return check;
            }
        }

        public int Weight {
            get {
                return Value;
            }
        }

        public bool IsFinished {
            get {
                return IsCheckmate || IsStalemate;
            }
        }

        /// <summary>
        /// filters the moves before adding them to the final list of possible moves
        /// </summary>
        /// <param name="moves"></param>
        /// <returns></returns>
        private List<ChessMove> filterMoves(List<ChessMove> m) {
            //apply move
            List<ChessMove> answers = new List<ChessMove>();
            bool kingCheck = false;
            List<BoardPosition> ThreatenedPositions = new List<BoardPosition>();
            for (int i = 0; i < m.Count; i++) {
                int cPlayer = player;
                ApplyMove(m[i]);
                var KingPositions = GetPositionsOfPiece(ChessPieceType.King, cPlayer == -1 ? 2 : 1);
                if (KingPositions.Any()) {
                    var kPos = KingPositions.First();
                    ThreatenedPositions.AddRange(GetThreatenedPositions(-cPlayer));
                    if (ThreatenedPositions.Contains(kPos))
                        kingCheck = true;
                }
                
                //if (!false) { // do not use IsCheck, instead see if king is under threat
                if (kingCheck == false) { 
                    answers.Add(m[i]);
                }

               
                if (MoveHistory.Any()) {
                    UndoLastMove();
                }


                kingCheck = false;
                ThreatenedPositions.Clear();
            }
            return answers;
        }
    }
}

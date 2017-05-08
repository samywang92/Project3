using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Cecs475.BoardGames;
using Cecs475.BoardGames.View;
using System;
using Cecs475.BoardGames.ComputerOpponent;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.Chess.View {
    public class ChessSquare : INotifyPropertyChanged {
        private ChessPiecePosition mPiece;
        public ChessPiecePosition Piece {
            get { return mPiece; }
            set {
                if (!value.Equals(mPiece)) {
                    mPiece = value;
                    OnPropertyChanged(nameof(Piece));
                }
            }
        }

        private bool mIsSelected;
        public bool IsSelected {
            get { return mIsSelected; }
            set {
                if (value != mIsSelected) {
                    mIsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public BoardPosition Position {
            get; set;
        }

        private bool mIsHovered;
        public bool IsHovered {
            get { return mIsHovered; }
            set {
                if (value != mIsHovered) {
                    mIsHovered = value;
                    OnPropertyChanged(nameof(IsHovered));
                }
            }
        }

        private bool mIsCheck;
        public bool IsCheck {
            get { return mIsCheck; }
            set {
                if (value != mIsCheck) {
                    mIsCheck = value;
                    OnPropertyChanged(nameof(IsCheck));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class ChessViewModel : INotifyPropertyChanged, IGameViewModel {
        private const int MAX_AI_DEPTH = 2;
        private ChessBoard mBoard;
        private ObservableCollection<ChessSquare> mSquares;
        private ChessSquare kingSquare = new ChessSquare();
        private IGameAi mGameAi = new MinimaxAi(MAX_AI_DEPTH);

        public event EventHandler GameFinished;
        public event PropertyChangedEventHandler PropertyChanged;
        //public event EventHandler CurrentPlayerMustPass;

        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ChessViewModel() {
            mBoard = new ChessBoard();
            mSquares = new ObservableCollection<ChessSquare>(
                from pos in (
                    from r in Enumerable.Range(0, 8)
                    from c in Enumerable.Range(0, 8)
                    select new BoardPosition(r, c)
                )
                select new ChessSquare() {
                    Position = pos,
                    Piece = mBoard.GetPieceAtPosition(pos)
                }
            );

            isCheck = mBoard.IsCheck;

            PossibleMoves = new HashSet<ChessMove>(
                from ChessMove m in mBoard.GetPossibleMoves()
                select m
                );


        }

        public int BoardValue {
            get {
                return mBoard.Value;
            }
        }

        public BoardPosition kingPos {
            get; private set;
        }

        public bool isCheck {
            get; private set;
        }

        public bool CanUndo {
            get {
                return mBoard.MoveHistory.Any();
            }
        }

        public int CurrentPlayer {
            get {
                return mBoard.CurrentPlayer;
            }
        }

        public ObservableCollection<ChessSquare> Squares {
            get { return mSquares; }
        }

        public HashSet<ChessMove> PossibleMoves {
            get; private set;
        }

        public ChessMove LastMove {
            get; private set;
        }

        public NumberOfPlayers Players { get; set; }

        public void UndoMove() {
            if (mBoard.MoveHistory.Any()) {

                if (LastMove.MoveType == ChessMoveType.PawnPromote) {
                    mBoard.UndoLastMove();
                    LastMove = mBoard.MoveHistory[mBoard.MoveHistory.Count - 1] as ChessMove;
                }


                mBoard.UndoLastMove();
                if (Players == NumberOfPlayers.One)
                {
                    mBoard.UndoLastMove();
                }
                if (!mBoard.IsCheck) {
                    kingSquare.IsCheck = false;
                }

                PossibleMoves = new HashSet<ChessMove>(
                     from ChessMove m in mBoard.GetPossibleMoves()
                     select m
                );

                kingPos = mBoard.GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).FirstOrDefault();
                isCheck = mBoard.IsCheck;
                if (isCheck) {
                    kingSquare = mSquares.Where(x => x.Position.Equals(kingPos)).FirstOrDefault();
                    kingSquare.IsCheck = true;
                }


                var newSquares =
                    from r in Enumerable.Range(0, 8)
                    from c in Enumerable.Range(0, 8)
                    select new BoardPosition(r, c);
                int i = 0;
                foreach (var pos in newSquares) {
                    mSquares[i].Piece = mBoard.GetPieceAtPosition(pos);
                    i++;
                }

                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(BoardValue));
                OnPropertyChanged(nameof(CurrentPlayer));
            }
        }

        private void RebindState() {
            PossibleMoves = new HashSet<ChessMove>(
                    from ChessMove m in mBoard.GetPossibleMoves()
                    select m
                );
            //CurrentPlayer == 2 ? 1:2
            kingPos = mBoard.GetPositionsOfPiece(ChessPieceType.King, CurrentPlayer).FirstOrDefault();
            isCheck = mBoard.IsCheck;
            if (isCheck) {
                kingSquare = mSquares.Where(x => x.Position.Equals(kingPos)).FirstOrDefault();
                kingSquare.IsCheck = true;
            }


            var newSquares =
                from r in Enumerable.Range(0, 8)
                from c in Enumerable.Range(0, 8)
                select new BoardPosition(r, c);

            int i = 0;
            foreach (var pos in newSquares) {
                mSquares[i].Piece = mBoard.GetPieceAtPosition(pos);
                i++;
            }


            OnPropertyChanged(nameof(BoardValue));
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(CanUndo));
        }

        public async Task ApplyMove(ChessMove chessMove) {

            //add ai pawn promotion

            var possMoves = mBoard.GetPossibleMoves() as IEnumerable<ChessMove>;
            foreach (var move in possMoves) {
                if (move.Equals(chessMove)) {
                    mBoard.ApplyMove(move);
                    LastMove = chessMove;
                    if (!mBoard.IsCheck) {
                        kingSquare.IsCheck = false;
                    }
                    break;
                }
            }

            //    PossibleMoves = new HashSet<ChessMove>(
            //        from ChessMove m in mBoard.GetPossibleMoves()
            //        select m
            //    );
            //    //CurrentPlayer == 2 ? 1:2
            //    kingPos = mBoard.GetPositionsOfPiece(ChessPieceType.King,CurrentPlayer).FirstOrDefault();
            //    isCheck = mBoard.IsCheck;
            //    if (isCheck) {
            //        kingSquare = mSquares.Where(x => x.Position.Equals(kingPos)).FirstOrDefault();
            //        kingSquare.IsCheck = true;
            //    }


            //    var newSquares =
            //        from r in Enumerable.Range(0, 8)
            //        from c in Enumerable.Range(0, 8)
            //        select new BoardPosition(r, c);

            //    int i = 0;
            //    foreach (var pos in newSquares) {
            //        mSquares[i].Piece = mBoard.GetPieceAtPosition(pos);
            //        i++;
            //    }

            //}
            //OnPropertyChanged(nameof(BoardValue));
            //OnPropertyChanged(nameof(CurrentPlayer));
            //OnPropertyChanged(nameof(CanUndo));
            RebindState();
            if (Players == NumberOfPlayers.One && !mBoard.IsFinished) {

                var bestMove = await Task.Run(() => mGameAi.FindBestMove(mBoard));
                if (bestMove != null) {
                    mBoard.ApplyMove(bestMove);
                    RebindState();
                }
            }

            if (mBoard.IsCheckmate || mBoard.IsStalemate) {
                GameFinished?.Invoke(this, new EventArgs());
            }
        }
    }


}

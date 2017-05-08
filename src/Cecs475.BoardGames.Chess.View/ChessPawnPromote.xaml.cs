using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.Chess.View {
    /// <summary>
    /// Interaction logic for ChessPawnPromote.xaml
    /// </summary>
    public partial class ChessPawnPromote : Window {
        private ChessMove LastMove;
        public ChessPawnPromote(ChessViewModel vm) {
            InitializeComponent();
            this.Resources.Add("cvm", vm);
            LastMove = vm.LastMove;
        }

        private async Task bishopButton_Click(object sender, RoutedEventArgs e) {
            var vm = FindResource("cvm") as ChessViewModel;
            LastMove = vm.LastMove;
            ChessMove pawnPromoteBishop = new ChessMove(LastMove.EndPosition, new BoardPosition(-1, (int)ChessPieceType.Bishop), ChessMoveType.PawnPromote);
            await vm.ApplyMove(pawnPromoteBishop);
            this.Close();
        }

        private async Task knightButton_Click(object sender, RoutedEventArgs e) {
            var vm = FindResource("cvm") as ChessViewModel;
            LastMove = vm.LastMove;
            ChessMove pawnPromoteBishop = new ChessMove(LastMove.EndPosition, new BoardPosition(-1, (int)ChessPieceType.Knight), ChessMoveType.PawnPromote);
            await vm.ApplyMove(pawnPromoteBishop);
            this.Close();
        }

        private async Task rookButton_Click(object sender, RoutedEventArgs e) {
            var vm = FindResource("cvm") as ChessViewModel;
            LastMove = vm.LastMove;
            ChessMove pawnPromoteBishop = new ChessMove(LastMove.EndPosition, new BoardPosition(-1, (int)ChessPieceType.RookPawn), ChessMoveType.PawnPromote);
            await vm.ApplyMove(pawnPromoteBishop);
            this.Close();
        }

        private async Task queenButton_Click(object sender, RoutedEventArgs e) {
            var vm = FindResource("cvm") as ChessViewModel;
            LastMove = vm.LastMove;
            ChessMove pawnPromoteBishop = new ChessMove(LastMove.EndPosition, new BoardPosition(-1, (int)ChessPieceType.Queen), ChessMoveType.PawnPromote);
            await vm.ApplyMove(pawnPromoteBishop);
            this.Close();
        }
    }
}

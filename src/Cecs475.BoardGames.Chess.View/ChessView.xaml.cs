using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.Chess.View {
    /// <summary>
    /// Interaction logic for ChessView.xaml
    /// </summary>
    public partial class ChessView : UserControl {
        public ChessView() {
            InitializeComponent();
        }
        private ChessSquare prevSquare = new ChessSquare();
        private void Border_MouseEnter(object sender, MouseEventArgs e) {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;
            var possibleStartMoves = vm.PossibleMoves.Select(x => x.StartPosition);
            var possibleEndMoves = vm.PossibleMoves.Where(x => x.StartPosition.Equals(prevSquare.Position)).Select(x => x.EndPosition).ToList();
            if (prevSquare.IsSelected) {
                if (possibleEndMoves.Contains(square.Position)) {
                    square.IsHovered = true;
                }
            } else {
                if (possibleStartMoves.Contains(square.Position)) {
                    square.IsHovered = true;
                }
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e) {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            square.IsHovered = false;
        }

        private async void Border_MouseUp(object sender, MouseButtonEventArgs e) {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;
            square.IsHovered = false;
            var possibleEndMoves = vm.PossibleMoves.Where(x => x.StartPosition.Equals(prevSquare.Position)).Select(x => x.EndPosition).ToList();
            var possibleStartMoves = vm.PossibleMoves.Select(x => x.StartPosition);
            if (prevSquare.IsSelected) {
                if (possibleEndMoves.Contains(square.Position)) {
                    ChessMove move = new ChessMove(prevSquare.Position, square.Position);
                    await vm.ApplyMove(move);
                    prevSquare.IsSelected = false;
                    if(square.Piece.PieceType == ChessPieceType.Pawn && (square.Position.Row == 0 || square.Position.Row == 7)) {
                        switch (square.Piece.Player) {
                            case 1:
                                ChessPawnPromote whitePromotionWin = new ChessPawnPromote(vm);
                                Nullable<bool> dialogResult = whitePromotionWin.ShowDialog();
                                break;
                            case 2:
                                ChessPawnPromoteBlack blackPromotionWin = new ChessPawnPromoteBlack(vm);
                                dialogResult = blackPromotionWin.ShowDialog();
                                break;
                        }
                    }
                }else {
                    prevSquare.IsSelected = false;
                }

            } else if (possibleStartMoves.Contains(square.Position)) {
                square.IsSelected = true;
                prevSquare = square;
            } else {
                prevSquare.IsSelected = false;
            }
        }
        

        public ChessViewModel Model {
            get { return FindResource("vm") as ChessViewModel; }
        }
    }
    
	public class ChessSquarePlayerConverter : IValueConverter {
        public object Convert(object values, Type targetType, object parameter, CultureInfo culture) {
            ChessPiecePosition piece = (ChessPiecePosition)values;

            if (piece.Player == 1) {
                string src = piece.PieceType.ToString();
                return new Image() {
                    Source = new BitmapImage(new Uri("/Cecs475.BoardGames.Chess.View;component/Resources/White" + src + ".png", UriKind.Relative))
                };
            } else {
                string src = piece.PieceType.ToString();
                return new Image() {
                    Source = new BitmapImage(new Uri("/Cecs475.BoardGames.Chess.View;component/Resources/Black" + src + ".png", UriKind.Relative))
            };
            }

        }

      
        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
        
    }
}



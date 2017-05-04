using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Cecs475.BoardGames.Chess.View {
    class ChessSquareColorConverter : IMultiValueConverter {
        private static SolidColorBrush BLUE_BRUSH = new SolidColorBrush(Colors.SteelBlue);
        private static SolidColorBrush HOVER_BRUSH = new SolidColorBrush(Colors.SkyBlue);
        private static SolidColorBrush SELECT_BRUSH = new SolidColorBrush(Colors.DodgerBlue);
        private static SolidColorBrush DEFAULT_BRUSH = new SolidColorBrush(Colors.GhostWhite);
        private static SolidColorBrush CHECK_BRUSH = new SolidColorBrush(Colors.LightYellow);
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            BoardPosition pos = (BoardPosition)values[0];
            bool isHovered = (bool)values[1];
            bool isSelected = (bool)values[2];
            bool isCheck = (bool)values[3];

            if (isHovered) {
                return HOVER_BRUSH;
            }

            if (isSelected) {
                return SELECT_BRUSH;
            }

            if (isCheck) {
                return CHECK_BRUSH;
            }

            if ((pos.Row + pos.Col)%2 == 0) {
                return BLUE_BRUSH;
            }

            return DEFAULT_BRUSH;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}

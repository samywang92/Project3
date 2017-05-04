using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cecs475.BoardGames.Chess.View {
    public class ChessValueConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            int i = (int)value;
            string text;
            if (i < 0) {
                text = $"Black has a {Math.Abs(i)} point advantage.";
                return text;
            } else if (i > 0) {
                text = $"White has a {i} point advantange.";
                return text;
            } else if (i == 0) {
                text = "Tie Game!";
                return text;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    
}

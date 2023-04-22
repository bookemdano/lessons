using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ARFUILib {
    static public class UIUtils {
        static Dictionary<System.Drawing.Color, Brush> _brushes = new Dictionary<System.Drawing.Color, Brush>();
        public static Brush GetBrush(System.Drawing.Color color) {
            if (!_brushes.ContainsKey(color))
                _brushes[color] = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            return _brushes[color];
        }
        public static Visibility IsVis(bool b) {
            if (b)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }


    }
}

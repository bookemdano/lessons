using System.Collections.Generic;
using System.Windows.Media;

namespace ARFCon {
    static internal class UILib {
        static Dictionary<System.Drawing.Color, Brush> _brushes = new Dictionary<System.Drawing.Color, Brush>();
        static internal Brush GetBrush(System.Drawing.Color color) {
            if (!_brushes.ContainsKey(color))
                _brushes[color] = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            return _brushes[color];
        }
    }
}

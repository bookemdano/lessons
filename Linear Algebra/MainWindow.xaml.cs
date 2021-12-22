using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Linear_Algebra
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextBox[] _ents;

        public MainWindow()
        {
            InitializeComponent();
            _ents = new TextBox[] { entB1, entB2, entB3, entB4 };
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            var v = Matrix.Read(entV.Text);
            Log("v=" + v);
            var bs = new List<Matrix>();
            foreach(var ent in _ents)
            {

                var b = Matrix.Read(ent.Text);
                if (b == null)
                    break;
                bs.Add(b);
                Log("b=" + b);
            }

            int i = 0;
            foreach(var b in bs)
                LogFraction("m" + (++i), v.Dot(b), b.LengthSquared);

        }
        void LogFraction(string name, int n, int d)
        {
            var fract = new Fraction(n, d);
            while (true)
            {
                var newFract = ReduceAll(fract);
                if (fract == newFract)
                    break;
                fract = newFract;
            }
            Log($"{name} = {n}/{d} = {fract}");
        }
        Fraction ReduceAll(Fraction fract)
        {
            var primes = new int[] { 5, 3, 2 };
            foreach (var prime in primes)
            {
                var res = Reduce(fract, prime);
                if (res != null)
                    return res;
            }
            return fract;

        }
        Fraction? Reduce(Fraction fract, int factor)
        {
            if (!(fract.Top % factor == 0 && fract.Bottom % factor == 0))
                return null;
            return new Fraction(fract.Top / factor, fract.Bottom / factor);
        }
        void Log(object o)
        {
            lst.Items.Add(o);
        }
    }
    public class Fraction
    {
        public Fraction(int top, int bottom)
        {
            Top = top;
            Bottom = bottom;
        }

        public int Top { get; set; }
        public int Bottom { get; set; }
        public override string ToString()
        {
            return $"{Top}/{Bottom}";
        }
    }

    public class Matrix
    {
        private int[] _values;

        public Matrix(IEnumerable<int> values)
        {
            _values = values.ToArray();
        }

        static public Matrix? Read(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            var parts = str.Split(' ');
            return new Matrix(parts.Select(p => int.Parse(p)));
        }

        public int LengthSquared
        {
            get
            {
                var rv = 0;
                foreach (var v in _values)
                    rv += v * v;
                return rv;
            }
        }

        internal int Dot(Matrix other)
        {
            var rv = 0;
            for (int i = 0; i < _values.Length; i++)
                rv += _values[i] * other._values[i];

            return rv;
        }
        public override string ToString()
        {
            return String.Join(" ", _values);
        }
    }
}

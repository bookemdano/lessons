using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Linear_Algebra
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        TextBox[] _ents;
        Matrix? _v;
        List<Matrix> _mats = new List<Matrix>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainWindow()
        {
            InitializeComponent();
            _ents = new TextBox[] { entB1, entB2, entB3, entB4 };
            Load(entV);
            foreach (var ent in _ents)
                Load(ent);
            DataContext = this;
            ent_TextChanged(null, null);
        }
        void Save()
        {
            Save(entV);
            foreach (var ent in _ents)
                Save(ent);
        }
        private void Load(TextBox ent)
        {
            if (System.IO.File.Exists($"{ent.Name}.cfg"))
                ent.Text = System.IO.File.ReadAllText($"{ent.Name}.cfg");
        }
        private void Save(TextBox ent)
        {
            System.IO.File.WriteAllText($"{ent.Name}.cfg", ent.Text);
        }

        public bool CanBasis
        {
            get
            {
                if (Vbar == null || Sbar == null)
                    return false;
                if (Vbar.Cols != 1)
                    return false;
                if (_mats.Count() < 2)
                    return false;
                foreach (var mat in _mats)
                {
                    if (mat.Cols != 1)
                        return false;
                    if (Vbar.Rows != mat.Rows)
                        return false;
                }
                return true;
            }
        }
        private void Basis_Click(object sender, RoutedEventArgs e)
        {
            Save();
            if (Vbar == null)
                return;
            Log("v=" + Vbar);
            int i = 0;
            foreach(var b in _mats)
                Log("m" + (++i), new Fraction(Vbar.Dot(b), b.LengthSquared));

        }
        void Log(string name, Fraction fract)
        {
            Log($"{name} = {fract} = {fract.Reduce()}");
        }
        void Log(string name, Matrix matrix)
        {
            var lines = matrix.ToLines();
            Log($"{name} = ");
            foreach (var line in lines)
                Log(line);
        }


        void Log(object o)
        {
            lst.Items.Add(o);
        }

        Matrix? Sbar
        {
            get
            {
                if (_mats?.Count() > 0)
                    return _mats[0];
                return null;
            }
        }
        Matrix? Vbar => _v;
        Matrix? Rbar => _v;

        public bool CanProject
        {
            get
            {
                if (Rbar == null || Sbar == null)
                    return false;
                if (Rbar.Cols != 1 || Sbar.Cols != 1)
                    return false;
                if (Rbar.Rows != Sbar.Rows)
                    return false;
                return true;
            }
        }
        private void Project_Click(object sender, RoutedEventArgs e)
        {
            Save();
            if (Rbar == null || Sbar == null)
                return;
            var fract = new Fraction(Rbar.Dot(Sbar), Rbar.Dot(Rbar));
            var newR = Rbar.Times(fract);
            Log("top", newR.Item1);
            Log("bottom", newR.Item2);
        }

        public bool CanMultiply
        {
            get
            {
                if (Vbar == null || Sbar == null)
                    return false;
                if (Vbar.Cols != Sbar.Rows)
                    return false;
                return true;
            }
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            Save();
            if (Rbar == null || Sbar == null)
                return;
            Log("s", Sbar);
            Log("r", Rbar);
            Log("result", Rbar.Multiply(Sbar));
        }

        private void ent_TextChanged(object sender, TextChangedEventArgs e)
        {
            _v = Matrix.Read(entV.Text);
            if (_ents != null)
            {
                _mats = new List<Matrix>();
                foreach (var ent in _ents)
                {

                    var b = Matrix.Read(ent.Text);
                    if (b == null)
                        break;
                    _mats.Add(b);
                }
            }
            OnPropertyChanged("CanMultiply");
            OnPropertyChanged("CanProject");
            OnPropertyChanged("CanBasis");
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Save();
            entV.Clear();
            foreach (var ent in _ents)
                ent.Clear();
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            Save();
            lst.Items.Clear();
        }
    }
    public class Fraction
    {
        public Fraction(decimal top, decimal bottom)
        {
            Top = top;
            Bottom = bottom;
        }

        public decimal Top { get; set; }
        public decimal Bottom { get; set; }
        public override string ToString()
        {
            if (Bottom == 1)
                return Top.ToString();
            return $"{Top}/{Bottom}";
        }

        internal Fraction Times(decimal val)
        {
            return new Fraction(Top * val, Bottom);
        }

        internal Fraction Reduce()
        {
            var fract = new Fraction(Top, Bottom);
            while (true)
            {
                var newFract = ReduceAll(fract);
                if (fract == newFract)
                    break;
                fract = newFract;
            }
            return fract;
        }
        static Fraction ReduceAll(Fraction fract)
        {
            if ((fract.Bottom % 1) != 0)
                return fract;
            var primes = new int[] { 7, 5, 3, 2 };
            foreach (var prime in primes)
            {
                var res = TryReduce(fract, prime);
                if (res != null)
                    return res;
            }
            return fract;

        }
        static Fraction? TryReduce(Fraction fract, int factor)
        {
            if (!(fract.Top % factor == 0 && fract.Bottom % factor == 0))
                return null;
            return new Fraction(fract.Top / factor, fract.Bottom / factor);
        }
    }

    public class Matrix
    {
        private readonly decimal[][] _values;

        public Matrix(int rows, int cols)
        {
            var values = new List<IEnumerable<decimal>>();
            for (int i = 0; i < cols; i++)
                values.Add(Enumerable.Repeat(0M, rows));
            _values = values.Select(c => c.ToArray()).ToArray();
        }
        public Matrix(IEnumerable<decimal> values)
        {
            _values = new decimal[][] { values.ToArray() };
        }

        public Matrix(IEnumerable<IEnumerable<decimal>> values)
        {
            _values = values.Select(c => c.ToArray()).ToArray();
        }
        internal int Rows
        {
            get
            {
                return _values[0].Length;
            }
        }
        internal int Cols
        {
            get
            {
                return _values.Length;
            }
        }
        static public Matrix? Read(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            try
            {
                var cols = new List<IEnumerable<decimal>>();
                var colStrings = str.Split('|');
                foreach (var colString in colStrings)
                {
                    var parts = colString.Split(' ');
                    cols.Add(parts.Select(p => decimal.Parse(p)).ToArray());
                }
                return new Matrix(cols);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public decimal LengthSquared
        {
            get
            {
                System.Diagnostics.Debug.Assert(Cols == 1);
                var rv = 0M;
                foreach (var v in _values[0])
                    rv += v * v;
                return rv;
            }
        }

        internal decimal Dot(Matrix other)
        {
            System.Diagnostics.Debug.Assert(_values.Length == 1);
            var rv = 0M;
            for (int i = 0; i < Cols; i++)
                rv += _values[0][i] * other._values[0][i];

            return rv;
        }
        public override string ToString()
        {
            var parts = new List<string>();
            foreach (var col in _values)
                parts.Add(string.Join(" ", col));
            return String.Join("|", parts);
        }

        internal Tuple<Fraction, Fraction> Times(Fraction fract)
        {
            System.Diagnostics.Debug.Assert(Cols == 1);
            var top = fract.Times(_values[0][0]);
            var bottom = fract.Times(_values[0][0]);
            return new Tuple<Fraction, Fraction>(top, bottom);
        }
        decimal Get(int iRow, int iCol)
        {
            return _values[iCol][iRow];
        }
        void Set(int iRow, int iCol, decimal value)
        {
            _values[iCol][iRow] = value;
        }

        internal Matrix Multiply(Matrix other)
        {
            System.Diagnostics.Debug.Assert(Cols == other.Rows);
            var rv = new Matrix(Rows, other.Cols);
            for (int i = 0; i < Rows; i++)
            {
                for (int k = 0; k < other.Cols; k++)
                {
                    var v = 0M;
                    for (int j = 0; j < Cols; j++)
                    {
                        v += Get(i, j) * other.Get(j, k);
                    }
                    rv.Set(i, k, v);
                }
            }
            return rv;
        }

        internal IEnumerable<string> ToLines()
        {
            var rv = new List<string>();
            for (int i = 0; i < Rows; i++)
            {
                var row = new List<decimal>();
                for (int j = 0; j < Cols; j++)
                    row.Add(Get(i, j));
                rv.Add(String.Join(" ", row));
            }
            return rv;
        }
    }
}

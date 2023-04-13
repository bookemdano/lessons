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

namespace ARFCon {
    /// <summary>
    /// Interaction logic for CustomizeWindow.xaml
    /// </summary>
    public partial class CustomizeWindow : Window {
        public bool ShowCustom { get; internal set; }

        public CustomizeWindow() {
            InitializeComponent();
            entStopText.Text = Config.StopText;
            entSlowText.Text = Config.SlowText;
            entCustomText.Text = Config.CustomText;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }

        private void Update_Click(object sender, RoutedEventArgs e) {
            Config.StopText = entStopText.Text;
            Config.SlowText = entSlowText.Text;
            Config.CustomText = entCustomText.Text;
            DialogResult = true;
            Close();
        }

        private void Show_Click(object sender, RoutedEventArgs e) {
            Config.StopText = entStopText.Text;
            Config.SlowText = entSlowText.Text;
            Config.CustomText = entCustomText.Text;
            ShowCustom = true;
            DialogResult = true;
            Close();
        }
    }
}

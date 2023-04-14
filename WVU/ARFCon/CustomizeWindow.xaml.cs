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
            entCameraName1.Text = Config.CameraName1;
            entCameraName2.Text = Config.CameraName2;
            entCameraAddress1.Text = Config.CameraAddress1;
            entCameraAddress2.Text = Config.CameraAddress2;
            entStopText.Text = Config.StopText;
            entSlowText.Text = Config.SlowText;
            entCustomText.Text = Config.CustomText;
            chkTesting.IsChecked = Config.LocalTesting;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }

        void SaveAll() {
            Config.CameraName1 = entCameraName1.Text;
            Config.CameraName2 = entCameraName2.Text;
            Config.CameraAddress1 = entCameraAddress1.Text;
            Config.CameraAddress2 = entCameraAddress2.Text;
            Config.StopText = entStopText.Text;
            Config.SlowText = entSlowText.Text;
            Config.CustomText = entCustomText.Text;
            Config.LocalTesting = chkTesting.IsChecked == true;
        }
        private void Update_Click(object sender, RoutedEventArgs e) {
            SaveAll();
            DialogResult = true;
            Close();
        }

        private void Show_Click(object sender, RoutedEventArgs e) {
            SaveAll();
            ShowCustom = true;
            DialogResult = true;
            Close();
        }
    }
}

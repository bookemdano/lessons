using ARFUILib;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
namespace ARFCon {

    public partial class CustomizeWindow : Window {
        public bool ShowCustom { get; internal set; }

        public CustomizeWindow() {
            InitializeComponent();
            entCameraName1.Text = Config.GetCameraName(0);
            entCameraName2.Text = Config.GetCameraName(1);
            entCameraAddress1.Text = Config.GetCameraAddress(0);
            entCameraAddress2.Text = Config.GetCameraAddress(1);
            entStopText.Text = Config.StopText;
            entSlowText.Text = Config.SlowText;
            entCustomText.Text = Config.CustomText;
            entStopColor.Text = Config.StopColor;
            entSlowColor.Text = Config.SlowColor;
            entCustomColor.Text = Config.CustomColor;
            entErrorColor.Text = Config.ErrorColor;
            chkTesting.IsChecked = Config.LocalTesting;
            pnlStop.Background = UIUtils.GetBrush(SignState.CalcColor(SignEnum.Stop));
            pnlSlow.Background = UIUtils.GetBrush(SignState.CalcColor(SignEnum.Slow));
            pnlCustom.Background = UIUtils.GetBrush(SignState.CalcColor(SignEnum.Custom));
            pnlError.Background = UIUtils.GetBrush(SignState.CalcColor(SignEnum.Error));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }

        void SaveAll() {
            Config.SetCameraName(0, entCameraName1.Text);
            Config.SetCameraName(1, entCameraName2.Text);
            Config.SetCameraAddress(0, entCameraAddress1.Text);
            Config.SetCameraAddress(1, entCameraAddress2.Text);
            Config.StopText = entStopText.Text;
            Config.SlowText = entSlowText.Text;
            Config.CustomText = entCustomText.Text;
            if (Color.FromName(entStopColor.Text).IsKnownColor)
                Config.StopColor = entStopColor.Text;
            if (Color.FromName(entSlowColor.Text).IsKnownColor)
                Config.SlowColor = entSlowColor.Text;
            if (Color.FromName(entCustomColor.Text).IsKnownColor)
                Config.CustomColor = entCustomColor.Text;
            if (Color.FromName(entErrorColor.Text).IsKnownColor)
                Config.ErrorColor = entErrorColor.Text;
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

        private void entStopColor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            UpdateColorMaybe(entStopColor, pnlStop);
        }

        private void entSlowColor_TextChanged(object sender, TextChangedEventArgs e) {
            UpdateColorMaybe(entSlowColor, pnlSlow);
        }

        private void entCustomColor_TextChanged(object sender, TextChangedEventArgs e) {
            UpdateColorMaybe(entCustomColor, pnlCustom);
        }
        void UpdateColorMaybe(TextBox ent, Panel pnl) {
            var color = System.Drawing.Color.FromName(ent.Text);
            if (color.IsKnownColor)
                pnl.Background = UIUtils.GetBrush(color);
        }

        private void entErrorColor_TextChanged(object sender, TextChangedEventArgs e) {
            UpdateColorMaybe(entErrorColor, pnlError);
        }

        private void Window_Activated(object sender, System.EventArgs e) {

        }
    }
}

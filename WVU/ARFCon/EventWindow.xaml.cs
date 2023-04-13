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
    /// Interaction logic for EventWindow.xaml
    /// </summary>
    public partial class EventWindow : Window {
        public EventWindow() {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            EventType = cmbEventType.Text;
            EventNotes = entEventNotes.Text;
            Close();
        }

        public string EventType { get; set; }
        public string EventNotes { get; set; }
    }
}

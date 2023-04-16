using System.Windows;

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

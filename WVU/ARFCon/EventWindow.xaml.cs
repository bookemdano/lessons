﻿using ARFUILib;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ARFCon {
    /// <summary>
    /// Interaction logic for EventWindow.xaml
    /// </summary>
    public partial class EventWindow : Window {
        public IEnumerable<ArfEvent> ArfEvents { get; }
        public EventWindow() {
            ArfEvents = ArfEvent.GetArfEvents().OrderByDescending(e => e.Timestamp);
            InitializeComponent();
            foreach(var e in ArfEvents) {
                lst.Items.Add(e);
            }
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

        private void ViewLog_Click(object sender, RoutedEventArgs e) {
            Logger.View();
        }
    }
}

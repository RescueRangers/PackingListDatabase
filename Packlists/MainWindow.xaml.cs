using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Packlists.Model;
using Packlists.ViewModel;

namespace Packlists
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private static void NotificationMessageReceived(NotificationMessage obj)
        {
            
            switch (obj.Notification)
            {
                case "ShowItemsPanel":
                    var itemsView = new ItemsView();
                    itemsView.Show();
                    break;
                case "ShowMaterialsPanel":
                    var materialsView = new MaterialsView();
                    materialsView.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        private void Calendar_DisplayModeChanged(object sender, System.Windows.Controls.CalendarModeChangedEventArgs e)
        {
            var calendar = (System.Windows.Controls.Calendar)sender;
            calendar.DisplayMode = System.Windows.Controls.CalendarMode.Year;
        }
        
    }
}
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using PacklistDatabase.Model;
using PacklistDatabase.ViewModel;

namespace PacklistDatabase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
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
                    itemsView.ShowDialog();
                    break;
                case "ShowMaterialsPanel":
                    var materialsView = new MaterialsView();
                    materialsView.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        private void Years_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var year = (Year) e.Row.Item;
            e.Cancel = year.IsEdited;
        }
    }
}
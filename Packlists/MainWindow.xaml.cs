using System.Windows;
using GalaSoft.MvvmLight.Messaging;
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
    }
}
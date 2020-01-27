using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Packlists.Properties;
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

                case "ShowImportPanel":
                    var importView = new ImportView();
                    importView.Show();
                    break;

                case "ShowMonthlyReport":
                    var reportView = new MonthlyUsageView();
                    reportView.Show();
                    break;

                case "ShowCOCs":
                    var cocsView = new COCsView();
                    cocsView.Show();
                    break;

                default:
                    break;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Default.Save();
            base.OnClosing(e);
        }
    }
}
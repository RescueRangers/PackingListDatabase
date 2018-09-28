using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Packlists.Properties;
using Packlists.ViewModel;

namespace Packlists
{
    /// <summary>
    /// Interaction logic for ItemsView.xaml
    /// </summary>
    public partial class ItemsView : Window
    {
        public ItemsView()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Settings.Default.Save();
            base.OnClosing(e);
        }
    }
}

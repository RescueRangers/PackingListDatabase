using System.ComponentModel;
using System.Windows;
using Packlists.Properties;
using Packlists.ViewModel;

namespace Packlists
{
    /// <summary>
    /// Interaction logic for MaterialsView.xaml
    /// </summary>
    public partial class MaterialsView : Window
    {
        public MaterialsView()
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

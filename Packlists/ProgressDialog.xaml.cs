using System.Windows;

namespace Packlists
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }
    }
}

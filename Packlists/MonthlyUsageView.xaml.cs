using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Packlists
{
    /// <summary>
    /// Interaction logic for MonthlyUsageView.xaml
    /// </summary>
    public partial class MonthlyUsageView : Window
    {
        public MonthlyUsageView()
        {
            InitializeComponent();
        }

        private void Dg_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var textCol = e.Column as DataGridTextColumn;
            if (textCol == null)
                return;
            var binding = textCol.Binding as Binding;
            if (binding == null)
                return;
            binding.Path = new PropertyPath("[" + binding.Path.Path + "]");
        }
    }
}

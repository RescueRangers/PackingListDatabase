using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

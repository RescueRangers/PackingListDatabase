using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace Packlists.Converters
{
    public class KeyboardEventArgsToKeyConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            var args = (KeyEventArgs) value;
            var key = args.Key;

            return key;
        }
    }
}

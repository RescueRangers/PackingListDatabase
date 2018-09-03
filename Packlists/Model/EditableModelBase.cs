using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Packlists.Model
{
    public abstract class EditableModelBase<T> : ViewModelBase, IEditableObject
    {
        private T Cache { get; set; }

        private object CurrentModel
        {
            get { return this; }
        }

        public RelayCommand CancelEditCommand
        {
            get { return new RelayCommand(CancelEdit); }
        }

        #region IEditableObject Members

        public void BeginEdit()
        {
            Cache = Activator.CreateInstance<T>();

            //Set Properties of Cache
            foreach (var info in CurrentModel.GetType().GetProperties())
            {
                if (!info.CanRead || !info.CanWrite) continue;
                var oldValue = info.GetValue(CurrentModel, null);
                Cache.GetType().GetProperty(info.Name).SetValue(Cache, oldValue, null);
            }
        }

        public void EndEdit()
        {
            Cache = default(T);
        }


        public void CancelEdit()
        {
            foreach (var info in CurrentModel.GetType().GetProperties())
            {
                if (!info.CanRead || !info.CanWrite) continue;
                var oldValue = info.GetValue(Cache, null);
                CurrentModel.GetType().GetProperty(info.Name).SetValue(CurrentModel, oldValue, null);
            }
        }

        #endregion
    }
}

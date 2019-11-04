﻿using System.Collections.ObjectModel;

namespace Packilists.DataAccess.Data
{
    public class Item
    {
        /// <summary>
        /// Sets and gets the ItemName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Sets and gets the Materials property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<MaterialAmount> Materials { get; set; }

        public int ItemId { get; set; }

        public override string ToString()
        {
            return ItemName;
        }
    }
}
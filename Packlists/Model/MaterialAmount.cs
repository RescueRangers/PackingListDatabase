﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Packlists.Model
{
    public class MaterialAmount : ObservableObject
    {
        private float _amount;

        /// <summary>
        /// Sets and gets the Amount property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public float Amount
        {
            get => _amount;
            set => Set(nameof(Amount), ref _amount, value);
        }

        private Material _material;

        /// <summary>
        /// Sets and gets the Material property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Material Material
        {
            get => _material;
            set => Set(nameof(Material), ref _material, value);
        }

        public int MaterialAmountId { get; set; }
    }
}
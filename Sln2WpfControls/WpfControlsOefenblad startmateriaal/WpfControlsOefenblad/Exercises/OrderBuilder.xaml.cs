using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Order Builder", Description = "CheckBox + RadioButton met samenvatting en reset.", Order = 5, IsVisible = true)]
    public partial class OrderBuilder : Page
    {
        public OrderBuilder()
        {
            InitializeComponent();
        }
    }
}

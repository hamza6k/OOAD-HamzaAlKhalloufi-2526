using System;
using System.Windows;
using System.Windows.Controls;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Select And Move", Description = "Items verplaatsen tussen twee ListBoxes.", Order = 9, IsVisible = true)]
    public partial class SelectAndMove : Page
    {
        public SelectAndMove()
        {
            InitializeComponent();
        }
    }
}
